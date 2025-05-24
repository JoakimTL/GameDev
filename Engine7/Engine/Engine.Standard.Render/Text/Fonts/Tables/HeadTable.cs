using Engine.Standard.Render.Text.Fonts.Tables.Head;

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
		caret.MoveCaretBy( 4 ); //version
		caret.MoveCaretBy( 4 ); //fontRevision
		caret.MoveCaretBy( 4 ); //checkSumAdjustment
		caret.MoveCaretBy( 4 ); //magicNumber
		this.Flags = (FontFlags) caret.Read<ushort>();
		this.UnitsPerEm = caret.Read<ushort>();
		caret.MoveCaretBy( 8 ); //created
		caret.MoveCaretBy( 8 ); //modified
		short minX = caret.Read<short>();
		short minY = caret.Read<short>();
		short maxX = caret.Read<short>();
		short maxY = caret.Read<short>();
		this.MacStyle = (MacStyle) caret.Read<ushort>();
		caret.MoveCaretBy( 2 ); //lowestRecPPEM
		this.FontDirectionHint = (FontDirectionHint) caret.Read<short>();
		this.BytesPerLocationLookup = caret.Read<ushort>() == 1 ? 4 : 2;
		caret.MoveCaretBy( 2 ); //glyphDataFormat
		this.Min = new( minX, minY );
		this.Max = new( maxX, maxY );
	}
}