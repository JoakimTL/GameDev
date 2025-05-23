﻿using Engine.Standard.Render.Text.Fonts;

namespace Engine.Standard.Render.Text.Typesetting;

public sealed class Word {

	public string Characters { get; private set; } = "";
	public char EndCharacter { get; private set; } = '\0';
	public float ScaledWidth { get; private set; }

	public int Read( Font font, ReadOnlySpan<char> text, float realScale ) {
		Span<char> charsInWord = stackalloc char[ text.Length ];
		int len = 0;
		for (; len < text.Length; len++) {
			char c = text[ len ];
			if (IsWordBreak( c )) {
				this.EndCharacter = c;
				break;
			}
			charsInWord[ len ] = c;
		}

		this.Characters = new string( charsInWord[ ..len ] );

		this.ScaledWidth = 0;
		for (int i = 0; i < this.Characters.Length; i++)
			this.ScaledWidth += (font[ this.Characters[ i ] ]?.Advance ?? 0) * realScale;

		return len;
	}

	public static bool IsWordBreak( char c ) => c == ' ' || c == '\t' || c == '\n' || c == '\r';
}