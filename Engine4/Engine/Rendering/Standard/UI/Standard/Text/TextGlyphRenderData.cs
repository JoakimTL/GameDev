using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.UI.Standard.Text;

[StructLayout( LayoutKind.Explicit )]
public readonly struct TextGlyphRenderData {
	[FieldOffset( 0 )]
	public readonly Vector3 Translation;
	[FieldOffset( 12 )]
	public readonly Vector2 Scale;
	[FieldOffset( 20 )]
	public readonly float Rotation;
	[FieldOffset( 24 )]
	public readonly Vector4 UV;
	[FieldOffset( 40 )]
	public readonly Color16x4 Color;
	[FieldOffset( 48 )]
	public readonly long FontTextureHandle;

	public TextGlyphRenderData( Vector3 translation, Vector2 scale, float rotation, Vector4 uV, Color16x4 color, long fontTextureHandle ) {
		this.Translation = translation;
		this.Scale = scale;
		this.Rotation = rotation;
		this.UV = uV;
		this.Color = color;
		this.FontTextureHandle = fontTextureHandle;
	}
}
