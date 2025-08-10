using Civlike.Logic.Setup;
using System.Text;

namespace Civlike.Logic;

public static class Definitions {
	public static DefinitionList<ResourceTypeBase> Resources { get; } = new();
	public static DefinitionList<RecipeTypeBase> Recipes { get; } = new();
	//public static DefinitionList<BuildingTypeBase> BuildingTypes { get; } = new();
	//public static DefinitionList<SectorTypeBase> Sectors { get; } = new();
	//public static DefinitionList<VocationTypeBase> Vocations { get; } = new();
	//public static DefinitionList<ProfessionTypeBase> Professions { get; } = new();
	//public static DefinitionList<TechnologyTypeBase> Technologies { get; } = new();
}

public class NameGenerator {

	private readonly List<char> _chars;
	private readonly Dictionary<char, LetterRelations> _relations;

	public NameGenerator() {
		this._chars = [];
		this._relations = [];
	}

	public string GetRandomName( Random random, int length ) {
		char current = this._chars[ random.Next( this._chars.Count ) ];
		Span<char> name = stackalloc char[ length ];
		int i = 0;
		name[ i ] = current;
		while (i < length) {
			if (!this._relations.TryGetValue( current, out LetterRelations? relation ))
				return new string( name[ ..i ] );
			name[ i ] = current = relation.GetRandomLetter( random, i );
			i++;
		}
		return new string( name );
	}

	public void AddName( string name ) {
		string lowerName = name.ToLower();
		for (int i = 0; i < lowerName.Length - 1; i++) {
			char c = lowerName[ i ];
			char n = lowerName[ i + 1 ];
			if (!this._chars.Contains( c ))
				this._chars.Add( c );
			if (!this._relations.TryGetValue( c, out LetterRelations? relationsC ))
				this._relations.Add( c, relationsC = new( c ) );
			relationsC.AddOccurrence( n, i );
		}
	}

	public void AddNames( params IEnumerable<string> names ) {
		foreach (string name in names)
			AddName( name );
	}
}

public sealed class LetterRelations {

	private readonly List<LetterOccurrences> _occurrences;
	private int _occurrenceCount;
	public char Letter { get; }

	public LetterRelations( char letter ) {
		this._occurrences = [];
		this._occurrenceCount = 0;
		this.Letter = letter;
	}

	public void AddOccurrence( char c, int position ) {
		LetterOccurrences? occurrences = this._occurrences.FirstOrDefault( oc => oc.Letter == c );
		if (occurrences == null)
			this._occurrences.Add( occurrences = new( c ) );
		occurrences.Count++;
		occurrences.PositionSum += position;
		this._occurrenceCount++;
	}

	public char GetRandomLetter( Random random, int position ) {
		long occurenceCountSq = 0;
		for (int i = 0; i < this._occurrences.Count; i++) {
			occurenceCountSq += (long) this._occurrences[ i ].Count * this._occurrences[ i ].Count * this._occurrences[ i ].Count * this._occurrences[ i ].Count;
		}
		long n = random.NextInt64( occurenceCountSq );
		long tallied = 0;
		for (int i = 0; i < this._occurrences.Count; i++) {
			LetterOccurrences letterOccurences = this._occurrences[ i ];
			long count = (long) letterOccurences.Count * letterOccurences.Count * letterOccurences.Count * letterOccurences.Count;
			if (n < tallied + count)
				return letterOccurences.Letter;
			tallied += count;
		}
		throw new Exception();
	}

	private class LetterOccurrences {
		public char Letter { get; }
		public int Count { get; set; }
		public float PositionSum { get; set; }
		public float AveragePosition => this.PositionSum / float.Max( this.Count, 1 );

		public LetterOccurrences( char letter ) {
			this.Letter = letter;
			this.Count = 0;
			this.PositionSum = 0;
		}
	}

}

public static class Syllabifier {
	private static readonly HashSet<string> _clusters = new( StringComparer.OrdinalIgnoreCase ) { "sh", "th", "ch", "ll", "ss", "tt", "ng", "ph", "ck", "rt", "ld", "st" };

	public static string[] HeuristicSyllabify( string word ) {
		const string V = "aeiouy";
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
		for (int i = 1; i < parts.Count; i++) {
			if (parts[ i ].Length < minLen) {
				parts[ i - 1 ] += parts[ i ];
				parts.RemoveAt( i );
				i--; // re-evaluate at this index
			}
		}
		return parts.ToArray();
	}
}
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
			bool isFirstOrLast = (i == 0) || (i == words.Length - 1);

			if (isFirstOrLast || !SmallWords.Contains( w )) {
				// Uppercase first letter, keep rest as-is
				words[ i ] = char.ToUpperInvariant( w[ 0 ] ) + w.Substring( 1 );
			} else {
				// Leave a true “small word” entirely lowercase
				words[ i ] = w;
			}
		}

		return string.Join( " ", words );
	}
}

public class SyllableMarkovGenerator {
	private readonly HashSet<string> _blacklistedEndings;
	private readonly HashSet<string> _seedNames;
	private readonly Dictionary<string, List<string>> _transitions = [];
	private readonly Random _rng;
	private readonly int _order;

	public SyllableMarkovGenerator( IEnumerable<string> seeds, IEnumerable<string> blacklistedEndings, int order = 1, int? seed = null ) {
		this._seedNames = [ .. seeds.Select( p => p.ToLower() ) ];
		this._blacklistedEndings = [ .. blacklistedEndings ];
		this._order = Math.Max( 1, order );
		this._rng = seed.HasValue ? new Random( seed.Value ) : new Random();
		BuildTransitions( this._seedNames );
	}

