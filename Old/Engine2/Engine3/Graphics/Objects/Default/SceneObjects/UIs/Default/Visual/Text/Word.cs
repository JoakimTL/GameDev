using System.Collections.Generic;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class Word {

		public IReadOnlyList<InternalLetter> Letters => letters.AsReadOnly();
		private List<InternalLetter> letters;
		public int NumSymbols => letters.Count;
		public float Length { get; private set; }

		public Word(  ) {
			letters = new List<InternalLetter>();
		}

		public InternalLetter this[ int index ] => letters[ index ];

		public Word SubtextStart( int start ) {
			if( start > NumSymbols )
				return null;
			if( start < 0 )
				start = 0;
			Word w = new Word();
			for( int i = start; i < NumSymbols; i++ )
				w.Add( this[ i ] );
			return w;
		}
		public Word SubtextEnd( int end ) {
			if( end > NumSymbols )
				return null;
			if( end < 0 )
				return null;
			Word w = new Word();
			for( int i = 0; i < end; i++ )
				w.Add( this[ i ] );
			return w;
		}

		public void Add( InternalLetter s ) {
			letters.Add( s );
			Length += s.Symbol.SizeX;
		}

		public override string ToString() {
			string s = "";
			for( int i = 0; i < letters.Count; i++ ) {
				s += letters[ i ].Symbol.Character;
			}
			return s;
		}

	}
}
