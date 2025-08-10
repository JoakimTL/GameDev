namespace Civlike.Services.Names;

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
		this._rng = seed.HasValue ? new( seed.Value ) : new();
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

			foreach (string syll in kv.Value)
				if (syll == END)
					nextKeys.Add( END );
				else {
					// compute the next context by shifting
					List<string> parts = key.Split( '|' ).ToList();
					parts.RemoveAt( 0 );
					parts.Add( syll );
					nextKeys.Add( string.Join( "|", parts ) );
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
		for (int t = 1; t <= maxSylls; t++)
			foreach (string? s in states) {
				if (s == END)
					continue;
				long sum = 0;
				foreach (string nxt in graph[ s ])
					sum += count[ idx[ nxt ], t - 1 ];
				count[ idx[ s ], t ] = sum;
			}

		// Sum up all walks from startKey to END of length 1..maxSylls
		long total = 0;
		int startIdx = idx[ startKey ];
		for (int t = 1; t <= maxSylls; t++)
			total += count[ startIdx, t ];

		return total;
	}
}