	private void BuildTransitions( IEnumerable<string> seeds ) {
		foreach (string raw in seeds) {
			List<string> syllables = Syllabifier.HeuristicSyllabify( raw )
									   .Select( s => s.ToLowerInvariant() )
									   .Where( s => !string.IsNullOrWhiteSpace( s ) )
									   .ToList();
			// add end token
			syllables.Add( "$" );

			// pad with start tokens
			string[] padded = Enumerable.Repeat( "^", this._order )
								   .Concat( syllables )
								   .ToArray();

			for (int i = 0; i + this._order < padded.Length; i++) {
				// key is a join of the last `order` syllables
				string key = string.Join( "|", padded[ i..(i + this._order) ] );
				string next = padded[ i + this._order ];

				if (!this._transitions.TryGetValue( key, out List<string>? list ))
					this._transitions[ key ] = list = [];

				list.Add( next );
			}
		}
	}

	/// <summary>
	/// Generate a new “name” by chaining syllables.
	/// maxSylls caps how many real syllables (not tokens) you’ll get.
	/// </summary>
	public string NextName( int maxSylls = 8, int minLength = 4 ) {
		while (true) {
			// initialize the sliding window with start tokens
			Queue<string> window = new( Enumerable.Repeat( "^", this._order ) );
			List<string> result = [];
			bool endedOnToken = false;

			while (result.Count < maxSylls) {
				string key = string.Join( "|", window );
				if (!this._transitions.TryGetValue( key, out List<string>? choices ))
					break;  // dead end; stop early

				string pick = choices[ this._rng.Next( choices.Count ) ];
				if (pick == "$") {
					endedOnToken = true;
					break;  // hit the end token
				}

				result.Add( pick );
				window.Enqueue( pick );
				window.Dequeue();
			}

			// join without separators, or use '-' if you want readability
			string name = string.Concat( result );

			if (!endedOnToken || name.Length < minLength || this._seedNames.Contains( name ) || this._blacklistedEndings.Any( name.EndsWith ))
				continue;

			return NameFormatter.TitleCaseWithExceptions( name );
		}
	}

	public long CountPossibleNames( int maxSylls ) {
		// 1) Build a map from each context-key to a list of “next keys”
		Dictionary<string, List<string>> graph = [];
		string startKey = string.Join( "|", Enumerable.Repeat( "^", this._order ) );
		const string END = "$";

		foreach (KeyValuePair<string, List<string>> kv in this._transitions) {
			string key = kv.Key;
			List<string> nextKeys = [];

			foreach (string syll in kv.Value) {
				if (syll == END)
					nextKeys.Add( END );
				else {
					// compute the next context by shifting
					List<string> parts = key.Split( '|' ).ToList();
					parts.RemoveAt( 0 );
					parts.Add( syll );
					nextKeys.Add( string.Join( "|", parts ) );
				}
			}
			graph[ key ] = nextKeys;
		}

		// 2) Prepare DP table: count[state][t]
		List<string> states = graph.Keys.Append( END ).Distinct().ToList();
		Dictionary<string, int> idx = states.Select( ( s, i ) => (s, i) ).ToDictionary( x => x.s, x => x.i );
		int S = states.Count;
		long[,] count = new long[ S, maxSylls + 1 ];

		// Base case
		count[ idx[ END ], 0 ] = 1;

		// Fill table
		for (int t = 1; t <= maxSylls; t++) {
			foreach (string? s in states) {
				if (s == END)
					continue;
				long sum = 0;
				foreach (string nxt in graph[ s ]) {
					sum += count[ idx[ nxt ], t - 1 ];
				}
				count[ idx[ s ], t ] = sum;
			}
		}

		// Sum up all walks from startKey to END of length 1..maxSylls
		long total = 0;
		int startIdx = idx[ startKey ];
		for (int t = 1; t <= maxSylls; t++)
			total += count[ startIdx, t ];

		return total;
	}
}

public class MarkovNameGenerator {
	private readonly HashSet<string> _seedNames;
	private readonly Dictionary<string, List<char>> _transitions = [];
	private readonly Random _rng;
	private readonly int _order;

	public MarkovNameGenerator( IEnumerable<string> seedNames, int order = 2, int? seed = null ) {
		this._order = Math.Max( 1, order );
		this._rng = seed.HasValue ? new Random( seed.Value ) : new Random();
		this._seedNames = [ .. seedNames ];
		BuildTransitions( this._seedNames );
	}

	// ---------------- core API ----------------
	public string NextName( int maxLength = 15 ) {
		do {
			// Begin with the special start token "^"
			StringBuilder buffer = new( "^".PadRight( this._order, '^' ) );

			while (true) {
				string key = buffer.ToString( buffer.Length - this._order, this._order );
				if (!this._transitions.TryGetValue( key, out List<char>? nextChars ))
					break;                                  // dead end – restart

				char next = nextChars[ this._rng.Next( nextChars.Count ) ];
				if (next == '$' || buffer.Length > maxLength + this._order)
					break;                                  // reached end token

				buffer.Append( next );
			}

			string name = buffer.ToString( this._order, buffer.Length - this._order );

			if (this._seedNames.Contains( name ))
				continue;

			// strip the leading start tokens
			return name;
		} while (true);
	}

	// --------------- internals -----------------
	private void BuildTransitions( IEnumerable<string> names ) {
		foreach (string raw in names) {
			string name = raw.Trim().ToLowerInvariant();    // keep it simple
			string padded = new string( '^', this._order ) + name + "$";

			for (int i = 0; i < padded.Length - this._order; i++) {
				string key = padded.Substring( i, this._order );
				char next = padded[ i + this._order ];

				if (!this._transitions.TryGetValue( key, out List<char>? list ))
					this._transitions[ key ] = list = [];

				list.Add( next );
			}
		}
	}
}