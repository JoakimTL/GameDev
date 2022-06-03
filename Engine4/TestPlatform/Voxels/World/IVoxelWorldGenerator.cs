using Engine.Data.Datatypes;

namespace TestPlatform.Voxels.World;

public interface IVoxelWorldGenerator {
	ushort GetId( Vector3i worldVoxelCoordinate );
}