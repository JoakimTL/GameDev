namespace Engine.Standard.Render.Text.Fonts.Tables.Glyf;

public sealed class GlyphReader {
	private readonly FontTableHeader _glyfTableHeader;
	private readonly LocaTable _locaTable;
	private readonly CmapTable _cmapTable;
	private readonly FontDataReader _reader;
	private readonly CompoundGlyphReader _compoundGlyphReader;

	private readonly Dictionary<uint, IGlyphData> _glyphDataByIndex;

	public GlyphReader( FontTableHeader glyfTableHeader, LocaTable locaTable, CmapTable cmapTable, FontDataReader reader ) {
		this._glyfTableHeader = glyfTableHeader;
		this._locaTable = locaTable;
		this._cmapTable = cmapTable;
		this._reader = reader;
		this._glyphDataByIndex = [];
		this._compoundGlyphReader = new( this );
	}

	public IGlyphData GetGlyphData( uint glyphIndex ) {
		if (this._glyphDataByIndex.TryGetValue( glyphIndex, out IGlyphData? glyphData ))
			return glyphData;

		FontCaretedDataReader caret = new( this._reader );
		caret.GoTo( this._glyfTableHeader.Offset );
		caret.MoveCaretBy( (int) this._locaTable.GlyphLocationOffsetBytes[ (int) glyphIndex ] );

		FontGlyphHeader header = new( caret.Read<short>(), caret.Read<short>(), caret.Read<short>(), caret.Read<short>(), caret.Read<short>() );
		if (!this._cmapTable.GlyphMapByGlyphIndex.TryGetValue( glyphIndex, out GlyphMap glyphMap ))
			glyphMap = new GlyphMap( glyphIndex, 65535 );

		glyphData = header.NumberOfContours < 0
			? this._compoundGlyphReader.Read( header, glyphMap, caret )
			: ElementalGlyphReader.Read( header, glyphMap, caret );
		this._glyphDataByIndex[ glyphIndex ] = glyphData;

		return glyphData;
	}

	public IEnumerable<IGlyphData> Glyphs => this._glyphDataByIndex.Values;
}
