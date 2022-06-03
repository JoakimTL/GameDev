using Engine.Data.Datatypes;

namespace TestPlatform.Voxels2.Data;

public interface IVoxelWorldGenerator {
	ushort GetId( Vector3i worldVoxelCoordinate );
}