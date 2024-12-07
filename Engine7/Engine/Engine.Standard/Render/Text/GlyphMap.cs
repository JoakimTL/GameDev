namespace Engine.Standard.Render.Text;

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

	public override string ToString() => $"{this.GlyphIndex}->{this.Unicode}/{(char) this.Unicode}";
}
