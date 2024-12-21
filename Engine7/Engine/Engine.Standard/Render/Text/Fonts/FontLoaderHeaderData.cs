namespace Engine.Standard.Render.Text.Fonts;

public sealed class FontLoaderHeaderData {

	private readonly FontOffsetSubtable _fontOffsetSubTable;
	private readonly Dictionary<uint, FontTableHeader> _tables = [];

	public FontLoaderHeaderData( FontDataReader dataReader ) {
		FontCaretedDataReader caret = new( dataReader );
		_fontOffsetSubTable = ReadFontOffsetSubtable( caret );
		for (int i = 0; i < this._fontOffsetSubTable.NumTables; i++) {
			FontTableHeader table = ReadFontTableHeader( caret );
			this._tables.Add( table.Tag, table );
		}
	}

	private static FontOffsetSubtable ReadFontOffsetSubtable( FontCaretedDataReader caret )
		=> new( caret.Read<uint>(), caret.Read<ushort>(), caret.Read<ushort>(), caret.Read<ushort>(), caret.Read<ushort>() );

	private static FontTableHeader ReadFontTableHeader( FontCaretedDataReader caret )
		=> new( caret.Read<uint>( false ), caret.Read<uint>(), caret.Read<uint>(), caret.Read<uint>() );

	public FontOffsetSubtable FontOffsetSubTable => this._fontOffsetSubTable;
	public IReadOnlyDictionary<uint, FontTableHeader> Tables => this._tables;
}
