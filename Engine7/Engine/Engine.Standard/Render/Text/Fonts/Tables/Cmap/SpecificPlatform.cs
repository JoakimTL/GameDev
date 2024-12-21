namespace Engine.Standard.Render.Text.Fonts.Tables.Cmap;

public enum SpecificPlatform : ushort {
	Unicode_Version1_0 = 0b0_0000_0000,
	Unicode_Version1_1 = 0b0_0001_0000,
	/// <summary>
	/// Deprecated
	/// </summary>
	Unicode_ISO_10646_1993 = 0b0_0010_0000,
	Unicode_2_0_BMP = 0b0_0011_0000,
	Unicode_2_0_Full = 0b0_0100_0000,
	Unicode_VariationSequences = 0b0_0101_0000,
	Unicode_LastResort = 0b0_0110_0000,
	Macintosh_QuickDraw = 0b0_0000_0001,
	Windows_Symbol = 0b0_0000_0011,
	Windows_Unicode_BMP = 0b0_0001_0011,
	Windows_Shift_Jis = 0b0_0010_0011,
	Windows_PRC = 0b0_0011_0011,
	Windows_Big5 = 0b0_0100_0011,
	Windows_Wansung = 0b0_0101_0011,
	Windows_Johab = 0b0_0110_0011,
	Windows_Unicode_Ucs4 = 0b1_0010_0011,
}
