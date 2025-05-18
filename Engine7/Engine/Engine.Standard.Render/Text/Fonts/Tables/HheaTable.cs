namespace Engine.Standard.Render.Text.Fonts.Tables;

/// <summary>
/// <c>Horizontal header table</c><br/>
/// Contains information about the horizontal layout of the font.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6hhea.html">Horizontal header table documentation</see>
public sealed class HheaTable : FontTable {

	public short Ascent { get; }
	public short Descent { get; }
	public short LineGap { get; }
	public ushort AdvanceWidthMax { get; }
	public short MinLeftSideBearing { get; }
	public short MinRightSideBearing { get; }
	public short XMaxExtent { get; }
	public short CaretSlopeRise { get; }
	public short CaretSlopeRun { get; }
	public short CaretOffset { get; }
	public short MetricDataFormat { get; }
	public ushort NumberOfHMetrics { get; }

	public HheaTable( FontTableHeader header, FontDataReader reader ) : base( header ) {
		FontCaretedDataReader caret = new( reader );
		caret.GoTo( header.Offset );
		caret.MoveCaretBy( sizeof( int ) ); //Skip version
		Ascent = caret.Read<short>();
		Descent = caret.Read<short>();
		LineGap = caret.Read<short>();
		AdvanceWidthMax = caret.Read<ushort>();
		MinLeftSideBearing = caret.Read<short>();
		MinRightSideBearing = caret.Read<short>();
		XMaxExtent = caret.Read<short>();
		CaretSlopeRise = caret.Read<short>();
		CaretSlopeRun = caret.Read<short>();
		CaretOffset = caret.Read<short>();
		caret.MoveCaretBy( 4 * sizeof( short ) ); //Skip 4 reserved shorts
		MetricDataFormat = caret.Read<short>();
		NumberOfHMetrics = caret.Read<ushort>();
	}
}
