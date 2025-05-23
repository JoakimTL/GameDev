﻿using Engine.Standard.Render.Text.Fonts.Tables.Glyf;

namespace Engine.Standard.Render.Text.Fonts.Tables;
/// <summary>
/// <c>Glyph outline table</c><br/>
/// Contains the vectorized outlines of glyphs. Uses <see cref="CmapTable"/> and <see cref="LocaTable"/>.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6glyf.html">Glyph outline table documentation</see>
/// </summary>
public sealed class GlyfTable : FontTable {

	private readonly Dictionary<uint, IGlyphData> _glyphByIndex;
	private readonly Dictionary<char, IGlyphData> _glyphByUnicode;

	public GlyfTable( FontTableHeader header, LocaTable locaTable, CmapTable cmapTable, FontDataReader reader ) : base( header ) {
		this._glyphByIndex = [];
		this._glyphByUnicode = [];
		GlyphReader glyphReader = new( header, locaTable, cmapTable, reader );
		foreach (GlyphMap glyphMap in cmapTable.GlyphMaps)
			glyphReader.GetGlyphData( glyphMap.GlyphIndex );

		foreach (IGlyphData glyph in glyphReader.Glyphs) {
			this._glyphByIndex.Add( glyph.Mapping.GlyphIndex, glyph );
			if (!this._glyphByUnicode.ContainsKey( (char) glyph.Mapping.Unicode ))
				this._glyphByUnicode.Add( (char) glyph.Mapping.Unicode, glyph );
		}
	}

	public IReadOnlyDictionary<uint, IGlyphData> GlyphByIndex => this._glyphByIndex;
	public IReadOnlyDictionary<char, IGlyphData> GlyphByUnicode => this._glyphByUnicode;

	/// <summary>
	/// Glyph indicating a missing glyph.
	/// </summary>
	public IGlyphData? MissingGlyph => this._glyphByIndex.TryGetValue( 0, out IGlyphData? glyph ) ? glyph : null;
}
