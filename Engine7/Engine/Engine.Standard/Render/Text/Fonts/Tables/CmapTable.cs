﻿using Engine.Standard.Render.Text.Fonts.Tables.Cmap;

namespace Engine.Standard.Render.Text.Fonts.Tables;

/// <summary>
/// <c>Character code mapping table</c><br/>
/// Contains information about how character codes map to glyph indices.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6cmap.html">Character code mapping table</see>
/// </summary>
public sealed class CmapTable : FontTable {

	private readonly ushort _version;
	private readonly Dictionary<SpecificPlatform, CharacterMap> _characterMapsBySpecificPlatform;
	private readonly Dictionary<uint, GlyphMap> _glyphMapByGlyphIndex;
	private readonly List<GlyphMap> _glyphMaps;

	public CmapTable( FontTableHeader header, FontDataReader reader ) : base( header ) {
		_characterMapsBySpecificPlatform = [];
		FontCaretedDataReader caret = new( reader );
		caret.GoTo( header.Offset );
		_version = caret.Read<ushort>();
		ushort numTables = caret.Read<ushort>();
		for (int i = 0; i < numTables; i++) {
			ushort platformId = caret.Read<ushort>();
			Platform platform = (Platform) platformId;
			ushort specificPlatformId = caret.Read<ushort>();
			SpecificPlatform specificPlatform = (SpecificPlatform) ((platformId & 0b1111) | ((specificPlatformId & 0b1_1111) << 4));
			_characterMapsBySpecificPlatform.Add( specificPlatform, new( specificPlatform, caret.Read<uint>() ) );
		}

		CharacterMap preferredCharacterMap = FindPreferredMap();

		caret.GoTo( header.Offset + preferredCharacterMap.Offset );
		CharacterMapFormatBase format = CharacterMapFormatBase.Formats[ caret.Read<ushort>() ];
		_glyphMaps = format.ReadFrom( caret );
		_glyphMapByGlyphIndex = [];
		foreach (GlyphMap glyphMap in _glyphMaps) {
			_glyphMapByGlyphIndex.Add( glyphMap.GlyphIndex, glyphMap );
		}
	}

	private CharacterMap FindPreferredMap() {
		Span<SpecificPlatform> platformPreference = [
			SpecificPlatform.Unicode_2_0_Full,
			//SpecificPlatform.Unicode_2_0_BMP,
			SpecificPlatform.Unicode_Version1_1,
			SpecificPlatform.Unicode_Version1_0,
			SpecificPlatform.Unicode_VariationSequences,
			SpecificPlatform.Unicode_LastResort,
			//SpecificPlatform.Unicode_ISO_10646_1993,
			SpecificPlatform.Windows_Symbol,
			//SpecificPlatform.Windows_Unicode_BMP,
			SpecificPlatform.Windows_Shift_Jis,
			SpecificPlatform.Windows_PRC,
			SpecificPlatform.Windows_Big5,
			SpecificPlatform.Windows_Johab,
			SpecificPlatform.Windows_Unicode_Ucs4,
		];
		CharacterMap preferredCharacterMap = null!;
		foreach (SpecificPlatform platform in platformPreference) {
			if (_characterMapsBySpecificPlatform.TryGetValue( platform, out CharacterMap? characterMap )) {
				preferredCharacterMap = characterMap;
				break;
			}
		}
		if (preferredCharacterMap == null)
			throw new Exception( "No preferred character map found" );
		return preferredCharacterMap;
	}

	public ushort Version => _version;
	public IReadOnlyDictionary<SpecificPlatform, CharacterMap> CharacterMaps => _characterMapsBySpecificPlatform;
	public IReadOnlyDictionary<uint, GlyphMap> GlyphMapByGlyphIndex => _glyphMapByGlyphIndex;
	public IReadOnlyList<GlyphMap> GlyphMaps => _glyphMaps;
}