namespace Engine.Standard.Render.Text;

public sealed class Font {
	public const uint Tag_Cmap = 1885433187;
	public const uint Tag_Glyf = 1719233639;
	public const uint Tag_Head = 1684104552;
	public const uint Tag_Loca = 1633906540;
	public const uint Tag_Maxp = 1886937453;
	public const uint Tag_Hhea = 1634035816;
	public const uint Tag_Hmtx = 2020896104;

	public string Path { get; }
	private ushort _unitsPerEm;
	private ushort _numBytesPerLocationLookup;
	private FontOffsetSubtable _fontOffsetSubtable;
	private Dictionary<uint, FontTable> _tables = [];
	private Dictionary<char, FontGlyph> _glyphs = [];

	internal unsafe Font( string path ) {
		// Load font
		this.Path = path;
		byte[] fontData = File.ReadAllBytes( path );

		fixed (byte* ptr = fontData) {
			ReadFontData( ptr, fontData.Length );
		}

	}

	public FontGlyph this[ char c ] => _glyphs[ c ];

	private unsafe void ReadFontData( byte* srcPtr, int length ) {
		nint offset = 0;
		_fontOffsetSubtable = FontUtilities.Read<FontOffsetSubtable>( srcPtr, ref offset ).ProperEndianness;

		for (int i = 0; i < _fontOffsetSubtable.NumTables; i++) {
			FontTable table = FontUtilities.Read<FontTable>( srcPtr, ref offset ).ProperEndianness;
			_tables.Add( table.Tag, table );
		}

		FontTable headTable = _tables[ Tag_Head ];
		FontTable maxpTable = _tables[ Tag_Maxp ];
		FontTable locaTable = _tables[ Tag_Loca ];
		FontTable glyfTable = _tables[ Tag_Glyf ];

		//Head table
		offset = (nint) headTable.Offset + 18;
		_unitsPerEm = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();
		offset += 30;
		ushort numBytesPerLocationLookup = (ushort) (FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian() == 0 ? 2u : 4u);

		//Maxp table
		offset = (nint) maxpTable.Offset + 4;
		ushort numGlyphs = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();
		Span<uint> glyphLocations = stackalloc uint[ numGlyphs ];
		GetAllGlyphLocations( srcPtr, glyphLocations, numBytesPerLocationLookup, locaTable.Offset, glyfTable.Offset );

		GlyphMap[] mappings = GetUnicodeToGlyphIndexMappings( srcPtr, _tables[ Tag_Cmap ] );

		for (int i = 0; i < mappings.Length; i++) {
			var glyph = ReadGlyph( srcPtr, glyphLocations, mappings[ i ] );
			if (glyph is not null)
				_glyphs.Add( (char) glyph.Unicode, glyph );
		}

	}
	//https://www.youtube.com/watch?v=SO83KQuuZvg

	private static unsafe FontGlyph ReadGlyph( byte* srcPtr, Span<uint> glyphLocations, GlyphMap mapping ) {
		nint loc = (nint) glyphLocations[ (int) mapping.GlyphIndex ];

		FontGlyphHeader header = FontUtilities.Read<FontGlyphHeader>( srcPtr, ref loc ).ProperEndianness;

		bool compoundGlyph = header.NumberOfContours < 0;

		return compoundGlyph
			? null//ReadCompoundGlyph( srcPtr, loc, header, mapping )
			: ReadSingleGlyph( srcPtr, loc, header, mapping );

	}

