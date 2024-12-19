namespace Engine.Standard.Render.Text.Fonts.Tables;

[Flags]
public enum MacStyle : ushort {
	/*
bit 0 bold
bit 1 italic
bit 2 underline
bit 3 outline
bit 4 shadow
bit 5 condensed (narrow)
bit 6 extended
	 */
	Bold = 0b0000_0000_0000_0001,
	Italic = 0b0000_0000_0000_0010,
	Underline = 0b0000_0000_0000_0100,
	Outline = 0b0000_0000_0000_1000,
	Shadow = 0b0000_0000_0001_0000,
	Condensed = 0b0000_0000_0010_0000,
	Extended = 0b0000_0000_0100_0000,
}

//public const uint Tag_Hmtx = 2020896104;