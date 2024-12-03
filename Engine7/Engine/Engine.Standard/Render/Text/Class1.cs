using Engine.Algorithms.Triangulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Standard.Render.Text;
internal class Class1 {

}

public sealed class FontService {

	public string FontBasePath { get; set; } = ".\\assets\\fonts";
	private readonly Dictionary<string, Font> _fonts = [];

	/// <summary>
	/// Throws a <see cref="FileNotFoundException"/> if the font file is not found. The font file is found in the <see cref="FontBasePath"/> directory, and must have the extension ".ttf".
	/// </summary>
	/// <exception cref="FileNotFoundException"></exception>
	public Font Get( string fontName ) {
		if (_fonts.TryGetValue( fontName, out Font? font ))
			return font;

		string path = Path.Combine( FontBasePath, $"{fontName}.ttf" );
		if (!File.Exists( path ))
			throw new FileNotFoundException( "Font file not found.", path );

		font = new Font( path );
		_fonts.Add( fontName, font );
		return font;
	}
}

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

		short[] xCoordinates = ReadCoordinates( srcPtr, ref offset, flags, true );
		short[] yCoordinates = ReadCoordinates( srcPtr, ref offset, flags, false );
		var points = new (Vector2<short> coordinate, bool onCurve)[ xCoordinates.Length ];
		for (int i = 0; i < xCoordinates.Length; i++)
			points[ i ] = (new Vector2<short>( xCoordinates[ i ], yCoordinates[ i ] ), ReadFlagBit( flags[ i ], 0 ));

		return new FontGlyph( header, mapping, points, endPointsOfContours, instructions, flags );

		short[] ReadCoordinates( byte* srcPtr, ref nint offset, byte[] flags, bool readingX ) {
			int offsetSizeFlagBit = readingX ? 1 : 2;
			int offsetSignOrSkipBit = readingX ? 4 : 5;
			short[] coordinates = new short[ flags.Length ];
			for (int i = 0; i < flags.Length; i++) {
				short currentCoordinate = coordinates[ Math.Max( 0, i - 1 ) ];
				byte flag = flags[ i ];

				if (ReadFlagBit( flag, offsetSizeFlagBit )) {
					currentCoordinate += (short) (FontUtilities.Read<byte>( srcPtr, ref offset ) * (ReadFlagBit( flag, offsetSignOrSkipBit ) ? 1 : -1));
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

//static uint[] GetAllGlyphLocations( FontReader reader, int numGlyphs, int bytesPerEntry, uint locaTableLocation, uint glyfTableLocation ) {
//	uint[] allGlyphLocations = new uint[ numGlyphs ];
//	bool isTwoByteEntry = bytesPerEntry == 2;

//	for (int glyphIndex = 0; glyphIndex < numGlyphs; glyphIndex++) {
//		reader.GoTo( locaTableLocation + glyphIndex * bytesPerEntry );
//		// If 2-byte format is used, the stored location is half of actual location (so multiply by 2)
//		uint glyphDataOffset = isTwoByteEntry ? reader.ReadUInt16() * 2u : reader.ReadUInt32();
//		allGlyphLocations[ glyphIndex ] = glyfTableLocation + glyphDataOffset;
//	}

//	return allGlyphLocations;
//}

public readonly struct GlyphMap( uint index, uint unicode ) {
	public readonly uint GlyphIndex = index;
	public readonly uint Unicode = unicode;
}

//public class GlyphData {
//	public uint UnicodeValue;
//	public uint GlyphIndex;
//	public Point[] Points;
//	public int[] ContourEndIndices;
//	public int AdvanceWidth;
//	public int LeftSideBearing;

//	public int MinX;
//	public int MaxX;
//	public int MinY;
//	public int MaxY;

//	public int Width => MaxX - MinX;
//	public int Height => MaxY - MinY;

//}
public sealed class FontGlyph( FontGlyphHeader header, GlyphMap mapping, (Vector2<short> coordinate, bool onCurve)[] points, ushort[] endPointsOfContours, byte[] instructions, byte[] flags ) {
	//https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6glyf.html
	public readonly FontGlyphHeader Header = header;
	private readonly GlyphMap _mapping = mapping;
	public uint Unicode => _mapping.Unicode;
	public uint GlyphIndex => _mapping.GlyphIndex;

	private readonly ushort[] _endPointsOfContours = endPointsOfContours;
	private readonly byte[] _instructions = instructions;
	private readonly byte[] _flags = flags;
	private readonly (Vector2<short> coordinate, bool onCurve)[] _points = points;

	public (Triangle2<float>, bool filled)[] CreateMeshTriangles(float scale) {
		Span<Vector2<decimal>> points = stackalloc Vector2<decimal>[ _points.Length ];
		for (int i = 0; i < _points.Length; i++)
			points[ i ] = _points[ i ].coordinate.CastSaturating<short, decimal>();
		var a = Delaunay.Triangulate( points ).ToArray();
		//var a = EarClipping.Triangulate( points ).ToArray();

		//https://learn.microsoft.com/en-us/typography/opentype/spec/ttch01#outlines
		//The points that make up a curve must be numbered in consecutive order. It makes a difference whether the order is increasing or decreasing in determining the fill pattern of the shapes that make up the glyph. The direction of the curves has to be such that, if the curve is followed in the direction of increasing point numbers, the black space (the filled area) will always be to the right.

		//So let's retian triangles where the triangle has any points is on a filling contour and remove the triangle if it only has points on a non-filling contour.
		Span<bool> displayTriangleA = stackalloc bool[ a.Length ];
		displayTriangleA.Fill( true );
		for (int i = 0; i < _endPointsOfContours.Length; i++) {
			SetDisplayedTriangles( a, points, displayTriangleA, i );
		}

		List<(Triangle2<float>, bool filled)> result = [];

		for (int i = 0; i < displayTriangleA.Length; i++) {
			if (displayTriangleA[ i ])
				result.Add( (new Triangle2<float>(a[ i ].A.CastSaturating<decimal, float>() * scale, a[i].B.CastSaturating<decimal, float>() * scale, a[i].C.CastSaturating<decimal, float>() * scale ), true) );
		}

		return result.ToArray();
	}

	private void SetDisplayedTriangles( Triangle2<decimal>[] triangles, Span<Vector2<decimal>> points, Span<bool> displayed, int contourIndex ) {
		uint contourStart = contourIndex > 0 ? _endPointsOfContours[ contourIndex - 1 ] : 0u;
		uint contourEnd = _endPointsOfContours[ contourIndex ];

		Span<Vector2<decimal>> pointInContour = points.Slice( (int) contourStart, (int) (contourEnd - contourStart) );

		if (PolygonExtensions.GetSignedArea( pointInContour ) < 0)
			return;

		for (int i = 0; i < triangles.Length; i++) {
			if (triangles[ i ].AllPointsIn( pointInContour ))
				displayed[ i ] = false;
		}
	}
}

public readonly struct FontGlyphHeader( short numberOfContours, short xMin, short yMin, short xMax, short yMax ) {
	//https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6glyf.html
	public readonly short NumberOfContours = numberOfContours;
	public readonly short XMin = xMin;
	public readonly short YMin = yMin;
	public readonly short XMax = xMax;
	public readonly short YMax = yMax;
	public FontGlyphHeader ProperEndianness => new( NumberOfContours.FromBigEndian(), XMin.FromBigEndian(), YMin.FromBigEndian(), XMax.FromBigEndian(), YMax.FromBigEndian() );
}

public readonly struct FontOffsetSubtable( uint scalerType, ushort numTables, ushort searchRange, ushort entrySelector, ushort rangeShift ) {
	//https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6.html
	public readonly uint ScalerType = scalerType;
	public readonly ushort NumTables = numTables;
	public readonly ushort SearchRange = searchRange;
	public readonly ushort EntrySelector = entrySelector;
	public readonly ushort RangeShift = rangeShift;
	public FontOffsetSubtable ProperEndianness => new( ScalerType.FromBigEndian(), NumTables.FromBigEndian(), SearchRange.FromBigEndian(), EntrySelector.FromBigEndian(), RangeShift.FromBigEndian() );
}


public readonly struct FontTable( uint tag, uint checksum, uint offset, uint length ) {
	//https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6.html
	public readonly uint Tag = tag;
	public readonly uint Checksum = checksum;
	public readonly uint Offset = offset;
	public readonly uint Length = length;
	public string TagString => Encoding.ASCII.GetString( BitConverter.GetBytes( Tag ) );
	public FontTable ProperEndianness => new( Tag, Checksum.FromBigEndian(), Offset.FromBigEndian(), Length.FromBigEndian() );

	public override string ToString() => $"Font Table {TagString}";
}


public static class FontUtilities {

	public static unsafe T FromBigEndian<T>( this T value ) where T : unmanaged, IBinaryNumber<T> {
		if (!BitConverter.IsLittleEndian)
			return value;
		T returnValue = T.Zero;
		byte* srcPtr = (byte*) &value;
		byte* dstPtr = (byte*) &returnValue;
		for (int i = 0; i < sizeof( T ); i++)
			dstPtr[ sizeof( T ) - i - 1 ] = srcPtr[ i ];
		return *(T*) dstPtr;
	}

	public static unsafe T Read<T>( byte* srcPtr, ref nint offset ) where T : unmanaged {
		T value = *(T*) (srcPtr + offset);
		offset += sizeof( T );
		return value;
	}
}

public sealed class FontCharacter {

}
