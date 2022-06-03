using System.Numerics;
using System.Runtime.InteropServices;

namespace TestPlatform.Voxels2.Render;

[StructLayout( LayoutKind.Explicit )]
public struct VoxelWorldModelData {
	[FieldOffset( 0 )]
	public Matrix4x4 ModelMatrix;
}

