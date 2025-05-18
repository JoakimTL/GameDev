namespace Engine.Standard.Render.Text.Fonts.Tables.Cmap;

public abstract class CharacterMapFormatBase( ushort format ) {
	public static IReadOnlyDictionary<ushort, CharacterMapFormatBase> Formats = new Dictionary<ushort, CharacterMapFormatBase>() {
		//{ 0, new CharacterMapFormat0() },
		//{ 2, new CharacterMapFormat2() },
		{ 4, new CharacterMapFormat4() },
		//{ 6, new CharacterMapFormat6() },
		//{ 8, new CharacterMapFormat8() },
		//{ 10, new CharacterMapFormat10() },
		{ 12, new CharacterMapFormat12() },
	}.AsReadOnly();

	public ushort Format = format;

	public abstract List<GlyphMap> ReadFrom( FontCaretedDataReader caret );
}
