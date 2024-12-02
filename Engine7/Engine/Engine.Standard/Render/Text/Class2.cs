using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Standard.Render.Text;
internal class Class2 {
}
//static GlyphMap[] GetUnicodeToGlyphIndexMappings( FontReader reader, uint cmapOffset ) {
//	List<GlyphMap> glyphPairs = new();
//	reader.GoTo( cmapOffset );

//	uint version = reader.ReadUInt16();
//	uint numSubtables = reader.ReadUInt16(); // font can contain multiple character maps for different platforms

//	// --- Read through metadata for each character map to find the one we want to use ---
//	uint cmapSubtableOffset = 0;
//	int selectedUnicodeVersionID = -1;

//	for (int i = 0; i < numSubtables; i++) {
//		int platformID = reader.ReadUInt16();
//		int platformSpecificID = reader.ReadUInt16();
//		uint offset = reader.ReadUInt32();

//		// Unicode encoding
//		if (platformID == 0) {
//			// Use highest supported unicode version
//			if (platformSpecificID is 0 or 1 or 3 or 4 && platformSpecificID > selectedUnicodeVersionID) {
//				cmapSubtableOffset = offset;
//				selectedUnicodeVersionID = platformSpecificID;
//			}
//		}
//		// Microsoft Encoding
//		else if (platformID == 3 && selectedUnicodeVersionID == -1) {
//			if (platformSpecificID is 1 or 10) {
//				cmapSubtableOffset = offset;
//			}
//		}
//	}

//	if (cmapSubtableOffset == 0) {
//		throw new Exception( "Font does not contain supported character map type (TODO)" );
//	}

//	// Go to the character map
//	reader.GoTo( cmapOffset + cmapSubtableOffset );
//	int format = reader.ReadUInt16();
//	bool hasReadMissingCharGlyph = false;

//	if (format != 12 && format != 4) {
//		throw new Exception( "Font cmap format not supported (TODO): " + format );
//	}

//	// ---- Parse Format 4 ----
//	if (format == 4) {
//		int length = reader.ReadUInt16();
//		int languageCode = reader.ReadUInt16();
//		// Number of contiguous segments of character codes
//		int segCount2X = reader.ReadUInt16();
//		int segCount = segCount2X / 2;
//		reader.SkipBytes( 6 ); // Skip: searchRange, entrySelector, rangeShift

//		// Ending character code for each segment (last = 2^16 - 1)
//		int[] endCodes = new int[ segCount ];
//		for (int i = 0; i < segCount; i++) {
//			endCodes[ i ] = reader.ReadUInt16();
//		}

//		reader.Skip16BitEntries( 1 ); // Reserved pad

//		int[] startCodes = new int[ segCount ];
//		for (int i = 0; i < segCount; i++) {
//			startCodes[ i ] = reader.ReadUInt16();
//		}

//		int[] idDeltas = new int[ segCount ];
//		for (int i = 0; i < segCount; i++) {
//			idDeltas[ i ] = reader.ReadUInt16();
//		}

//		(int offset, int readLoc)[] idRangeOffsets = new (int, int)[ segCount ];
//		for (int i = 0; i < segCount; i++) {
//			int readLoc = (int) reader.GetLocation();
//			int offset = reader.ReadUInt16();
//			idRangeOffsets[ i ] = (offset, readLoc);
//		}

//		for (int i = 0; i < startCodes.Length; i++) {
//			int endCode = endCodes[ i ];
//			int currCode = startCodes[ i ];

//			if (currCode == 65535)
//				break; // not sure about this (hack to avoid out of bounds on a specific font)

//			while (currCode <= endCode) {
//				int glyphIndex;
//				// If idRangeOffset is 0, the glyph index can be calculated directly
//				if (idRangeOffsets[ i ].offset == 0) {
//					glyphIndex = (currCode + idDeltas[ i ]) % 65536;
//				}
//				// Otherwise, glyph index needs to be looked up from an array
//				else {
//					uint readerLocationOld = reader.GetLocation();
//					int rangeOffsetLocation = idRangeOffsets[ i ].readLoc + idRangeOffsets[ i ].offset;
//					int glyphIndexArrayLocation = 2 * (currCode - startCodes[ i ]) + rangeOffsetLocation;

//					reader.GoTo( glyphIndexArrayLocation );
//					glyphIndex = reader.ReadUInt16();

//					if (glyphIndex != 0) {
//						glyphIndex = (glyphIndex + idDeltas[ i ]) % 65536;
//					}

//					reader.GoTo( readerLocationOld );
//				}

//				glyphPairs.Add( new( (uint) glyphIndex, (uint) currCode ) );
//				hasReadMissingCharGlyph |= glyphIndex == 0;
//				currCode++;
//			}
//		}
//	}
//	// ---- Parse Format 12 ----
//	else if (format == 12) {
//		reader.SkipBytes( 10 ); // Skip: reserved, subtableByteLengthInlcudingHeader, languageCode
//		uint numGroups = reader.ReadUInt32();

//		for (int i = 0; i < numGroups; i++) {
//			uint startCharCode = reader.ReadUInt32();
//			uint endCharCode = reader.ReadUInt32();
//			uint startGlyphIndex = reader.ReadUInt32();

//			uint numChars = endCharCode - startCharCode + 1;
//			for (int charCodeOffset = 0; charCodeOffset < numChars; charCodeOffset++) {
//				uint charCode = (uint) (startCharCode + charCodeOffset);
//				uint glyphIndex = (uint) (startGlyphIndex + charCodeOffset);

//				glyphPairs.Add( new( glyphIndex, charCode ) );
//				hasReadMissingCharGlyph |= glyphIndex == 0;
//			}
//		}
//	}

//	if (!hasReadMissingCharGlyph) {
//		glyphPairs.Add( new( 0, 65535 ) );
//	}

//	return glyphPairs.ToArray();
//}