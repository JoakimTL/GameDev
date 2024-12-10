using Engine.Shapes;

namespace Engine.Standard.Render.Text.Fonts;

public sealed class FontCompoundGlyph : IGlyph {
	public FontGlyphHeader Header { get; }
	public GlyphMap Mapping { get; }


	private readonly List<IGlyph> _glyphs;

	public FontCompoundGlyph( FontGlyphHeader header, GlyphMap mapping ) {
		this._glyphs = [];
		this.Header = header;
		this.Mapping = mapping;
	}

	public void AddGlyph( IGlyph glyph ) {
		this._glyphs.Add( glyph );
	}

	public GlyphTriangle[] TriangulateGlyph() {
		List<GlyphTriangle> result = [];
		foreach (IGlyph glyph in this._glyphs)
			result.AddRange( glyph.TriangulateGlyph() );
		return result.ToArray();
	}
}
