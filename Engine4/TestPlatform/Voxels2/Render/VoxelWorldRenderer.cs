using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.InteropServices;
using Engine;
using Engine.Data.Datatypes;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;
using OpenGL;
using TestPlatform.Voxels2.Data;

namespace TestPlatform.Voxels2.Render;
public class VoxelWorldRenderer : DisposableIdentifiable, IChunkCriteriaProducer {

	//Separate the chunks and the render data. Minimize the draw calls during render
	//When chunks are redrawn, wipe the entirety of the chunk data from the render objects. This should be continous data segments. Because of the size of the chunks, this approach should be fine.

	//Chunks flag themselves when they are ready to be redrawn on the GPU. This means our render objects should be manually controlled VBOs, not RDOs. When a chunk flags itself ready, the render loop will wipe the render objects and fill in the new data.

	public VoxelWorld World { get; }
	public SceneRenderer SceneRender { get; }
	public uint SceneCount => this._scene.Count;

	public object SceneValidCount => this._scene.RenderCount;

	public object SceneRenderStages => this._scene.RenderStages;

	private readonly Scene _scene;
	private readonly VoxelRenderDataManager[] _renderDataManagers;
	private readonly int _chunkViewRange;
	private readonly Dictionary<Vector3i, VoxelChunkFaceRenderer> _renderers;
	private readonly DataBlockCollection _dataBlocks;
	private readonly UniformBlock _worldModelData;

	private readonly AutoResetEvent _redrawnLock;
	private readonly Queue<(VoxelChunkFaceRenderer renderer, IReadOnlyList<VoxelFaceData[]> data)> _redrawnRenderers;
	private readonly Thread[] _chunkUpdateDrawThreads;
	private readonly Thread[] _chunkCreationDrawThreads;
	private readonly BlockingCollection<VoxelChunkFaceRenderer> _newChunkDrawRenderQueue;
	private readonly BlockingCollection<VoxelChunkFaceRenderer> _updatedChunkDrawRenderQueue;

	private readonly Vector3i[] _chunkPriorityMap;
	private Vector3 _lastCameraLocation;
	private Vector3i _lastCameraChunkTranslation;
	private int _chunkCheckIndex;

	public int LodBounds { get; }

	public string DisplayData => $"({this._chunkCheckIndex},{this._newChunkDrawRenderQueue.Count},{this._updatedChunkDrawRenderQueue.Count})";

	public VoxelWorldRenderer( VoxelWorld world, int chunkViewRange, int lodBounds ) {
		this.World = world ?? throw new ArgumentNullException( nameof( world ) );
		this.World.ChunkUpdated += WorldChunkUpdated;
		this.World.ChunkInitialized += WorldChunkGenerated;
		this._scene = new LayerlessScene();
		this._chunkViewRange = chunkViewRange;
		this.LodBounds = lodBounds;
		this._renderDataManagers = new VoxelRenderDataManager[ 6 ];
		this._renderers = new Dictionary<Vector3i, VoxelChunkFaceRenderer>();
		this._dataBlocks = new DataBlockCollection( this._worldModelData = new UniformBlock( "VoxelWorldModelDataBlock", (uint) Marshal.SizeOf<VoxelWorldModelData>(), ShaderType.VertexShader ) );
		//this._voxelDataInBlockCollection = VoxelIdRenderData.TryAddVoxelData( this._dataBlocks );
		this.SceneRender = new SceneRenderer( this._scene, this._dataBlocks );
		this._redrawnLock = new AutoResetEvent( true );
		this._redrawnRenderers = new Queue<(VoxelChunkFaceRenderer renderer, IReadOnlyList<VoxelFaceData[]> data)>();
		this._newChunkDrawRenderQueue = new BlockingCollection<VoxelChunkFaceRenderer>();
		this._updatedChunkDrawRenderQueue = new BlockingCollection<VoxelChunkFaceRenderer>();
		this._chunkPriorityMap = GeneratePriorityMap( chunkViewRange );
		this._chunkUpdateDrawThreads = new Thread[ 4 ];
		this._chunkCreationDrawThreads = new Thread[ 2 ];
		for ( int i = 0; i < this._renderDataManagers.Length; i++ )
			this._renderDataManagers[ i ] = new VoxelRenderDataManager( (VoxelFaceDirection) i, this._scene );
		for ( int i = 0; i < this._chunkUpdateDrawThreads.Length; i++ )
			this._chunkUpdateDrawThreads[ i ] = Resources.Get<ThreadManager>().Start( ChunkUpdateDrawThread, $"UpdateDraw[{i}]" );
		for ( int i = 0; i < this._chunkCreationDrawThreads.Length; i++ )
			this._chunkCreationDrawThreads[ i ] = Resources.Get<ThreadManager>().Start( ChunkCreationDrawThread, $"CreationDraw[{i}]" );
	}

	private void WorldChunkUpdated( VoxelChunk chunk ) {
		if ( GetChunkDistance( chunk.ChunkPosition, this._lastCameraChunkTranslation ) > this._chunkViewRange )
			return;
		lock ( this._renderers ) {
			if ( this._renderers.TryGetValue( chunk.ChunkPosition, out VoxelChunkFaceRenderer? renderer ) )
				this._updatedChunkDrawRenderQueue.Add( renderer );
		}
	}

