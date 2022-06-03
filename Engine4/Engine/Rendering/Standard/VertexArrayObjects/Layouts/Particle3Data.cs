using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Particle3Data {
	[FieldOffset( 0 )]
	public Vector3 Translation;
	[FieldOffset( 12 )]
	public Vector2 RotationSineVector;
	[FieldOffset( 20 )]
	public Vector2 Scale;
	[FieldOffset( 28 )]
	public Color16x4 Color;
	[FieldOffset( 36 )]
	public ulong Texture1;
	[FieldOffset( 44 )]
	public ulong Texture2;
	[FieldOffset( 52 )]
	public float Blend;
}