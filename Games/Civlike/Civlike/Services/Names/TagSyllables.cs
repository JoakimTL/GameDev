namespace Civlike.Services.Names;

public sealed class TagSyllables {

	private readonly List<string> _headSyllables;
	private readonly List<string> _middleSyllables;
	private readonly List<string> _tailSyllables;

	public TagSyllables( string tag, IEnumerable<string> words ) {
		IEnumerable<(string syllable, SyllablePlacement placement)> syllables = words.SelectMany( p => AssignSyllablePositions( [ .. Syllabifier.HeuristicSyllabify( p ).Where( s => !string.IsNullOrWhiteSpace( s ) ).Select( s => s.ToLowerInvariant() ) ] ) );
		this._headSyllables = [ .. syllables.Where( s => s.placement == SyllablePlacement.Head ).Select( s => s.syllable ) ];
		this._middleSyllables = [ .. syllables.Where( s => s.placement == SyllablePlacement.Middle ).Select( s => s.syllable ) ];
		this._tailSyllables = [ .. syllables.Where( s => s.placement == SyllablePlacement.Tail ).Select( s => s.syllable ) ];
		this.Tag = tag;
	}

	public string Tag { get; }

	public string GetHead( Random random ) => this._headSyllables[ random.Next( this._headSyllables.Count ) ];
	public string GetMiddle( Random random ) => this._middleSyllables[ random.Next( this._middleSyllables.Count ) ];
	public string GetTail( Random random ) => this._tailSyllables[ random.Next( this._tailSyllables.Count ) ];

	private static (string syllable, SyllablePlacement placement)[] AssignSyllablePositions( string[] syllables ) {
		(string, SyllablePlacement)[] placedSyllables = new (string, SyllablePlacement)[ syllables.Length ];
		for (int i = 0; i < syllables.Length; i++)
			placedSyllables[ i ] = (syllables[ i ], i == 0 ? SyllablePlacement.Head : i == syllables.Length - 1 ? SyllablePlacement.Tail : SyllablePlacement.Middle);
		return placedSyllables;
	}

	private enum SyllablePlacement {
		Head,
		Middle,
		Tail
	}
}
