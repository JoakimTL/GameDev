﻿using Engine.Logging;

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
		_glyphDataByIndex = [];
		_compoundGlyphReader = new( this );
	}

	public IGlyphData GetGlyphData( uint glyphIndex ) {
		if (_glyphDataByIndex.TryGetValue( glyphIndex, out IGlyphData? glyphData ))
			return glyphData;

		FontCaretedDataReader caret = new( _reader );
		caret.GoTo( _glyfTableHeader.Offset );
		caret.MoveCaretBy( (int) _locaTable.GlyphLocationOffsetBytes[ (int) glyphIndex ] );

		FontGlyphHeader header = new(caret.Read<short>(), caret.Read<short>(), caret.Read<short>(), caret.Read<short>(), caret.Read<short>() );
		if (!_cmapTable.GlyphMapByGlyphIndex.TryGetValue( glyphIndex, out GlyphMap glyphMap ))
			glyphMap = this.LogWarningThenReturn( $"Failed to find glyph mapping for glyph index {glyphIndex}!", new GlyphMap( glyphIndex, 65535 ) );

		glyphData = header.NumberOfContours < 0
			? _compoundGlyphReader.Read( header, glyphMap, caret )
			: ElementalGlyphReader.Read( header, glyphMap, caret );
		_glyphDataByIndex[ glyphIndex ] = glyphData;

		return glyphData;
	}

	public IEnumerable<IGlyphData> Glyphs => _glyphDataByIndex.Values;
}