using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[StructLayout( LayoutKind.Explicit, Pack = 1 )]
public readonly struct Vertex3 {
	public const uint SIZE = 12 + 8 + 12 + 8;

	[FieldOffset( 0 )]
	public readonly Vector3 Translation;
	[FieldOffset( 12 )]
	public readonly Vector2 UV;
	[FieldOffset( 20 )]
	public readonly Vector3 Normal;
	[FieldOffset( 32 )]
	public readonly Color16x4 Color;

	public Vertex3( in Vector3 position ) {
		this.Translation = position;
		this.UV = default;
		this.Normal = default;
		this.Color = Color16x4.White;
	}

	public Vertex3( in Vector3 position, in Vector2 uv ) {
		this.Translation = position;
		this.UV = uv;
		this.Normal = default;
		this.Color = Color16x4.White;
	}

	public Vertex3( in Vector3 position, in Vector2 uv, in Vector3 normal, in Vector4 color ) {
		this.Translation = position;
		this.UV = uv;
		this.Normal = normal;
		this.Color = color;
	}

	public Vertex3( in Vector3 position, in Vector2 uv, in Vector3 normal, in Color16x4 color ) {
		this.Translation = position;
		this.UV = uv;
		this.Normal = normal;
		this.Color = color;
	}
}
