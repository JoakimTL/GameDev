using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine;
using Engine.Data.Datatypes;
using Engine.Data.Datatypes.Transforms;
using TestPlatform.Voxels2.Render;

namespace TestPlatform.Voxels2.Data;

public class VoxelWorld : DisposableIdentifiable {

	//Multiplayer support
	//Singleplayer

	/*
	 * Multiplayer:
	 * - Server should keep a certain radius of chunks loaded around all players. Just like the renderer will load chunks, a worldsim will load chunks. The worldsim loaded chunks will always be of precision 1.
	 * Client (Singleplayer):
	 * - Client should keep the same radius as the server of chunks loaded around the player, with precision 1. Outside this radius a lower (higher number) precision can be used to render further away.
	 * - When the renderer requests a lower (higher number) precision chunk, but the chunk is already loaded, it will not count as access. That memory needs to be freed, so after the chunk load time runs out, a lower (higher number) precision chunk will be loaded in to replace it.
	 * 
	 * The world itself is just a storage place. Other actors have to keep it updated. It does have an automatic dispose function to dispose chunks outside of range.
	 * 
	 */

	private readonly Dictionary<Vector3i, VoxelChunk> _chunks;
	private readonly Queue<Vector3i> _chunkUpdates;
	private readonly HashSet<Vector3i> _updatedChunksTranslation;
	private readonly IVoxelWorldGenerator _worldGen;
	private readonly Thread[] _worldGenThreads;
	private readonly HashSet<Vector3i> _inGenQueue;
	private readonly BlockingCollection<VoxelChunk> _genQueue;
	private readonly AutoResetEvent _lodQueueEvent;
	private readonly ConcurrentQueue<VoxelChunk>[] _lodQueues;
	private readonly BlockingCollection<VoxelChunk> _discardedChunks;
	private readonly AutoResetEvent _consumerIndexBlock;
	private readonly List<IChunkCriteriaProducer> _chunkCriteriaProducers;
	protected readonly Transform3 _transform;
	public TransformReadonly<Vector3, Quaternion, Vector3> Transform => this._transform.Readonly;
	public AABB3i? Bounds { get; }

	public event Action<VoxelChunk>? ChunkInitialized;
	public event Action<VoxelChunk>? ChunkUpdated;

	public int ChunksInLodQueue => this._lodQueues.Sum( p => p.Count );
	public int CHunksInGen => this._genQueue.Count;

	public VoxelWorld( IVoxelWorldGenerator worldGen, AABB3i? bounds = null ) {
		this._transform = new Transform3();
		this._transform.Scale = new Vector3( 0.5f );
		this._chunks = new Dictionary<Vector3i, VoxelChunk>();
		this._chunkUpdates = new Queue<Vector3i>();
		this._updatedChunksTranslation = new HashSet<Vector3i>();
		this._inGenQueue = new HashSet<Vector3i>();
		this._genQueue = new();
		this._lodQueues = new ConcurrentQueue<VoxelChunk>[ 10 ];
		this._worldGen = worldGen;
		this._worldGenThreads = new Thread[ 2 ];
		this._lodQueueEvent = new AutoResetEvent( false );
		this._consumerIndexBlock = new AutoResetEvent( true );
		this._chunkCriteriaProducers = new();
		this._discardedChunks = new();
		for ( int i = 0; i < this._lodQueues.Length; i++ )
			this._lodQueues[ i ] = new ConcurrentQueue<VoxelChunk>();
		for ( int i = 0; i < this._worldGenThreads.Length; i++ )
			this._worldGenThreads[ i ] = Resources.Get<ThreadManager>().Start( WorldGenerationThread, $"WorldGen[{i}]" );
		Resources.Get<ThreadManager>().Start( WorldGenSortingThread, $"ChunkSorter" );
		Resources.Get<ThreadManager>().Start( ChunkDisposing, "ChunkDisposer" );
		this.Bounds = bounds;
	}

	private void QueueChunkGenInitialization( VoxelChunk chunk ) {
		this._lodQueues[ chunk.LodLevel ].Enqueue( chunk );
		this._lodQueueEvent.Set();
	}

	private void WorldGenSortingThread() {
		while ( !this.Disposed ) {
			this._lodQueueEvent.WaitOne();
			if ( this._genQueue.Count >= 10 )
				continue;
			int i;
			for ( i = 0; i < this._lodQueues.Length; i++ )
				if ( this._lodQueues[ i ].TryDequeue( out VoxelChunk? chunk ) ) {
					this._genQueue.Add( chunk );
					break;
				}
			if ( i < this._lodQueues.Length )
				this._lodQueueEvent.Set();
		}
	}

