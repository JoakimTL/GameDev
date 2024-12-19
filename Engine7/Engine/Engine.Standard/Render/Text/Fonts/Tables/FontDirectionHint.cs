namespace Engine.Standard.Render.Text.Fonts.Tables;

public enum FontDirectionHint : short {
	/*
0 Mixed directional glyphs
1 Only strongly left to right glyphs
2 Like 1 but also contains neutrals
-1 Only strongly right to left glyphs
-2 Like -1 but also contains neutrals
	*/
	StronglyRightToLeftWithNeutrals = -2,
	StronglyRightToLeft = -1,
	Mixed = 0,
	StronglyLeftToRight = 1,
	StronglyLeftToRightWithNeutrals = 2
}

//public const uint Tag_Hmtx = 2020896104;