	private static unsafe FontGlyph ReadSingleGlyph( byte* srcPtr, nint offset, FontGlyphHeader header, GlyphMap mapping ) {
		var endPointsOfContours = new ushort[ header.NumberOfContours ];
		for (int i = 0; i < header.NumberOfContours; i++)
			endPointsOfContours[ i ] = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();

		int instructionLength = FontUtilities.Read<short>( srcPtr, ref offset ).FromBigEndian();
		var instructions = new byte[ instructionLength ];
		for (int i = 0; i < instructionLength; i++)
			instructions[ i ] = FontUtilities.Read<byte>( srcPtr, ref offset );
		int numPoints = endPointsOfContours[ ^1 ] + 1;
		var flags = new byte[ numPoints ];
		for (int i = 0; i < numPoints; i++) {
			byte flag = FontUtilities.Read<byte>( srcPtr, ref offset );
			flags[ i ] = flag;
			if (ReadFlagBit( flag, 3 )) {
				byte repeat = FontUtilities.Read<byte>( srcPtr, ref offset );
				for (int j = 0; j < repeat; j++) {
					i++;
					flags[ i ] = flag;
				}
			}
		}

		int[] xCoordinates = ReadCoordinates( srcPtr, ref offset, flags, true );
		int[] yCoordinates = ReadCoordinates( srcPtr, ref offset, flags, false );
		var points = new (Vector2<int> coordinate, bool onCurve)[ xCoordinates.Length ];
		for (int i = 0; i < xCoordinates.Length; i++)
			points[ i ] = (new Vector2<int>( xCoordinates[ i ], yCoordinates[ i ] ), ReadFlagBit( flags[ i ], 0 ));

		return new FontGlyph( header, mapping, points, endPointsOfContours, instructions, flags );

		int[] ReadCoordinates( byte* srcPtr, ref nint offset, byte[] flags, bool readingX ) {
			int offsetSizeFlagBit = readingX ? 1 : 2;
			int offsetSignOrSkipBit = readingX ? 4 : 5;
			int[] coordinates = new int[ flags.Length ];
			for (int i = 0; i < flags.Length; i++) {
				int currentCoordinate = coordinates[ Math.Max( 0, i - 1 ) ];
				byte flag = flags[ i ];

				if (ReadFlagBit( flag, offsetSizeFlagBit )) {
					currentCoordinate += FontUtilities.Read<byte>( srcPtr, ref offset ) * (ReadFlagBit( flag, offsetSignOrSkipBit ) ? 1 : -1);
				} else if (!ReadFlagBit( flag, offsetSignOrSkipBit )) {
					currentCoordinate += FontUtilities.Read<short>( srcPtr, ref offset ).FromBigEndian();
				}
				coordinates[ i ] = currentCoordinate;
			}
			return coordinates;
		}
	}

	//private static unsafe FontGlyph ReadCompoundGlyph( byte* srcPtr, nint offset, FontGlyphHeader header, GlyphMap mapping ) {
	//	FontData.GlyphData compoundGlyph = new();
	//	compoundGlyph.GlyphIndex = glyphIndex;

	//	uint glyphLocation = glyphLocations[ glyphIndex ];
	//	reader.GoTo( glyphLocation );
	//	reader.SkipBytes( 2 );

	//	compoundGlyph.MinX = reader.ReadInt16();
	//	compoundGlyph.MinY = reader.ReadInt16();
	//	compoundGlyph.MaxX = reader.ReadInt16();
	//	compoundGlyph.MaxY = reader.ReadInt16();

	//	List<FontData.Point> allPoints = new();
	//	List<int> allContourEndIndices = new();

	//	while (true) {
	//		(FontData.GlyphData componentGlyph, bool hasMoreGlyphs) = ReadNextComponentGlyph( reader, glyphLocations, glyphLocation );

	//		// Add all contour end indices from the simple glyph component to the compound glyph's data
	//		// Note: indices must be offset to account for previously-added component glyphs
	//		foreach (int endIndex in componentGlyph.ContourEndIndices) {
	//			allContourEndIndices.Add( endIndex + allPoints.Count );
	//		}
	//		allPoints.AddRange( componentGlyph.Points );

	//		if (!hasMoreGlyphs)
	//			break;
	//	}

	//	compoundGlyph.Points = allPoints.ToArray();
	//	compoundGlyph.ContourEndIndices = allContourEndIndices.ToArray();
	//	return compoundGlyph;

	//	return new FontGlyph( header, mapping, points, endPointsOfContours, instructions, flags );
	//}

	private static bool ReadFlagBit( byte flag, int bit ) => ((flag >> bit) & 1) == 1;

	private static unsafe void GetAllGlyphLocations( byte* srcPtr, Span<uint> glyphLocations, ushort numBytesPerLocationLookup, uint locaTableLocation, uint glyfTableLocation ) {
		bool isTwoByteEntry = numBytesPerLocationLookup == 2;
		for (int glyphIndex = 0; glyphIndex < glyphLocations.Length; glyphIndex++) {
			nint offset = (nint) (locaTableLocation + glyphIndex * numBytesPerLocationLookup);
			// If 2-byte format is used, the stored location is half of actual location (so multiply by 2)
			uint glyphDataOffset = isTwoByteEntry ? FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian() * 2u : FontUtilities.Read<uint>( srcPtr, ref offset ).FromBigEndian();
			glyphLocations[ glyphIndex ] = glyfTableLocation + glyphDataOffset;
		}
	}

