namespace Engine.Standard.Render.Text.Fonts.Tables.Cmap;

public sealed class CharacterMapFormat12() : CharacterMapFormatBase( 12 ) {
	public override List<GlyphMap> ReadFrom( FontCaretedDataReader caret ) {
		List<GlyphMap> glyphMaps = [];
		bool hasReadMissingCharGlyph = false;
		caret.MoveCaretBy( 10 );
		//offset += 10; // Skip: reserved, subtableByteLengthInlcudingHeader, languageCode
		uint numGroups = caret.Read<uint>();

		for (uint i = 0; i < numGroups; i++) {
			uint startCharCode = caret.Read<uint>();
			uint endCharCode = caret.Read<uint>();
			uint startGlyphIndex = caret.Read<uint>();

			uint numChars = endCharCode - startCharCode + 1;
			for (uint charCodeOffset = 0; charCodeOffset < numChars; charCodeOffset++) {
				uint charCode = startCharCode + charCodeOffset;
				uint glyphIndex = startGlyphIndex + charCodeOffset;

				glyphMaps.Add( new GlyphMap( glyphIndex, charCode ) );
				hasReadMissingCharGlyph |= glyphIndex == 0;
			}
		}

		if (!hasReadMissingCharGlyph)
			glyphMaps.Add( new GlyphMap( 0, 0xFFFD ) );
		return glyphMaps;
	}
}
