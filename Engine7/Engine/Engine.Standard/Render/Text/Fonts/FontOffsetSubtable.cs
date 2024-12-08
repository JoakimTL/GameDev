namespace Engine.Standard.Render.Text.Fonts;

public readonly struct FontOffsetSubtable( uint scalerType, ushort numTables, ushort searchRange, ushort entrySelector, ushort rangeShift ) {
	//https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6.html
	public readonly uint ScalerType = scalerType;
	public readonly ushort NumTables = numTables;
	public readonly ushort SearchRange = searchRange;
	public readonly ushort EntrySelector = entrySelector;
	public readonly ushort RangeShift = rangeShift;
	public FontOffsetSubtable ProperEndianness => new( this.ScalerType.FromBigEndian(), this.NumTables.FromBigEndian(), this.SearchRange.FromBigEndian(), this.EntrySelector.FromBigEndian(), this.RangeShift.FromBigEndian() );
}