	private void WorldChunkGenerated( VoxelChunk chunk ) {
		if ( GetChunkDistance( chunk.ChunkPosition, this._lastCameraChunkTranslation ) > this._chunkViewRange )
			return;
		lock ( this._renderers ) {
			if ( this._renderers.TryGetValue( chunk.ChunkPosition, out VoxelChunkFaceRenderer? renderer ) ) {
				renderer.SetChunk( chunk );
			} else {
				renderer = new VoxelChunkFaceRenderer( this, chunk );
				renderer.NeedNewDraw += ChunkNeedNewDraw;
				renderer.Redrawn += RedrawChunk;
				this._newChunkDrawRenderQueue.TryAdd( renderer );
				this._renderers.Add( chunk.ChunkPosition, renderer );
			}
		}
	}

	private void ChunkNeedNewDraw( VoxelChunkFaceRenderer renderer ) => this._newChunkDrawRenderQueue.Add( renderer );

	private Vector3i[] GeneratePriorityMap( int chunkViewRange ) {
		int actualSize = (chunkViewRange * 2) + 1;
		Vector3i[] map = new Vector3i[ actualSize * actualSize * actualSize ];
		int index = 0;
		for ( int y = -chunkViewRange; y <= chunkViewRange; y++ )
			for ( int x = -chunkViewRange; x <= chunkViewRange; x++ )
				for ( int z = -chunkViewRange; z <= chunkViewRange; z++ )
					map[ index++ ] = (x, y, z);
		Array.Sort( map, MapComparator );
		return map;
	}

	private int MapComparator( Vector3i x, Vector3i y ) {
		Vector3i absA = Vector3i.Abs( x );
		Vector3i absB = Vector3i.Abs( y );
		int distA = Math.Max( Math.Max( absA.X, absA.Y ), absA.Z );
		int distB = Math.Max( Math.Max( absB.X, absB.Y ), absB.Z );
		return distA - distB;
	}

	private void ChunkCreationDrawThread() {
		while ( !this.Disposed ) {
			if ( this._newChunkDrawRenderQueue.TryTake( out VoxelChunkFaceRenderer? renderer, -1 ) )
				if ( !renderer.Redraw() )
					this._newChunkDrawRenderQueue.Add( renderer );
		}
	}

	private void ChunkUpdateDrawThread() {
		while ( !this.Disposed ) {
			if ( this._updatedChunkDrawRenderQueue.TryTake( out VoxelChunkFaceRenderer? renderer, -1 ) )
				if ( !renderer.Redraw() )
					this._updatedChunkDrawRenderQueue.Add( renderer );
		}
	}

	//Render requests chunks -> Chunks are generated -> render receives event from world that chunk is generated -> chunk mesh is generated -> redrawn -> updated to gpu
	//Chunk updated -> added to queue in world render -> render thread redraws thread -> redrawn -> updated to gpu
	//When a redrawn is triggered, a queue in the world render is enqueued, the update method will clear them

	public bool GetNextChunkTranslation( out Vector3i chunkTranslation ) {
		Vector3i cameraChunkTranslation = this.World.ToChunkCoordinate( this._lastCameraLocation );
		if ( this._lastCameraChunkTranslation != cameraChunkTranslation ) {
			this._lastCameraChunkTranslation = cameraChunkTranslation;
			this._chunkCheckIndex = 0;
		}

		chunkTranslation = 0;
		if ( this._chunkCheckIndex >= this._chunkPriorityMap.Length )
			return false;
		chunkTranslation = this._chunkPriorityMap[ this._chunkCheckIndex ] + this._lastCameraChunkTranslation;
		++this._chunkCheckIndex;
		return true;
	}

	public uint GetLodRequirement( Vector3i chunkTranslation ) => (uint) ( GetChunkDistance( chunkTranslation, this._lastCameraChunkTranslation ) / this.LodBounds );

	public static int GetChunkDistance( Vector3i chunkTranslationA, Vector3i chunkTranslationB ) {
		Vector3i absDiff = Vector3i.Abs( chunkTranslationA - chunkTranslationB );
		return Math.Max( Math.Max( absDiff.X, absDiff.Y ), absDiff.Z );
	}

	public void Update( Vector3 cameraLocation ) {
		this._worldModelData.DirectWrite( this.World.GetWorldModelData() );
		this._lastCameraLocation = cameraLocation;
		Vector3i cameraChunkTranslation = this.World.ToChunkCoordinate( cameraLocation );
		if ( this._lastCameraChunkTranslation != cameraChunkTranslation ) {
			this._lastCameraChunkTranslation = cameraChunkTranslation;
			this._chunkCheckIndex = 0;
		}

		if ( GetNextChunkTranslation( out Vector3i chunkTranslation ) )
			if ( this.World.TryGetChunk( chunkTranslation, true, out VoxelChunk? chunk ) ) {

			}

		try {
			this._redrawnLock.WaitOne();
			while ( this._redrawnRenderers.TryDequeue( out (VoxelChunkFaceRenderer renderer, IReadOnlyList<VoxelFaceData[]> data) renderData ) ) {
				for ( int i = 0; i < this._renderDataManagers.Length; i++ ) {
					this._renderDataManagers[ i ].Clear( renderData.renderer );
					if ( renderData.renderer.IsValid() ) {
						this._renderDataManagers[ i ].Fill( renderData.renderer, renderData.data[ i ] );
					}
				}
			}
		} finally {
			this._redrawnLock.Set();
		}
	}

	private void RedrawChunk( VoxelChunkFaceRenderer renderer, IReadOnlyList<VoxelFaceData[]> data ) {
		try {
			this._redrawnLock.WaitOne();
			this._redrawnRenderers.Enqueue( (renderer, data) );
		} finally {
			this._redrawnLock.Set();
		}
	}

	protected override bool OnDispose() {
		for ( int i = 0; i < this._renderDataManagers.Length; i++ ) {
			this._renderDataManagers[ i ].Dispose();
		}
		this._scene.Dispose();
		return true;
	}
}
