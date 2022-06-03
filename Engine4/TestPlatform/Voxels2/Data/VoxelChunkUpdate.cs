using Engine.Data.Datatypes;

namespace TestPlatform.Voxels2.Data;

public readonly struct VoxelChunkUpdate {
	public readonly Vector3i ChunkTranslation;
	public readonly Vector3i LocalVoxelTranslation;

	public VoxelChunkUpdate( Vector3i chunkTranslation, Vector3i localVoxelTranslation ) {
		this.ChunkTranslation = chunkTranslation;
		this.LocalVoxelTranslation = localVoxelTranslation;
	}
}