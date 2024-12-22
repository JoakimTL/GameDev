using Engine.Standard.Render.Text.Fonts.Tables.Glyf;

namespace Engine.Standard.Render.Text.Fonts;

//Turns glyph data into glyph data for mesh rendering (triangles with font rendering info)
public sealed class Font {

	private readonly DefinedGlyph?[] _glyphs;

	public Font( FontLoader loader ) {
		FontName = loader.FontName;
		UnitsPerEm = loader.HeadTable.UnitsPerEm;
		LineGap = loader.HheaTable.LineGap;
		ScalingFactor = 1f / UnitsPerEm;
		_glyphs = new DefinedGlyph?[ char.MaxValue + 1 ];
		foreach (IGlyphData glyphData in loader.GlyfTable.GlyphByUnicode.Values)
			CreateDefinedGlyph( glyphData, loader );
	}

	public string FontName { get; }
	public ushort UnitsPerEm { get; }
	/// <summary>
	/// The distance between the baselines of two consecutive lines of text. Use <see cref="ScaledLineGap"/> to get the scaled value in Ems.
	/// </summary>
	public short LineGap { get; }
	public float ScalingFactor { get; }
	public float ScaledLineGap => LineGap * ScalingFactor;

	private void CreateDefinedGlyph( IGlyphData glyphData, FontLoader loader ) {
		IGlyph glyph = CreateGlyph( glyphData, Matrix.Create2x2.Scaling( ScalingFactor, ScalingFactor ), Vector2<float>.Zero );

		int glyphIndex = (int) glyphData.Mapping.GlyphIndex;
		Tables.Hmtx.HorizontalMetric horizontalMetrics = loader.HmtxTable.HorizontalMetrics[ glyphIndex ];
		float advance = horizontalMetrics.AdvanceWidth * ScalingFactor;
		float leftSideBearing = horizontalMetrics.LeftSideBearing * ScalingFactor;

		_glyphs[ (char) glyphData.Mapping.Unicode ] = new( glyph, advance, leftSideBearing );
	}

	public DefinedGlyph? this[ char character ] => _glyphs[ character ];

	public static IGlyph CreateGlyph( IGlyphData glyph, Matrix2x2<float> transformationMatrix, Vector2<float> offset ) {
		if (glyph is ElementalGlyphData elementalData)
			return new FontElementalGlyph( elementalData, transformationMatrix, offset );
		if (glyph is CompoundGlyphData compoundData)
			return new FontCompoundGlyph( compoundData, transformationMatrix, offset );
		throw new Exception( "Unknown glyph component type" );
	}
}
