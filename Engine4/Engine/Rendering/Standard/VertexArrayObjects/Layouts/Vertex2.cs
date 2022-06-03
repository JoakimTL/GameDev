using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;
[StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Vertex2 {
	public const uint SIZE = 8 + 8 + 8;

	[FieldOffset( 0 )]
	public readonly Vector2 Translation;
	[FieldOffset( 8 )]
	public readonly Vector2 UV;
	[FieldOffset( 16 )]
	public readonly Color16x4 Color;

	public Vertex2( Vector2 position, Vector2 uv, Vector4 color ) {
		this.Translation = position;
		this.UV = uv;
		this.Color = color;
	}

	public Vertex2( Vector2 position, Vector2 uv, Color16x4 color ) {
		this.Translation = position;
		this.UV = uv;
		this.Color = color;
	}
}
