using Engine.Standard.Render.Text.Fonts.Tables.Hmtx;

namespace Engine.Standard.Render.Text.Fonts.Tables;

/// <summary>
/// <c>Horizontal metrics table</c><br/>
/// Contains information about the horizontal metrics of the font.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6hmtx.html">Horizontal metrics table documentation</see>
/// </summary>
public sealed class HmtxTable : FontTable {

	private HorizontalMetric[] _horizontalMetrics;

	public HmtxTable( FontTableHeader header, LocaTable locaTable, HheaTable hheaTable, FontDataReader reader ) : base( header ) {
		FontCaretedDataReader caret = new( reader );
		caret.GoTo( header.Offset );
		this._horizontalMetrics = new HorizontalMetric[ locaTable.GlyphLocationOffsetBytes.Count ];
		for (int i = 0; i < hheaTable.NumberOfHMetrics; i++) {
			ushort advanceWidth = caret.Read<ushort>();
			short leftSideBearing = caret.Read<short>();
			this._horizontalMetrics[ i ] = new HorizontalMetric( (uint) i, advanceWidth, leftSideBearing );
		}
		for (int i = hheaTable.NumberOfHMetrics; i < this._horizontalMetrics.Length; i++) {
			short leftSideBearing = caret.Read<short>();
			this._horizontalMetrics[ i ] = new HorizontalMetric( (uint) i, this._horizontalMetrics[ hheaTable.NumberOfHMetrics - 1 ].AdvanceWidth, leftSideBearing );
		}
	}

	public IReadOnlyList<HorizontalMetric> HorizontalMetrics => this._horizontalMetrics;
}
