using System.Runtime.InteropServices;
using Engine.Rendering.ResourceManagement;

namespace TestPlatform.Voxels.Rendering;
[BufferSizeManagement( 16_777_216, 16_777_216 )]
[StructLayout( LayoutKind.Explicit, Size = 17, Pack = 1 )]
public struct VoxelFaceData {
	[FieldOffset( 0 )]
	public int TranslationX;
	[FieldOffset( 4 )]
	public int TranslationY;
	[FieldOffset( 8 )]
	public int TranslationZ;
	[FieldOffset( 12 )]
	public byte ScaleX;
	[FieldOffset( 13 )]
	public byte ScaleY;
	[FieldOffset( 14 )]
	public byte ScaleZ;
	[FieldOffset( 15 )]
	public ushort Id;
}

