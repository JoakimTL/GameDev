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

	public (Triangle2<float>, bool filled, bool flipped)[] CreateMeshTriangles( float scale, bool useConstraints ) {
		List<(Triangle2<float>, bool filled, bool flipped)> result = [];
		foreach (IGlyph glyph in this._glyphs)
			result.AddRange( glyph.CreateMeshTriangles( scale, useConstraints ) );
		return result.ToArray();
	}

	public (Vector2<float>, uint indexInContour, bool onCurve)[] GetPointsInContours() {
		List<(Vector2<float>, uint, bool)> result = [];
		foreach (IGlyph glyph in this._glyphs)
			result.AddRange( glyph.GetPointsInContours() );
		return result.ToArray();
	}
}
