using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.UI.Standard.Text;

[StructLayout( LayoutKind.Explicit, Pack = 1)]
public readonly struct TextGlyphRenderData {
	[FieldOffset( 0 )]
	public readonly Matrix4x4 TransformationMatrix;
	[FieldOffset( 64 )]
	public readonly ulong FontTextureHandle;
	[FieldOffset( 72 )]
	public readonly TextGlyphData GlyphData;

	public TextGlyphRenderData( Matrix4x4 transformationMatrix, TextGlyphData glyphData, ulong fontTextureHandle ) {
		this.TransformationMatrix = transformationMatrix;
		this.GlyphData = glyphData;
		this.FontTextureHandle = fontTextureHandle;
	}
}
