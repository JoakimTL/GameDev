using System.Runtime.CompilerServices;

namespace Engine.Standard.Render.Text.Fonts.Tables.Glyf;

public static class ElementalGlyphReader {
	public static IGlyphData Read( FontGlyphHeader header, GlyphMap glyphMap, FontCaretedDataReader caret ) {
		if (header.NumberOfContours < 0)
			throw new Exception( "Expected elemental glyph, but found compound glyph instead" );

		ushort[] endPointsOfContours = new ushort[ header.NumberOfContours ];
		for (int i = 0; i < header.NumberOfContours; i++)
			endPointsOfContours[ i ] = caret.Read<ushort>();

		int instructionLength = caret.Read<short>();
		byte[] instructions = new byte[ instructionLength ];
		for (int i = 0; i < instructionLength; i++)
			instructions[ i ] = caret.Read<byte>();
		int numPoints = endPointsOfContours[ ^1 ] + 1;
		byte[] flags = new byte[ numPoints ];
		for (int i = 0; i < numPoints; i++) {
			byte flag = caret.Read<byte>();
			flags[ i ] = flag;

			if (!ReadFlagBit( flag, 3 ))
				continue;
			byte repeat = caret.Read<byte>();
			for (int j = 0; j < repeat; j++) {
				i++;
				flags[ i ] = flag;
			}
		}

		int[] xCoordinates = ReadCoordinates( caret, flags, true );
		int[] yCoordinates = ReadCoordinates( caret, flags, false );
		RawGlyphPointData[] points = new RawGlyphPointData[ xCoordinates.Length ];
		for (int i = 0; i < xCoordinates.Length; i++)
			points[ i ] = new( (xCoordinates[ i ], yCoordinates[ i ]), ReadFlagBit( flags[ i ], 0 ) );

		return new ElementalGlyphData( header, glyphMap, points, endPointsOfContours );
	}

	private static int[] ReadCoordinates( FontCaretedDataReader caret, byte[] flags, bool readingX ) {
		int offsetSizeFlagBit = readingX ? 1 : 2;
		int offsetSignOrSkipBit = readingX ? 4 : 5;
		int[] coordinates = new int[ flags.Length ];
		for (int i = 0; i < flags.Length; i++) {
			int currentCoordinate = coordinates[ Math.Max( 0, i - 1 ) ];
			byte flag = flags[ i ];

			if (ReadFlagBit( flag, offsetSizeFlagBit ))
				currentCoordinate += caret.Read<byte>() * (ReadFlagBit( flag, offsetSignOrSkipBit ) ? 1 : -1);
			else if (!ReadFlagBit( flag, offsetSignOrSkipBit ))
				currentCoordinate += caret.Read<short>();
			coordinates[ i ] = currentCoordinate;
		}
		return coordinates;
	}

	private static bool ReadFlagBit( byte flag, int bit ) => ((flag >> bit) & 1) == 1;
}