	private void WorldGenerationThread() {
		while ( !this.Disposed ) {
			if ( this._genQueue.TryTake( out VoxelChunk? chunk, -1 ) )
				InitializeChunk( chunk );
		}
	}

	private void ChunkDisposing() {
		while ( !this.Disposed ) {
			if ( this._discardedChunks.TryTake( out VoxelChunk? chunk, -1 ) )
				chunk.Dispose();
		}
	}

	public void Add( IChunkCriteriaProducer chunkCriteriaProducer ) => this._chunkCriteriaProducers.Add( chunkCriteriaProducer );

	public uint GetRequiredLod( Vector3i chunkTranslation ) {
		uint lodRequirement = uint.MaxValue;
		for ( int i = 0; i < this._chunkCriteriaProducers.Count; i++ ) {
			uint producerLodRequirement = this._chunkCriteriaProducers[ i ].GetLodRequirement( chunkTranslation );
			if ( producerLodRequirement < lodRequirement )
				lodRequirement = producerLodRequirement;
		}
		return lodRequirement;
	}

	private void InitializeChunk( VoxelChunk chunk ) {
		VoxelChunkData data = chunk.Data;
		data.AddUser();
		if ( data.Initialized )
			return;
		int length = (int) data.ActualLength;
		int voxelSize = (int) data.VoxelSize;
		Span<ushort> initGenData = stackalloc ushort[ length * length * length ];
		int index = 0;
		for ( int y = 0; y < length; y++ ) {
			for ( int z = 0; z < length; z++ ) {
				for ( int x = 0; x < length; x++ ) {
					initGenData[ index++ ] = this._worldGen.GetId( chunk.VoxelPosition + (x * voxelSize, y * voxelSize, z * voxelSize) );
				}
			}
		}
		data.Initialize( initGenData );
		ChunkInitialized?.Invoke( chunk );
		data.RemoveUser();
	}

	//Render gets chunk and creates chunkrenderer.
	//The chunk is initialized, and then the mesh is created. The render then gets a new chunk.
	//If the camera moves, the render starts anew.

	public void Update() {
		lock ( this._chunkUpdates ) {
			this._updatedChunksTranslation.Clear();
			while ( this._chunkUpdates.TryDequeue( out Vector3i update ) )
				this._updatedChunksTranslation.Add( update );
		}

		foreach ( Vector3i chunkTranslation in this._updatedChunksTranslation )
			if ( TryGetChunk( chunkTranslation, false, out VoxelChunk? chunk ) )
				ChunkUpdated?.Invoke( chunk );
	}

	/// <param name="canWait">Can wait for chunk to be initialized another time.</param>
	public bool TryGetChunk( Vector3i chunkTranslation, bool canWait, [NotNullWhen( true )] out VoxelChunk? chunk ) {
		chunk = null;
		if ( this.Bounds.HasValue && !chunkTranslation.Inside( this.Bounds.Value ) )
			return false;

		uint requiredLodLevel = GetRequiredLod( chunkTranslation );
		lock ( this._chunks ) {
			if ( this._chunks.TryGetValue( chunkTranslation, out chunk ) ) {
				if ( chunk.LodLevel == requiredLodLevel )
					return true;
				chunk.SetLodLevel( requiredLodLevel );
			} else {
				chunk = new VoxelChunk( chunkTranslation, requiredLodLevel );
				this._chunks.Add( chunkTranslation, chunk );
			}
		}

		if ( !canWait ) {
			//Initialize now
			InitializeChunk( chunk );
		} else {
			QueueChunkGenInitialization( chunk );
		}
		return true;
	}

	public ushort GetId( Vector3i voxelTranslation ) {
		if ( !TryGetChunk( ToChunkCoordinate( voxelTranslation ), false, out VoxelChunk? chunk ) || chunk is null )
			return 0;
		return chunk.GetIdFromGlobalPosition( voxelTranslation );
	}

	public void SetId( Vector3i voxelTranslation, ushort id ) {
		Vector3i chunkTranslation = ToChunkCoordinate( voxelTranslation );
		if ( !TryGetChunk( chunkTranslation, false, out VoxelChunk? chunk ) || chunk is null )
			return;
		Vector3i local = chunk.ToLocalPosition( voxelTranslation );
		chunk.SetIdFromLocalPosition( local, id );
		lock ( this._chunkUpdates ) {
			this._chunkUpdates.Enqueue( chunkTranslation );
			if ( local.X == 0 ) {
				this._chunkUpdates.Enqueue( chunkTranslation - (1, 0, 0) );
			} else if ( local.X == VoxelChunk.Length - 1 )
				this._chunkUpdates.Enqueue( chunkTranslation + (1, 0, 0) );
			if ( local.Y == 0 ) {
				this._chunkUpdates.Enqueue( chunkTranslation - (0, 1, 0) );
			} else if ( local.Y == VoxelChunk.Length - 1 )
				this._chunkUpdates.Enqueue( chunkTranslation + (0, 1, 0) );
			if ( local.Z == 0 ) {
				this._chunkUpdates.Enqueue( chunkTranslation - (0, 0, 1) );
			} else if ( local.Z == VoxelChunk.Length - 1 )
				this._chunkUpdates.Enqueue( chunkTranslation + (0, 0, 1) );
		}
	}

