using Engine.Standard.Render.Text.Fonts.Tables.Glyf;

namespace Engine.Standard.Render.Text.Fonts;

public interface IGlyph {
	IGlyphData GlyphData { get; }
	GlyphTriangle[] TriangulateGlyph();
}