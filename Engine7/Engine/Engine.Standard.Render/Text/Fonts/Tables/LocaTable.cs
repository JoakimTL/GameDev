namespace Engine.Standard.Render.Text.Fonts.Tables;

/// <summary>
/// <c>Glyph location table</c><br/>
/// Contains information about the location of each glyph in the font.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6loca.html">Glyph location table documentation</see>
/// </summary>
public sealed class LocaTable : FontTable {

	private readonly uint[] _glyphLocationOffsetBytes;

	public LocaTable( FontTableHeader header, HeadTable headTable, MaxPTable maxPTable, FontDataReader reader ) : base( header ) {
		FontCaretedDataReader caret = new( reader );
		caret.GoTo( header.Offset );
		int numGlyphs = maxPTable.MaxGlyphs;
		_glyphLocationOffsetBytes = new uint[ numGlyphs ];
		bool isTwoByteEntry = headTable.BytesPerLocationLookup == 2;
		for (int glyphIndex = 0; glyphIndex < numGlyphs; glyphIndex++) {
			// If 2-byte format is used, the stored location is half of actual location (so multiply by 2)
			uint glyphDataOffset = isTwoByteEntry ? caret.Read<ushort>() * 2u : caret.Read<uint>();
			_glyphLocationOffsetBytes[ glyphIndex ] = glyphDataOffset;
		}
	}

	/// <summary>
	/// The offsets within the glyf table. Remember to start from the glyf table when applying the offset!
	/// </summary>
	public IReadOnlyList<uint> GlyphLocationOffsetBytes => _glyphLocationOffsetBytes;

}
