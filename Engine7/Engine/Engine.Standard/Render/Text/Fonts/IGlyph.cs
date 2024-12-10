using Engine.Shapes;

namespace Engine.Standard.Render.Text.Fonts;

public interface IGlyph {
	FontGlyphHeader Header { get; }
	GlyphMap Mapping { get; }
	GlyphTriangle[] TriangulateGlyph();
}
