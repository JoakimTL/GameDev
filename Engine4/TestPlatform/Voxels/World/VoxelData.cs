using System.Numerics;
using System.Runtime.InteropServices;

namespace TestPlatform.Voxels.World;

[StructLayout( LayoutKind.Explicit, Size = 48 )]
public struct VoxelData {
	[FieldOffset( 0 )]
	public Vector4 DiffuseColor;
	[FieldOffset( 16 )]
	public Vector3 GlowColor;
	[FieldOffset( 28 )]
	public float Metallic;
	[FieldOffset( 32 )]
	public float Roughness;
}