	public void SetVolumeId( AABB3i voxelVolume, Func<Vector3i, ushort?> id ) {
		AABB3i chunkVolume = new( ToChunkCoordinate( voxelVolume.Min - 1 ), ToChunkCoordinate( voxelVolume.Max + 1 ) );
		for ( int chunkY = chunkVolume.Min.Y; chunkY <= chunkVolume.Max.Y; chunkY++ )
			for ( int chunkZ = chunkVolume.Min.Z; chunkZ <= chunkVolume.Max.Z; chunkZ++ )
				for ( int chunkX = chunkVolume.Min.X; chunkX <= chunkVolume.Max.X; chunkX++ )
					if ( TryGetChunk( (chunkX, chunkY, chunkZ), false, out VoxelChunk? chunk ) && chunk is not null ) {
						Vector3i startLocal = Vector3i.Max( chunk.ToLocalPosition( voxelVolume.Min ), 0 );
						Vector3i endLocal = Vector3i.Min( chunk.ToLocalPosition( voxelVolume.Max ), (int) VoxelChunk.Length - 1 );
						for ( int localY = startLocal.Y; localY <= endLocal.Y; localY++ ) 
							for ( int localZ = startLocal.Z; localZ <= endLocal.Z; localZ++ ) 
								for ( int localX = startLocal.X; localX <= endLocal.X; localX++ ) {
									Vector3i local = (localX, localY, localZ);
									ushort? newId = id( local + chunk.VoxelPosition );
									if ( newId.HasValue )
										chunk.SetIdFromLocalPosition( local, newId.Value );
								}
					}

		lock ( this._chunkUpdates )
			for ( int chunkY = chunkVolume.Min.Y; chunkY <= chunkVolume.Max.Y; chunkY++ )
				for ( int chunkZ = chunkVolume.Min.Z; chunkZ <= chunkVolume.Max.Z; chunkZ++ )
					for ( int chunkX = chunkVolume.Min.X; chunkX <= chunkVolume.Max.X; chunkX++ ) {
						this._chunkUpdates.Enqueue( (chunkX, chunkY, chunkZ) );
					}
	}

	public void SetVolumeId( Vector3i a, Vector3i b, Func<Vector3i, ushort?> id ) => SetVolumeId( new AABB3i( a, b ), id );

	public Vector3i ToVoxelCoordinate( Vector3 worldTranslation ) => Vector3i.Floor( Vector3.Transform( worldTranslation - this._transform.GlobalTranslation, Quaternion.Inverse( this._transform.GlobalRotation ) ) / this._transform.GlobalScale );
	public static Vector3i ToVoxelCoordinate( Vector3i chunkTranslation ) => Vector3i.Floor( chunkTranslation * VoxelChunk.Length );
	public Vector3i ToChunkCoordinate( Vector3 worldTranslation ) => ToChunkCoordinate( ToVoxelCoordinate( worldTranslation ) );
	public static Vector3i ToChunkCoordinate( Vector3i voxelTranslation ) => Vector3i.Floor( voxelTranslation / VoxelChunk.Length );

	public bool TryGetChunk( Vector3 worldTranslation, bool canWait, out VoxelChunk? chunk ) => TryGetChunk( ToChunkCoordinate( worldTranslation ), canWait, out chunk );
	public ushort GetId( Vector3 worldTranslation ) => GetId( ToVoxelCoordinate( worldTranslation ) );
	public void SetId( Vector3 worldTranslation, ushort id ) => SetId( ToVoxelCoordinate( worldTranslation ), id );

	public VoxelWorldModelData GetWorldModelData() => new() { ModelMatrix = this._transform.Matrix };

	protected override bool OnDispose() {
		lock ( this._chunks ) {
			foreach ( VoxelChunk chunk in this._chunks.Values )
				chunk.Dispose();
			this._chunks.Clear();
		}
		return true;
	}
}

public interface IChunkCriteriaProducer {
	uint GetLodRequirement( Vector3i chunkTranslation );
}
