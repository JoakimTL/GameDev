using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Standard.Render.Text.Fonts.Tables;
/// <summary>
/// <c>Glyph outline table</c><br/>
/// Contains the vectorized outlines of glyphs. Used in conjunction with <see cref="LocaTable"/>.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6glyf.html">Glyph outline table documentation</see>
/// </summary>
public sealed class GlyfTable( FontTableHeader header, CmapTable cmap, LocaTable loca ) : FontTable( header ) {
}

/// <summary>
/// <c>Character code mapping table</c><br/>
/// Contains information about how character codes map to glyph indices.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6cmap.html">Character code mapping table</see>
/// </summary>
public sealed class CmapTable( FontTableHeader header ) : FontTable( header ) {
}

/// <summary>
/// <c>Kerning table</c><br/>
/// Contains kerning information for the font.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6kern.html">Kerning table documentation</see>
/// </summary>
public sealed class KernTable( FontTableHeader header ) : FontTable( header ) {
}

/// <summary>
/// <c>Glyph location table</c><br/>
/// Contains information about the location of each glyph in the font.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6loca.html">Glyph location table documentation</see>
/// </summary>
public sealed class LocaTable : FontTable {

	private List<uint> _glyphLocationOffsetBytes;

	public LocaTable( FontTableHeader header, HeadTable headTable, MaxPTable maxPTable, FontDataReader reader ) : base( header ) {
		_glyphLocationOffsetBytes = [];
		FontCaretedDataReader caret = new( reader );
		caret.GoTo( header.Offset );
		int numGlyphs = maxPTable.MaxGlyphs;
		bool isTwoByteEntry = headTable.BytesPerLocationLookup == 2;
		for (int glyphIndex = 0; glyphIndex < numGlyphs; glyphIndex++) {
			// If 2-byte format is used, the stored location is half of actual location (so multiply by 2)
			uint glyphDataOffset = isTwoByteEntry ? caret.Read<ushort>() * 2u : caret.Read<uint>();
			_glyphLocationOffsetBytes.Add( glyphDataOffset );
		}
	}

}

/// <summary>
/// <c>Horizontal header table</c><br/>
/// Contains information about the horizontal layout of the font.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6hhea.html">Horizontal header table documentation</see>
public sealed class HheaTable(FontTableHeader header) : FontTable( header ) { 
}


/// <summary>
/// <c>Horizontal metrics table</c><br/>
/// Contains information about the horizontal metrics of the font.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6hmtx.html">Horizontal metrics table documentation</see>
/// </summary>
public sealed class HmtxTable( FontTableHeader header ) : FontTable( header ) {
}