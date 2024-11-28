namespace Engine.Logging;

public static class StringExtensions {
	public static string GetUpperCaseLettersOnly( this string str, int trailingCharacters = 0, bool includeWhitespaces = false ) {
		int count = 0;
		int currentTrail = 0;
		Span<char> resultSpan = stackalloc char[ str.Length ];
		foreach (char c in str) {
			if (char.IsWhiteSpace( c )) {
				if (includeWhitespaces)
					resultSpan[ count++ ] = c;
				currentTrail = 0;
				continue;
			}
			if (char.IsUpper( c )) {
				resultSpan[ count++ ] = c;
				currentTrail = trailingCharacters;
			} else if (currentTrail > 0) {
				resultSpan[ count++ ] = c;
				currentTrail--;
			}
		}
		return new string( resultSpan[ ..count ] );
	}
}


