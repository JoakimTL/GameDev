namespace Engine.Standard.Render.Text.Fonts.Tables.Cmap;

public sealed class CharacterMapFormat4() : CharacterMapFormatBase( 4 ) {
	public override List<GlyphMap> ReadFrom( FontCaretedDataReader caret ) {
		List<GlyphMap> glyphMaps = [];
		bool hasReadMissingCharGlyph = false;

		ushort length = caret.Read<ushort>();
		ushort languageCode = caret.Read<ushort>();
		// Number of contiguous segments of character codes
		ushort segCount2X = caret.Read<ushort>();
		ushort segCount = (ushort) (segCount2X / 2);

		// Skip searchRange, entrySelector, rangeShift
		caret.MoveCaretBy( 6 );

		// Ending character code for each segment (last = 2^16 - 1)
		ushort[] endCodes = new ushort[ segCount ];
		for (int i = 0; i < segCount; i++)
			endCodes[ i ] = caret.Read<ushort>();

		// Skip reservedPad
		caret.MoveCaretBy( 2 );

		// Starting character code for each segment
		ushort[] startCodes = new ushort[ segCount ];
		for (int i = 0; i < segCount; i++)
			startCodes[ i ] = caret.Read<ushort>();

		ushort[] idDeltas = new ushort[ segCount ];
		for (int i = 0; i < segCount; i++)
			idDeltas[ i ] = caret.Read<ushort>();

		(ushort offset, uint readLoc)[] idRangeOffset = new (ushort, uint)[ segCount ];
		for (int i = 0; i < segCount; i++) {
			uint readLoc = caret.CurrentOffset;
			idRangeOffset[ i ] = (caret.Read<ushort>(), readLoc);
		}

		for (int i = 0; i < startCodes.Length; i++) {
			ushort currentCode = startCodes[ i ];
			ushort endCode = endCodes[ i ];

			if (currentCode == 0xFFFF)
				break;

			while (currentCode <= endCode) {
				uint glyphIndex;
				if (idRangeOffset[ i ].offset == 0)
					glyphIndex = (uint) ((currentCode + idDeltas[ i ]) % 65536);
				else {
					uint currentOffset = caret.CurrentOffset;
					uint rangeOffsetLocation = idRangeOffset[ i ].readLoc + idRangeOffset[ i ].offset;
					uint glyphIndexArrayLocation = (uint) ((2u * (currentCode - startCodes[ i ])) + rangeOffsetLocation);

					caret.GoTo( glyphIndexArrayLocation );
					glyphIndex = caret.Read<ushort>();
					if (glyphIndex != 0)
						glyphIndex += idDeltas[ i ];
					caret.GoTo( currentOffset );
				}
				glyphMaps.Add( new GlyphMap( glyphIndex, currentCode ) );
				hasReadMissingCharGlyph |= glyphIndex == 0;
				currentCode++;
			}
		}

		if (!hasReadMissingCharGlyph)
			glyphMaps.Add( new GlyphMap( 0, 0xFFFD ) );
		return glyphMaps;
	}
}
