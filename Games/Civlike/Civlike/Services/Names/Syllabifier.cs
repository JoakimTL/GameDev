namespace Civlike.Services.Names;

public static class Syllabifier {
	private static readonly HashSet<string> _clusters = new( StringComparer.OrdinalIgnoreCase ) { "sh", "th", "ch", "ll", "ss", "tt", "ng", "ph", "ck", "rt", "ld", "st" };

	public static string[] HeuristicSyllabify( string word ) {
		const string V = "aeiouyæøå";
		List<string> parts = [];
		int start = 0;
		word = word.ToLowerInvariant();

		for (int i = 1; i < word.Length; i++) {
			bool prevIsVowel = V.Contains( word[ i - 1 ] );
			bool currIsCons = !V.Contains( word[ i ] );
			bool restHasVowel = word[ i.. ].Any( V.Contains );
			bool inCluster = i + 1 < word.Length && _clusters.Contains( word[ (i - 1)..(i + 1) ] );

			// cut *after* a vowel before a consonant
			if (prevIsVowel && currIsCons && restHasVowel && !inCluster) {
				parts.Add( word[ start..i ] );
				start = i;
			}
		}
		// trailing chunk
		parts.Add( word[ start.. ] );

		const int minLen = 2;
		for (int i = 1; i < parts.Count; i++)
			if (parts[ i ].Length < minLen) {
				parts[ i - 1 ] += parts[ i ];
				parts.RemoveAt( i );
				i--; // re-evaluate at this index
			}
		return parts.ToArray();
	}
}