	private static unsafe GlyphMap[] GetUnicodeToGlyphIndexMappings( byte* srcPtr, in FontTable cmap ) {
		List<GlyphMap> glyphMaps = [];
		nint offset = (nint) cmap.Offset;

		ushort version = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();
		ushort numSubtables = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();

		// --- Read through metadata for each character map to find the one we want to use ---
		uint cmapSubtableOffset = 0;
		int selectedUnicodeVersionID = -1;

		for (int i = 0; i < numSubtables; i++) {
			ushort platformID = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();
			ushort platformSpecificID = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();
			uint subtableOffset = FontUtilities.Read<uint>( srcPtr, ref offset ).FromBigEndian();
			// Unicode encoding
			if (platformID == 0) {
				// Use highest supported unicode version
				if (platformSpecificID is 0 or 1 or 3 or 4 && platformSpecificID > selectedUnicodeVersionID) {
					cmapSubtableOffset = subtableOffset;
					selectedUnicodeVersionID = platformSpecificID;
				}
			}
			// Microsoft Encoding
			else if (platformID == 3 && selectedUnicodeVersionID == -1) {
				if (platformSpecificID is 1 or 10) {
					cmapSubtableOffset = subtableOffset;
				}
			}
		}

		if (cmapSubtableOffset == 0)
			throw new Exception( "Font does not contain supported character map type (TODO)" );

		// Go to the character map
		offset = (nint) (cmap.Offset + cmapSubtableOffset);
		ushort format = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();
		bool hasReadMissingCharGlyph = false;

		if (format != 12 && format != 4)
			throw new Exception( "Font cmap format not supported (TODO): " + format );

		// ---- Parse Format 4 ----
		if (format == 4) {
			ushort length = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();
			ushort languageCode = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();
			// Number of contiguous segments of character codes
			ushort segCount2X = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();
			ushort segCount = (ushort) (segCount2X / 2);
			offset += 6; // Skip: searchRange, entrySelector, rangeShift
						 // Ending character code for each segment (last = 2^16 - 1)
			ushort[] endCodes = new ushort[ segCount ];
			for (int i = 0; i < segCount; i++)
				endCodes[ i ] = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();

			// Skip reservedPad
			offset += 2;

			// Starting character code for each segment
			ushort[] startCodes = new ushort[ segCount ];
			for (int i = 0; i < segCount; i++)
				startCodes[ i ] = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();

			ushort[] idDeltas = new ushort[ segCount ];
			for (int i = 0; i < segCount; i++)
				idDeltas[ i ] = FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian();

			(ushort offset, uint readLoc)[] idRangeOffset = new (ushort, uint)[ segCount ];
			for (int i = 0; i < segCount; i++) {
				uint readLoc = (uint) offset;
				idRangeOffset[ i ] = (FontUtilities.Read<ushort>( srcPtr, ref offset ).FromBigEndian(), readLoc);
			}

			for (int i = 0; i < startCodes.Length; i++) {
				ushort currentCode = startCodes[ i ];
				ushort endCode = endCodes[ i ];

				if (currentCode == 0xFFFF)
					break;

				while (currentCode <= endCode) {
					uint glyphIndex;
					if (idRangeOffset[ i ].offset == 0) {
						glyphIndex = (uint) ((currentCode + idDeltas[ i ]) % 65536);
					} else {
						nint currentOffset = offset;
						uint rangeOffsetLocation = idRangeOffset[ i ].readLoc + idRangeOffset[ i ].offset;
						nint glyphIndexArrayLocation = (nint) (2u * (currentCode - startCodes[ i ]) + rangeOffsetLocation);

						glyphIndex = FontUtilities.Read<ushort>( srcPtr, ref glyphIndexArrayLocation ).FromBigEndian();
						if (glyphIndex != 0) {
							glyphIndex += idDeltas[ i ];
						}
						offset = currentOffset;
					}
					glyphMaps.Add( new GlyphMap( glyphIndex, currentCode ) );
					hasReadMissingCharGlyph |= glyphIndex == 0;
					currentCode++;
				}
			}
		} else if (format == 12) {
			offset += 10; // Skip: reserved, subtableByteLengthInlcudingHeader, languageCode
			uint numGroups = FontUtilities.Read<uint>( srcPtr, ref offset ).FromBigEndian();

			for (uint i = 0; i < numGroups; i++) {
				uint startCharCode = FontUtilities.Read<uint>( srcPtr, ref offset ).FromBigEndian();
				uint endCharCode = FontUtilities.Read<uint>( srcPtr, ref offset ).FromBigEndian();
				uint startGlyphIndex = FontUtilities.Read<uint>( srcPtr, ref offset ).FromBigEndian();

				uint numChars = endCharCode - startCharCode + 1;
				for (uint charCodeOffset = 0; charCodeOffset < numChars; charCodeOffset++) {
					uint charCode = startCharCode + charCodeOffset;
					uint glyphIndex = startGlyphIndex + charCodeOffset;

					glyphMaps.Add( new GlyphMap( glyphIndex, charCode ) );
					hasReadMissingCharGlyph |= glyphIndex == 0;
				}
			}
		}

		if (!hasReadMissingCharGlyph)
			glyphMaps.Add( new( 0, 65535 ) );

		return glyphMaps.ToArray();
	}
}
