using Engine.Standard.Render.Text.Fonts.Tables.Glyf;

namespace Engine.Standard.Render.Text.Fonts;

public sealed class FontCompoundGlyph : IGlyph {

	private readonly CompoundGlyphData _glyphData;
	private readonly List<IGlyph> _componentGlyphs;

	public FontCompoundGlyph( CompoundGlyphData glyphData, Matrix2x2<float> transformationMatrix, Vector2<float> offset ) {
		_glyphData = glyphData;
		_componentGlyphs = [];
		foreach (CompoundGlyphComponentData component in _glyphData.ComponentGlyphData)
			_componentGlyphs.Add( Font.CreateGlyph( component.Glyph, transformationMatrix * component.TransformationMatrix, offset + component.Offset ) );
	}

	public IGlyphData GlyphData => _glyphData;

	public GlyphTriangle[] TriangulateGlyph() {
		List<GlyphTriangle> triangles = [];
		foreach (IGlyph component in _componentGlyphs)
			triangles.AddRange( component.TriangulateGlyph() );
		return [ .. triangles ];
	}
}
