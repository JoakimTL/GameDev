namespace Engine.Standard.Render.Text.Fonts.Tables.Hmtx;

public readonly struct HorizontalMetric( uint glyphIndex, ushort advanceWidth, short leftSideBearing ) {
	public uint GlyphIndex { get; } = glyphIndex;
	public ushort AdvanceWidth { get; } = advanceWidth;
	public short LeftSideBearing { get; } = leftSideBearing;
	public override string ToString() => $"{GlyphIndex}: AW {AdvanceWidth}, LSB {LeftSideBearing}";
}