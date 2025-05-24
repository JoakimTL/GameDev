using Engine.Standard.Render.Text.Fonts.Tables.Glyf;

namespace Engine.Standard.Render.Text.Fonts;

public sealed class FontCompoundGlyph : IGlyph {

	private readonly CompoundGlyphData _glyphData;
	private readonly List<IGlyph> _componentGlyphs;

	public FontCompoundGlyph( CompoundGlyphData glyphData, Matrix2x2<float> transformationMatrix, Vector2<float> offset ) {
		this._glyphData = glyphData;
		this._componentGlyphs = [];
		foreach (CompoundGlyphComponentData component in this._glyphData.ComponentGlyphData)
			this._componentGlyphs.Add( Font.CreateGlyph( component.Glyph, transformationMatrix * component.TransformationMatrix, offset + component.Offset ) );
	}

	public IGlyphData GlyphData => this._glyphData;

	public GlyphTriangle[] TriangulateGlyph() {
		List<GlyphTriangle> triangles = [];
		foreach (IGlyph component in this._componentGlyphs)
			triangles.AddRange( component.TriangulateGlyph() );
		return [ .. triangles ];
	}
}
