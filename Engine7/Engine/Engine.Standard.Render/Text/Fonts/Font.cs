using Engine.Standard.Render.Text.Fonts.Tables.Glyf;

namespace Engine.Standard.Render.Text.Fonts;

//Turns glyph data into glyph data for mesh rendering (triangles with font rendering info)
public sealed class Font {

	private readonly DefinedGlyph?[] _glyphs;

	public Font( FontLoader loader ) {
		this.FontName = loader.FontName;
		this.UnitsPerEm = loader.HeadTable.UnitsPerEm;
		this.LineGap = loader.HheaTable.LineGap;
		this.Ascent = loader.HheaTable.Ascent;
		this.Descent = loader.HheaTable.Descent;
		this.ScalingFactor = 1f / this.UnitsPerEm;
		this._glyphs = new DefinedGlyph?[ char.MaxValue + 1 ];
		foreach (IGlyphData glyphData in loader.GlyfTable.GlyphByUnicode.Values)
			CreateDefinedGlyph( glyphData, loader );
	}

	public string FontName { get; }
	public ushort UnitsPerEm { get; }
	public short LineGap { get; }
	public short Ascent { get; }
	public short Descent { get; }
	public float GlyphHeight => this.Ascent - this.Descent;
	public float ScalingFactor { get; }
	public float ScaledAscent => this.Ascent * this.ScalingFactor;
	public float ScaledDescent => this.Descent * this.ScalingFactor;
	public float ScaledLineGap => this.LineGap * this.ScalingFactor;

	/// <summary>
	/// The maximum height of a glyph in the font, scaled to the font's units per em.<br/>
	/// Found by subtracting the descent from the ascent and then scaling the result.
	/// </summary>
	public float ScaledGlyphHeight => this.GlyphHeight * this.ScalingFactor;
	/// <summary>
	/// The height of a line of text in the font, scaled to the font's units per em.<br/>
	/// Found by adding the glyph height to the line gap and then scaling the result.
	/// </summary>
	public float ScaledLineHeight => (this.GlyphHeight + this.LineGap) * this.ScalingFactor;

	private void CreateDefinedGlyph( IGlyphData glyphData, FontLoader loader ) {
		IGlyph glyph = CreateGlyph( glyphData, Matrix.Create2x2.Scaling( this.ScalingFactor, this.ScalingFactor ), Vector2<float>.Zero );

		int glyphIndex = (int) glyphData.Mapping.GlyphIndex;
		Tables.Hmtx.HorizontalMetric horizontalMetrics = loader.HmtxTable.HorizontalMetrics[ glyphIndex ];
		float advance = horizontalMetrics.AdvanceWidth * this.ScalingFactor;
		float leftSideBearing = horizontalMetrics.LeftSideBearing * this.ScalingFactor;

		this._glyphs[ (char) glyphData.Mapping.Unicode ] = new( glyph, advance, leftSideBearing );
	}

	public DefinedGlyph? this[ char character ] => this._glyphs[ character ];

	public static IGlyph CreateGlyph( IGlyphData glyph, Matrix2x2<float> transformationMatrix, Vector2<float> offset ) {
		if (glyph is ElementalGlyphData elementalData)
			return new FontElementalGlyph( elementalData, transformationMatrix, offset );
		if (glyph is CompoundGlyphData compoundData)
			return new FontCompoundGlyph( compoundData, transformationMatrix, offset );
		throw new Exception( "Unknown glyph component type" );
	}
}
