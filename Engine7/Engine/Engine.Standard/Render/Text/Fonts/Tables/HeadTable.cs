namespace Engine.Standard.Render.Text.Fonts.Tables;

/// <summary>
/// <c>Font header table</c><br/>
/// Contains general information about the font, such as <see cref="UnitsPerEm"/><br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6head.html">Font header table documentation</see>
/// </summary>
public sealed class HeadTable : FontTable {

	public FontFlags Flags { get; }
	public ushort UnitsPerEm { get; }
	public Vector2<short> Min { get; }
	public Vector2<short> Max { get; }
	public MacStyle MacStyle { get; }
	public FontDirectionHint FontDirectionHint { get; }
	public int BytesPerLocationLookup { get; }

	public HeadTable( FontTableHeader header, FontDataReader dataReader ) : base( header ) {
		FontCaretedDataReader caret = new( dataReader );
		caret.GoTo( header.Offset );
		caret.SkipBytes( 4 ); //version
		caret.SkipBytes( 4 ); //fontRevision
		caret.SkipBytes( 4 ); //checkSumAdjustment
		caret.SkipBytes( 4 ); //magicNumber
		Flags = (FontFlags) caret.Read<ushort>();
		UnitsPerEm = caret.Read<ushort>();
		caret.SkipBytes( 4 ); //created
		caret.SkipBytes( 4 ); //modified
		short minX = caret.Read<short>();
		short minY = caret.Read<short>();
		short maxX = caret.Read<short>();
		short maxY = caret.Read<short>();
		MacStyle = (MacStyle) caret.Read<ushort>();
		caret.SkipBytes( 2 ); //lowestRecPPEM
		FontDirectionHint = (FontDirectionHint) caret.Read<short>();
		BytesPerLocationLookup = caret.Read<short>() == 1 ? 4 : 2;
		caret.SkipBytes( 2 ); //glyphDataFormat
		Min = new( minX, minY );
		Max = new( maxX, maxY );
	}
}