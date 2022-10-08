using System.Numerics;
using Engine;
using Engine.Data.Datatypes;

namespace TestPlatform.Voxels.World;
public class FiniteVoxelWorld : VoxelWorldBase {

	private readonly VoxelChunk?[] _chunks;
	private readonly Vector3i _chunkSize;
	private readonly int _areaXZ;
	private readonly IVoxelWorldGenerator _gen;
	private AABB3i _aabb;

	public override bool IsInfinite => false;

	public FiniteVoxelWorld( string name, Vector3i size, IVoxelWorldGenerator gen ) : base( name ) {
		this._chunkSize = new Vector3i( size.X / (int) VoxelChunk.SideLength, size.Y / (int) VoxelChunk.SideLength, size.Z / (int) VoxelChunk.SideLength );
		this._areaXZ = this._chunkSize.X * this._chunkSize.Z;
		this._aabb = new AABB3i( 0, this._chunkSize - 1 );
		this._chunks = new VoxelChunk[ this._chunkSize.X * this._chunkSize.Y * this._chunkSize.Z ];
		this.LogLine( $"{this._chunkSize}, {this._aabb.Min} -> {this._aabb.Max}, {this._chunks.Length}", Log.Level.LOW );
		this._gen = gen;
		Resources.GlobalService<ThreadManager>().Start( ChunkGeneration, "ChunkGeneration" );
	}

	public void SetTranslation( Vector3 translation ) => this._transform.Translation = translation;
	public void SetRotation( Quaternion rotation ) => this._transform.Rotation = rotation;
	public void SetScale( Vector3 scale ) => this._transform.Scale = scale;

	private void ChunkGeneration() {
		for ( int x = 0; x <= this._aabb.Max.X; x++ )
			for ( int z = 0; z <= this._aabb.Max.Z; z++ )
				for ( int y = 0; y <= this._aabb.Max.Y; y++ ) {
					ushort[] data = new ushort[ VoxelChunk.Volume ];
					Vector3i chunkTranslation = Vector3i.Floor( new Vector3( x, y, z ) * VoxelChunk.SideLength );
					int j = 0;
					for ( int yy = 0; yy < VoxelChunk.SideLength; yy++ )
						for ( int zz = 0; zz < VoxelChunk.SideLength; zz++ )
							for ( int xx = 0; xx < VoxelChunk.SideLength; xx++ )
								data[ j++ ] = this._gen.GetId( chunkTranslation + new Vector3i( xx, yy, zz ) );

					Vector3i chunkPosition = new( x * (int) VoxelChunk.SideLength, y * (int) VoxelChunk.SideLength, z * (int) VoxelChunk.SideLength );
					if ( VoxelChunk.CreateFromWorldGenData( this, chunkPosition, data, out VoxelChunk? chunk ) && chunk is not null )
						this._chunks[ GetChunkIndex( new Vector3i( x, y, z ) ) ] = chunk;
				}
	}

	private int GetChunkIndex( Vector3i translation ) => ( translation.Y * this._areaXZ ) + ( translation.Z * this._chunkSize.X ) + translation.X;

	public override VoxelChunk? GetChunk( Vector3i translation ) {
		Vector3i chunkTranslation = Vector3i.Floor( translation / VoxelChunk.SideLength );
		if ( !AABB3i.Inside( ref this._aabb, ref chunkTranslation ) )
			return null;
		return this._chunks[ GetChunkIndex( chunkTranslation ) ];
	}

	public override ushort GetId( Vector3i translation ) {
		VoxelChunk? chunk = GetChunk( translation );
		if ( chunk is null )
			return 0;
		return chunk.GetId( translation ); //Because translation is always 0, local and global translation are the same.
	}

	public override void SetId( Vector3i translation, ushort id ) {
		VoxelChunk? chunk = GetChunk( translation );
		if ( chunk is null )
			return;
		if ( chunk.SetId( chunk.ToLocal( translation ), id ) ) {  //Because translation is always 0, local and global translation are the same.
			chunk.TriggerVoxelChangeNoCheck( chunk.ToLocal( translation ) );
			chunk.TriggerVoxelChange( translation + new Vector3i( 1, 0, 0 ) );
			chunk.TriggerVoxelChange( translation + new Vector3i( -1, 0, 0 ) );
			chunk.TriggerVoxelChange( translation + new Vector3i( 0, 1, 0 ) );
			chunk.TriggerVoxelChange( translation + new Vector3i( 0, -1, 0 ) );
			chunk.TriggerVoxelChange( translation + new Vector3i( 0, 0, 1 ) );
			chunk.TriggerVoxelChange( translation + new Vector3i( 0, 0, -1 ) );
			chunk.TriggerVoxelRenderChange();
		}
	}

	protected override bool OnDispose() {
		for ( int i = 0; i < this._chunks.Length; i++ )
			this._chunks[ i ]?.Dispose();
		return true;
	}

	public override void GetChunks( AABB3i chunkArea, List<VoxelChunk?> chunks ) {
		Vector3i newMin = Vector3i.Max( chunkArea.Min, this._aabb.Min );
		Vector3i newMax = Vector3i.Min( chunkArea.Max, this._aabb.Max );

		chunks.Clear();
		for ( int y = newMin.Y; y <= newMax.Y; y++ )
			for ( int x = newMin.X; x <= newMax.X; x++ )
				for ( int z = newMin.Z; z <= newMax.Z; z++ )
					chunks.Add( this._chunks[ GetChunkIndex( new Vector3i( x, y, z ) ) ] );
	}

	public override bool HasGenerated( Vector3i voxelTranslation ) {
		Vector3i chunkTranslation = Vector3i.Floor( voxelTranslation / VoxelChunk.SideLength );
		if ( !AABB3i.Inside( ref this._aabb, ref chunkTranslation ) )
			return true;
		return this._chunks[ GetChunkIndex( chunkTranslation ) ] is not null;
	}
}
