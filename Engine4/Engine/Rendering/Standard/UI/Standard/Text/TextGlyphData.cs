using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.UI.Standard.Text;

[StructLayout( LayoutKind.Explicit )]
public readonly struct TextGlyphData {
	[FieldOffset( 0 )]
	public readonly Vector2 Translation;
	[FieldOffset( 8 )]
	public readonly Vector2 Scale;
	[FieldOffset( 16 )]
	public readonly Vector2 Rotation;
	[FieldOffset( 24 )]
	public readonly Vector4 UV;
	[FieldOffset( 40 )]
	public readonly Color16x4 Color;
	[FieldOffset( 48 )]
	public readonly Vector2 GlyphData;

	public TextGlyphData( Vector2 translation, Vector2 scale, float rotation, Vector4 uV, Color16x4 color, float thickness, float edge ) {
		this.Translation = translation;
		this.Scale = scale;
		this.Rotation = new Vector2( MathF.Cos( rotation ), MathF.Sin( rotation ) );
		this.UV = uV;
		this.Color = color;
		this.GlyphData = new( thickness, edge );
	}
}
