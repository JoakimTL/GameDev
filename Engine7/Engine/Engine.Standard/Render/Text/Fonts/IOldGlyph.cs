namespace Engine.Standard.Render.Text.Fonts;

public interface IOldGlyph {
	FontGlyphHeader Header { get; }
	GlyphMap Mapping { get; }
	GlyphTriangle[] TriangulateGlyph();
}