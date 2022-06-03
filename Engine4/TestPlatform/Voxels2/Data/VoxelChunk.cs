using Engine;
using Engine.Data.Datatypes;

namespace TestPlatform.Voxels2.Data;

public unsafe class VoxelChunk : DisposableIdentifiable {

	public const uint Length = 32;
	public const uint Area = Length * Length;
	public const uint ChunkVolume = Length * Length * Length;

	public event Action? OnInvalidated;

	public Vector3i VoxelPosition { get; }
	public Vector3i ChunkPosition { get; }
	public VoxelChunkData Data { get; private set; }
	public uint LodLevel => this.Data.LodLevel;
	public uint VoxelSize => this.Data.VoxelSize;
	public uint ActualLength => this.Data.ActualLength;
	public uint ActualArea => this.Data.ActualArea;
	public uint ActualVolume => this.Data.ActualVolume;
	public float LastAccess { get; private set; }
	public bool Invalidated { get; private set; }
	protected override string UniqueNameTag => this.ChunkPosition.ToString();

	/// <param name="lodLevel">Starts at 0, which means full voxel precision. Higher number means lower precision</param>
	public VoxelChunk( Vector3i chunkPosition, uint lodLevel ) {
		this.ChunkPosition = chunkPosition;
		this.VoxelPosition = chunkPosition * (int) Length;
		this.Data = new VoxelChunkData( lodLevel );
	}

	internal void SetLodLevel( uint newLodLevel ) {
		if ( newLodLevel == this.Data.LodLevel )
			return;
		Invalidate();
		this.Data.Dispose();
		this.Data = new VoxelChunkData( newLodLevel );
		this.Invalidated = false;
	}

	public Vector3i ToLocalPosition( Vector3i globalVoxelPosition ) => globalVoxelPosition - this.VoxelPosition;
	public Vector3i ToGlobalPosition( Vector3i localVoxelPosition ) => localVoxelPosition + this.VoxelPosition;
	public ushort GetIdFromGlobalPosition( Vector3i globalVoxelPosition ) => GetIdFromLocalPosition( ToLocalPosition( globalVoxelPosition ) );
	public bool SetIdFromGlobalPosition( Vector3i globalVoxelPosition, ushort id ) => SetIdFromLocalPosition( ToLocalPosition( globalVoxelPosition ), id );
	internal void SetLastAccess( float time ) => this.LastAccess = time;

	public uint GetLocalLodVoxelIndex( Vector3i localVoxelPosition ) {
		//Vector3i internalPosition = Vector3i.Floor( localVoxelPosition / this.VoxelSize * new Vector3( this._actualArea, this._actualLength, 1 ) );
		//return (uint) ( internalPosition.Y + internalPosition.Z + internalPosition.X );
		Vector3i internalPosition = Vector3i.Floor( localVoxelPosition / this.Data.VoxelSize );
		return (uint) ( ( internalPosition.Y * (int) this.Data.ActualArea ) + ( internalPosition.Z * (int) this.Data.ActualLength ) + internalPosition.X );
	}
	public static uint GetLocalFullVoxelIndex( Vector3i localVoxelPosition ) => (uint) ( ( localVoxelPosition.Y * Area ) + ( localVoxelPosition.Z * Length ) + localVoxelPosition.X );

	public uint GetLocalVoxelIndex( Vector3i localVoxelPosition ) => this.Data.VoxelSize == 1 ? GetLocalFullVoxelIndex( localVoxelPosition ) : GetLocalLodVoxelIndex( localVoxelPosition );

	public ushort GetIdFromLocalPosition( Vector3i localVoxelPosition ) => this.Data.GetIdFromIndex( GetLocalVoxelIndex( localVoxelPosition ) );
	public bool SetIdFromLocalPosition( Vector3i localVoxelPosition, ushort id ) {
		if ( this.Data.VoxelSize != 1 )
			return false;
		if ( localVoxelPosition.X < 0 || localVoxelPosition.X >= Length )
			throw new ArgumentOutOfRangeException( nameof( localVoxelPosition ), "Out of range on x-axis!" );
		if ( localVoxelPosition.Y < 0 || localVoxelPosition.Y >= Length )
			throw new ArgumentOutOfRangeException( nameof( localVoxelPosition ), "Out of range on y-axis!" );
		if ( localVoxelPosition.Z < 0 || localVoxelPosition.Z >= Length )
			throw new ArgumentOutOfRangeException( nameof( localVoxelPosition ), "Out of range on z-axis!" );
		return this.Data.SetIdFromIndex( GetLocalVoxelIndex( localVoxelPosition ), id );
	}

	internal void Invalidate() {
		this.Invalidated = true;
		OnInvalidated?.Invoke();
	}

	public bool IsValid() => !this.Invalidated && !this.Disposed && this.Data.Initialized;

	protected override bool OnDispose() {
		Invalidate();
		this.Data.Dispose();
		return true;
	}
}
