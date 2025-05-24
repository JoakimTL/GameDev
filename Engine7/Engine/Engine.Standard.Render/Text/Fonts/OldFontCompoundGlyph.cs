namespace Engine.Standard.Render.Text.Fonts;

public sealed class OldFontCompoundGlyph( FontGlyphHeader header, GlyphMap mapping ) : IOldGlyph {
	public FontGlyphHeader Header { get; } = header;
	public GlyphMap Mapping { get; } = mapping;


	private readonly List<IOldGlyph> _glyphs = [];

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
