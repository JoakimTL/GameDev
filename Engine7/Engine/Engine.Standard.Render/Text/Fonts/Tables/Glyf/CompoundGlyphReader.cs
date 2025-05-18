namespace Engine.Standard.Render.Text.Fonts.Tables.Glyf;

public class CompoundGlyphReader( GlyphReader glyphReader ) {
	private readonly GlyphReader _glyphReader = glyphReader;

	public IGlyphData Read( FontGlyphHeader header, GlyphMap glyphMap, FontCaretedDataReader caret ) {
		if (header.NumberOfContours >= 0)
			throw new Exception( "Expected compund glyph, but found elemental glyph instead" );

		CompoundGlyphData compoundGlyph = new( header, glyphMap );

		while (true) {
			(CompoundGlyphComponentData? componentGlyph, bool hasMoreGlyphs) = ReadNextComponentGlyph( caret, glyphMap );
			if (componentGlyph is null)
				break;
			compoundGlyph.AddComponentGlyph( componentGlyph );

			if (!hasMoreGlyphs)
				break;
		}

		return compoundGlyph;
	}

	private (CompoundGlyphComponentData? componentGlyph, bool hasMoreGlyphs) ReadNextComponentGlyph( FontCaretedDataReader caret, GlyphMap mapping ) {
		ushort flag = caret.Read<ushort>();
		ushort glyphIndex = caret.Read<ushort>();

		if (glyphIndex == mapping.GlyphIndex)
			return (null, false);

		bool argsAre2Bytes = ReadFlagBit( flag, 0 );
		bool argsAreXYValues = ReadFlagBit( flag, 1 );
		bool roundXYToGrid = ReadFlagBit( flag, 2 );
		bool isSingleScaleValue = ReadFlagBit( flag, 3 );
		bool isMoreComponentsAfterThis = ReadFlagBit( flag, 5 );
		bool isXAndYScale = ReadFlagBit( flag, 6 );
		bool is2x2Matrix = ReadFlagBit( flag, 7 );
		bool hasInstructions = ReadFlagBit( flag, 8 );
		bool useThisComponentMetrics = ReadFlagBit( flag, 9 );
		bool componentsOverlap = ReadFlagBit( flag, 10 );

		int arg1 = argsAre2Bytes ? caret.Read<short>() : caret.Read<sbyte>();
		int arg2 = argsAre2Bytes ? caret.Read<short>() : caret.Read<sbyte>();

		if (!argsAreXYValues)
			throw new Exception( "Font compound glyph component uses point indices as arguments (TODO)" );

		double offsetX = arg1;
		double offsetY = arg2;

		double iHat_x = 1;
		double iHat_y = 0;
		double jHat_x = 0;
		double jHat_y = 1;

		if (isSingleScaleValue)
			iHat_x = jHat_y = UInt16ToFixedPoint2Dot14( caret.Read<ushort>() );
		else if (isXAndYScale) {
			iHat_x = UInt16ToFixedPoint2Dot14( caret.Read<ushort>() );
			jHat_y = UInt16ToFixedPoint2Dot14( caret.Read<ushort>() );
		} else if (is2x2Matrix) {
			iHat_x = UInt16ToFixedPoint2Dot14( caret.Read<ushort>() );
			iHat_y = UInt16ToFixedPoint2Dot14( caret.Read<ushort>() );
			jHat_x = UInt16ToFixedPoint2Dot14( caret.Read<ushort>() );
			jHat_y = UInt16ToFixedPoint2Dot14( caret.Read<ushort>() );
		}

		IGlyphData componentGlyphData = _glyphReader.GetGlyphData( glyphIndex );
		CompoundGlyphComponentData componentGlyph = new( componentGlyphData );
		if (componentGlyphData is ElementalGlyphData)
			componentGlyph.Transform( offsetX, offsetY, iHat_x, iHat_y, jHat_x, jHat_y );

		return (componentGlyph, isMoreComponentsAfterThis);
	}

	public static double UInt16ToFixedPoint2Dot14( ushort raw ) {
		return (short) raw / (double) (1 << 14);
	}

	private static bool ReadFlagBit( ushort flag, int bit ) => (flag >> bit & 1) == 1;
}
