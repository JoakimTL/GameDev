namespace Engine.Standard.Render.Text.Fonts.Tables.Glyf;

public interface IGlyphData {
	FontGlyphHeader Header { get; }
	GlyphMap Mapping { get; }
}
