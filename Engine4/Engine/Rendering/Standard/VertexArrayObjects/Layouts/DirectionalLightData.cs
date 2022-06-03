using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct DirectionalLightData {
	[FieldOffset( 0 )]
	public Color16x3 Color;
	[FieldOffset( 6 )]
	public float Intensity;
	[FieldOffset( 10 )]
	public Vector3 Direction;
}
