using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Particle2Data {
	[FieldOffset( 0 )]
	public Vector2 Translation;
	[FieldOffset( 8 )]
	public Vector2 RotationSineVector;
	[FieldOffset( 16 )]
	public Vector2 Scale;
	[FieldOffset( 24 )]
	public Color16x4 Color;
	[FieldOffset( 32 )]
	public ulong Texture1;
	[FieldOffset( 40 )]
	public ulong Texture2;
	[FieldOffset( 48 )]
	public float Blend;
}
