namespace Engine.Standard.Render.Text.Fonts;

public sealed class DefinedGlyph( IGlyph glyph, float advance, float leftSideBearing ) {
	public IGlyph Glyph { get; } = glyph;
	public float Advance { get; } = advance;
	public float LeftSideBearing { get; } = leftSideBearing;
}
