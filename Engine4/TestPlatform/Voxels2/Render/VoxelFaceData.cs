using System.Runtime.InteropServices;
using Engine.Data.Datatypes;
using Engine.Rendering.ResourceManagement;

namespace TestPlatform.Voxels2.Render;

[BufferSizeManagement( 16_777_216, 16_777_216 )]
[StructLayout( LayoutKind.Explicit, Size = 16, Pack = 1 )]
public struct VoxelFaceData {

	public VoxelFaceData(Vector3i translation, Vector3i scales, ushort id) {
		this.TranslationX = translation.X;
		this.TranslationY = translation.Y;
		this.TranslationZ = translation.Z;
		this.Scales = GetScales( (byte) scales.X, (byte) scales.Y, (byte) scales.Z );
		this.Id = id;
	}

	[FieldOffset( 0 )]
	public int TranslationX;
	[FieldOffset( 4 )]
	public int TranslationY;
	[FieldOffset( 8 )]
	public int TranslationZ;
	//5 bits for X-scale, 5 bits for Y-scale and 5 bits for Z-scale
	[FieldOffset( 12 )]
	public ushort Scales;
	[FieldOffset( 14 )]
	public ushort Id;

	public static ushort GetScales( byte x, byte y, byte z ) => (ushort) ( ( x & 0b1_1111 ) | ( ( y << 5 ) & 0b11_1110_0000 ) | ( ( z << 10 ) & 0b111_1100_0000_0000 ) );

	public override string ToString() => $"{this.Id}:[({this.TranslationX},{this.TranslationY},{this.TranslationZ}) + ({this.Scales & 0b1_1111},{( this.Scales & 0b11_1110_0000 ) >> 5},{( this.Scales & 0b111_1100_0000_0000 ) >> 10})]";
}
