namespace Engine.Standard.Render.Text.Fonts;

public sealed class OldFontCompoundGlyph : IOldGlyph {
	public FontGlyphHeader Header { get; }
	public GlyphMap Mapping { get; }


	private readonly List<IOldGlyph> _glyphs;

	public OldFontCompoundGlyph( FontGlyphHeader header, GlyphMap mapping ) {
		this._glyphs = [];
		this.Header = header;
		this.Mapping = mapping;
	}

	public void AddGlyph( IOldGlyph glyph ) {
		this._glyphs.Add( glyph );
	}

	public GlyphTriangle[] TriangulateGlyph() {
		List<GlyphTriangle> result = [];
		foreach (IOldGlyph glyph in this._glyphs)
			result.AddRange( glyph.TriangulateGlyph() );
		return [ .. result ];
	}
}
