namespace Civlike.Services.Names;

public static class NameFormatter {
	// Words to leave lowercase if they’re in the middle
	private static readonly HashSet<string> SmallWords = new( StringComparer.OrdinalIgnoreCase )
	{
		"a", "an", "and", "as", "at", "but", "by",
		"for", "in", "nor", "of", "on", "or", "so",
		"the", "to", "up", "yet"
	};

	public static string TitleCaseWithExceptions( string rawName ) {
		if (string.IsNullOrWhiteSpace( rawName ))
			return rawName;

		string[] words = rawName
			.Split( ' ', StringSplitOptions.RemoveEmptyEntries );

		for (int i = 0; i < words.Length; i++) {
			string w = words[ i ].ToLowerInvariant();

			// Always capitalize first and last word
			bool isFirstOrLast = i == 0 || i == words.Length - 1;

			if (isFirstOrLast || !SmallWords.Contains( w )) 				// Uppercase first letter, keep rest as-is
				words[ i ] = char.ToUpperInvariant( w[ 0 ] ) + w.Substring( 1 );
else 				// Leave a true “small word” entirely lowercase
				words[ i ] = w;
		}

		return string.Join( " ", words );
	}
}
