using System.Collections.Generic;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class Line {
		public int StartIndex { get; private set; }
		public int NumWords { get { return words.Count; } }
		public float Length { get; private set; }

		private List<Word> words;
		private List<InternalLetter> letters;
		private bool padding;
		public IReadOnlyList<Word> Words => words.AsReadOnly();
		public IReadOnlyList<InternalLetter> Letters => letters.AsReadOnly();

		private TextData data;

		public Line( int start, TextData data ) {
			StartIndex = start;
			this.data = data;
			words = new List<Word>();
			letters = new List<InternalLetter>();
		}

		public Word this[ int index ] => words[ index ];

		public Word LastWord {
			get {
				if( words.Count == 0 )
					words.Add( new Word() );
				return words[ words.Count - 1 ];
			}
		}

		public Word RemoveLast() {
			if( words.Count == 0 )
				return new Word();
			Word w = words[ words.Count - 1 ];
			words.RemoveAt( words.Count - 1 );
			Length -= w.Length;
			letters.RemoveRange( letters.Count - w.NumSymbols, w.NumSymbols );
			return w;
		}

		public Word Add( Word word ) {
			if( word.NumSymbols == 0 )
				return null;
			if( word.Length > data.Attributes.MaxLength ) {
				if( data.Attributes.BreakLine ) {
					int i = 0;
					while( Length + word[ i ].Symbol.SizeX <= data.Attributes.MaxLength )
						AddLetter( word[ i++ ] );
					return word.SubtextStart( i );
				} else {
					if( words.Count == 0 ) {
						AddWord( word );
						return null;
					} else {
						return word;
					}
				}
			} else {
				if( word.Length + Length > data.Attributes.MaxLength ) {
					if( data.Attributes.Block ) {
						int i = 0;
						while( Length + word[ i ].Symbol.SizeX <= data.Attributes.MaxLength )
							AddLetter( word[ i++ ] );
						return word.SubtextStart( i );
					} else {
						if( words.Count == 0 ) {
							AddWord( word );
							return null;
						} else {
							return word;
						}
					}
				}
			}
			AddWord( word );
			return null;
		}

		public void AddPadding() {
			if( !padding )
				Length += data.Attributes.Font.Data[ ' ' ].SizeX;
			padding = true;
		}

		public void RemovePadding() {
			if( padding )
				Length -= data.Attributes.Font.Data[ ' ' ].SizeX;
			padding = false;
		}

		private void AddWord( Word word ) {
			if( words.Count == 0 )
				words.Add( new Word() );
			for( int i = 0; i < word.NumSymbols; i++ ) {
				AddLetter( word[ i ] );
			}
		}

		private void AddLetter( InternalLetter l, bool add = true ) {
			if( words.Count == 0 )
				words.Add( new Word() );

			words[ words.Count - 1 ].Add( l );
			if( add )
				letters.Add( l );
			Length += l.Symbol.SizeX;

			if( l.Symbol.Character == ' ' )
				words.Add( new Word() );
		}

		public void AddLineBreak() {
			letters.Add( new InternalLetter( data.Attributes.Font.Data.LineBreak, 0 ) );
		}

		public Word RemoveAfter( int absoluteIndex ) {
			if( StartIndex + letters.Count <= absoluteIndex )
				return null;
			int index = absoluteIndex - StartIndex;
			if( index < 0 || index > letters.Count )
				return null;
			letters.RemoveRange( index, letters.Count - index );
			Reconstruct();
			return null;
		}

		public InternalLetter GetLetter( int absoluteIndex ) {
			int index = absoluteIndex - StartIndex;
			if( index >= 0 && index < letters.Count )
				return letters[ index ];
			return new InternalLetter( data.Attributes.Font.Data[ ' ' ], 0 );
		}

		private void Reconstruct() {
			Length = 0;
			words.Clear();

			Word w = RemoveLast();
			for( int i = 0; i < letters.Count; i++ ) {
				AddLetter( letters[ i ], false );
			}
		}

		public override string ToString() {
			string s = "";
			for( int i = 0; i < NumWords; i++ ) {
				s += words[ i ];
				s += " ";
			}
			s += NumWords;
			return s
				;
		}
	}
}
