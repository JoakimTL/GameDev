using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class TextData {

		public bool HasUnhandledChanges { get; private set; }

		public TextAttributeData Attributes { get; private set; }

		public string Content { get; private set; }

		public int NumLines => lines.Count;

		private List<Line> lines;
		private List<DisplayLetter> letters;
		private Dictionary<int, Vector4i> colorCodes;
		//private List<IReadOnlyList<Vector2>> collisionData;
		private List<byte> letterData;
		//public Shape2Polygon.Mold CollisionModel { get; private set; }

		public event Action EventDataChanged;

		public TextData( TextAttributeData attributes, ColorCodedContent content ) {
			Attributes = attributes;
			letters = new List<DisplayLetter>();
			letterData = new List<byte>();
			lines = new List<Line>();
			colorCodes = content.ColorCodes as Dictionary<int, Vector4i>;
			//collisionData = new List<IReadOnlyList<Vector2>>();
			//CollisionModel = new Shape2Polygon.Mold();
			Set( content.Content );
		}

		public TextData( TextAttributeData attributes, string originalContent ) {
			Attributes = attributes;
			letters = new List<DisplayLetter>();
			letterData = new List<byte>();
			lines = new List<Line>();
			colorCodes = new Dictionary<int, Vector4i>();
			//collisionData = new List<IReadOnlyList<Vector2>>();
			//CollisionModel = new Shape2Polygon.Mold();
			Set( originalContent );
		}

		public ColorCodedContent AsColorCodedContent() {
			return new ColorCodedContent( Content, colorCodes );
		}

		public void Set( string content, bool clearColors = true ) {
			if( Content == content )
				return;
			Content = content;
			if( clearColors )
				ClearColors();
			HasUnhandledChanges = true;
		}

		public void Set( string content, Vector4i color ) {
			if( Content == content )
				return;
			Content = content;
			SetColor( color, 0, content.Length );
			HasUnhandledChanges = true;
		}

		public int Append( string content ) {
			Content += content;
			HasUnhandledChanges = true;
			return Content.Length;
		}

		public int Append( string content, Vector4i color ) {
			SetColor( color, Content.Length, content.Length );
			Content += content;
			HasUnhandledChanges = true;
			return Content.Length;
		}

		public int Append( ColorCodedContent content ) {
			foreach( KeyValuePair<int, Vector4i> cc in content.ColorCodes )
				colorCodes[ this.Content.Length + cc.Key ] = cc.Value;
			Content += content.Content;
			HasUnhandledChanges = true;
			return Content.Length;
		}

		public int Append( string content, int index, bool shoveColors = true ) {
			if( index >= 0 && index <= Content.Length ) {
				Content = Content.Insert( index, content );
				if( shoveColors )
					ShoveColors( index + 1, content.Length );
				HasUnhandledChanges = true;
				return index + content.Length;
			} else if( index > Content.Length ) {
				Append( content );
				return index + content.Length;
			} else {
				if( Content.Length == 0 && index == -1 ) {
					Content = content;
					HasUnhandledChanges = true;
					return content.Length;
				}
			}
			return -1;
		}

		public int Remove( int index, int count, bool removeColors = true ) {
			if( index < 0 )
				return 0;
			if( index >= Content.Length )
				return Content.Length;
			if( index + count > Content.Length )
				count = Content.Length - index;
			Content = Content.Remove( index, count );
			if( removeColors )
				ClearColors( index, count );
			HasUnhandledChanges = true;
			return index;
		}

		public void SetColor( Vector4i color, int startIndex, int length ) {
			for( int i = startIndex; i < startIndex + length; i++ )
				colorCodes[ i ] = color;
			if( length > 0 )
				HasUnhandledChanges = true;
		}

		public void ShoveColors( int startIndex, int length ) {
			for( int i = Content.Length - length; i >= startIndex; i-- ) {
				if( colorCodes.TryGetValue( i, out Vector4i color ) ) {
					colorCodes[ i + length ] = color;
					colorCodes.Remove( i );
				}
			}
			HasUnhandledChanges = true;
		}

		public void ClearColors( int startIndex, int length ) {
			for( int i = startIndex; i < startIndex + length; i++ )
				colorCodes.Remove( i );
			HasUnhandledChanges = true;
		}

		public void ClearColors() {
			colorCodes.Clear();
			HasUnhandledChanges = true;
		}

		public int ClearContent() {
			Content = "";
			ClearColors();
			HasUnhandledChanges = true;
			return 0;
		}

		public bool Update() {
			if( !HasUnhandledChanges )
				return false;
			HasUnhandledChanges = false;

			if( Attributes.Expanding ) {
				if( Content.Length > Attributes.MaxStringLength )
					Attributes.MaxStringLength = Attributes.MaxStringLength * 2;
			}

			OrderData();

			return true;
		}

		private void OrderData() {
			Word currentWord;
			Line currentLine;

			lines.Clear();
			currentLine = new Line( 0, this );
			lines.Add( currentLine );

			int i = 0;
			while( i < Content.Length ) {
				currentWord = currentLine.RemoveLast();
				while( i < Content.Length && Content[ i ] != ' ' && Content[ i ] != '\n' ) {
					Symbol s = Attributes.Font.Data[ Content[ i++ ] ];
					Vector4i color;
					if( !colorCodes.TryGetValue( i - 1, out color ) )
						color = 255;
					currentWord.Add( new InternalLetter( s, color ) );
				}
				if( i < Content.Length )
					if( Content[ i ] == ' ' )
						currentWord.Add( new InternalLetter( Attributes.Font.Data[ Content[ i ] ], 0 ) );
				Word w = currentLine.Add( currentWord );
				while( w != null ) {
					currentLine.AddPadding();
					currentLine = new Line( currentLine.StartIndex + currentLine.Letters.Count, this );
					lines.Add( currentLine );
					w = currentLine.Add( w );
				}
				if( i + 1 < Content.Length ) {
					if( Content[ i ] == '\n' ) {
						currentLine.AddLineBreak();
						currentLine.AddPadding();
						currentLine = new Line( currentLine.StartIndex + currentLine.Letters.Count, this );
						lines.Add( currentLine );
					}
					i++;
				} else if( i + 1 == Content.Length && ( Content[ i ] == ' ' || Content[ i ] == '\n' ) )
					i++; //essentially a break!
			}

			if( lines.Count > 0 )
				lines[ ^1 ].AddPadding();

			EventDataChanged?.Invoke();
		}

		internal void UpdateInstanceData() {
			letterData.Clear();
			for( int i = 0; i < letters.Count; i++ )
				letters[ i ].InjectData( letterData );
		}

		internal IReadOnlyList<byte> GetInstanceData() {
			return letterData;
		}

		public void CreateLetters() {
			float lineHeight = Attributes.Font.Data.LineHeight;
			Vector2 curser = new Vector2(
				0, Attributes.VerticalAlignment == VerticalAlignment.CENTER ? ( lines.Count * lineHeight ) / 2 : ( Attributes.VerticalAlignment == VerticalAlignment.TOP ? lines.Count * lineHeight : 0 )
			);
			letters.Clear();

			//collisionData.Clear();
			for( int i = 0; i < lines.Count; i++ ) {
				curser.X = Attributes.HorizontalAlignment == HorizontalAlignment.CENTER ? -lines[ i ].Length / 2 : ( Attributes.HorizontalAlignment == HorizontalAlignment.RIGHT ? -lines[ i ].Length : 0 );
				foreach( InternalLetter letter in lines[ i ].Letters ) {
					//List<Vector2> charColl = new List<Vector2>();
					AddCharacter( curser, letter/*, charColl*/ );
					//collisionData.Add( charColl.AsReadOnly() );
					curser.X += letter.Symbol.SizeX;
				}
				curser.Y -= lineHeight;
			}

			//CollisionModel.Set( collisionData );
		}

		private void AddCharacter( Vector2 curser, InternalLetter letter/*, List<Vector2> charColl */) {
			Symbol symbol = letter.Symbol;
			float x1 = curser.X + symbol.OffsetX;
			float y1 = curser.Y - symbol.OffsetY;
			//float x2 = x1 + symbol.Width;
			//float y2 = y1 - symbol.Height;

			letters.Add( new DisplayLetter(
				new Vector2( x1, y1 ),
				new Vector2( symbol.Width, -symbol.Height ),
				new Vector2( symbol.TextureX, 1 - symbol.TextureY ),
				new Vector2( symbol.TextureW, -symbol.TextureH ),
				letter.Color.AsByte )
			);

			//charColl.Add( new Vector2( x1, y1 ) );
			//charColl.Add( new Vector2( x2, y1 ) );
			//charColl.Add( new Vector2( x2, y2 ) );
			//charColl.Add( new Vector2( x1, y2 ) );
		}

		/// <summary>
		/// Finds the X and Y translation. This is BEFORE the symbol, not after.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public Vector2 GetPosition( int index, Vector2 scale ) {
			if( lines.Count == 0 )
				return 0;
			Vector2 ret = new Vector2(
				0, Attributes.VerticalAlignment == VerticalAlignment.CENTER ? ( lines.Count * Attributes.Font.Data.LineHeight ) / 2 : ( Attributes.VerticalAlignment == VerticalAlignment.TOP ? lines.Count * Attributes.Font.Data.LineHeight : 0 )
			);
			Line line = lines[ ^1 ];
			index = Math.Max( 0, Math.Min( line.StartIndex + line.Letters.Count + 1, index ) );
			int i;
			for( i = 0; i < lines.Count; i++ ) {
				if( i == lines.Count - 1 )
					break;
				if( lines[ i + 1 ].StartIndex > index ) {
					line = lines[ i ];
					break;
				}
			}

			ret.Y -= i * Attributes.Font.Data.LineHeight;
			ret.X = Attributes.HorizontalAlignment == HorizontalAlignment.CENTER ? -line.Length / 2 : ( Attributes.HorizontalAlignment == HorizontalAlignment.RIGHT ? -line.Length : 0 );
			int ind = line.StartIndex;
			while( ind < index ) {
				ret.X += line.GetLetter( ind++ ).Symbol.SizeX;
			}
			return ret * scale;
		}

		public int GetIndex( Vector2 position, Vector2 textTranslation, Vector2 scale ) {
			float lineHeight = Attributes.Font.Data.LineHeight;
			float curserX, curserY = ( Attributes.VerticalAlignment == VerticalAlignment.CENTER ? ( lines.Count * scale.Y * lineHeight ) / 2 : ( Attributes.VerticalAlignment == VerticalAlignment.TOP ? lines.Count * scale.Y * lineHeight : 0 ) );

			curserY -= position.Y;
			curserY += textTranslation.Y;

			int line = (int) Math.Floor( curserY / ( lineHeight * scale.Y ) );
			if( line >= 0 && line < lines.Count ) {
				Line l = lines[ line ];
				float startX = Attributes.HorizontalAlignment == HorizontalAlignment.CENTER ? -l.Length / 2 : ( Attributes.HorizontalAlignment == HorizontalAlignment.RIGHT ? -l.Length : 0 );
				curserX = startX;
				curserX -= position.X / scale.X;
				curserX += textTranslation.X / scale.X;
				float wantedLen = -curserX;
				float len = 0;
				int i;
				for( i = 0; i < l.Letters.Count; i++ ) {
					if( l.Letters[ i ].Symbol.Character == '\n' )
						break;
					float symbolLen = l.Letters[ i ].Symbol.SizeX;
					if( len + symbolLen > wantedLen ) {
						i += Math.Max( 0, Math.Sign( ( wantedLen - len ) - symbolLen / 2 ) );
						break;
					} else
						len += symbolLen;

				}
				return i + l.StartIndex;
			}
			if( line < 0 )
				return 0;
			return Content.Length;
		}

		public void SublinesFrom( TextData data, int startLine, int endLine, bool copyColors = true ) {
			if( startLine > endLine || data.lines.Count == 0 )
				return;

			int start = Math.Max( 0, Math.Min( startLine, data.lines.Count ) );
			int end = Math.Max( 0, Math.Min( endLine, data.lines.Count ) - 1 );

			if( start > end )
				return;

			Set( data.Content.Substring( data.lines[ start ].StartIndex, data.lines[ end ].StartIndex - data.lines[ start ].StartIndex + data.lines[ end ].Letters.Count ), true );
			if( copyColors )
				CopyColors( data.lines[ start ].StartIndex, data.lines[ end ].StartIndex - data.lines[ start ].StartIndex + data.lines[ end ].Letters.Count, data );
		}

		private void CopyColors( int startIndex, int length, TextData data ) {
			for( int i = startIndex; i < startIndex + length; i++ )
				if( data.colorCodes.TryGetValue( i, out Vector4i col ) )
					colorCodes[ i - startIndex ] = col;
		}

		public override string ToString() {
			StringBuilder s = new StringBuilder();

			s.Append( Content );

			s.Append( '\n' );

			s.Append( letters.Count );

			s.Append( "\nLINES\n" );
			for( int i = 0; i < lines.Count; i++ ) {
				s.Append( i );
				s.Append( '|' );
				for( int j = 0; j < lines[ i ].NumWords; j++ ) {
					for( int k = 0; k < lines[ i ][ j ].NumSymbols; k++ ) {
						s.Append( lines[ i ][ j ][ k ].Symbol.Character );
					}
					if( j < lines[ i ].NumWords - 1 )
						s.Append( '_' );
				}
				s.Append( '|' );
				s.Append( lines[ i ].NumWords );
				s.Append( '\n' );
			}

			s.Append( "LINLEN/WORLEN/ACTLEN\n" );
			for( int i = 0; i < lines.Count; i++ ) {
				s.Append( "Line " + i + ": " );
				float worlen = 0;
				float linlen = 0;

				for( int j = 0; j < lines[ i ].NumWords; j++ ) {
					for( int k = 0; k < lines[ i ][ j ].NumSymbols; k++ )
						linlen += lines[ i ][ j ][ k ].Symbol.SizeX;
				}

				for( int j = 0; j < lines[ i ].NumWords; j++ ) {

					s.Append( ":" );
					s.Append( lines[ i ][ j ].Length );
					s.Append( ":" );
					worlen += lines[ i ][ j ].Length;
				}
				s.Append( linlen );
				s.Append( '/' );
				s.Append( worlen );
				s.Append( '/' );
				s.Append( lines[ i ].Length );
				s.Append( '\n' );
			}

			s.Append( '\n' );

			for( int i = 0; i < lines.Count; i++ ) {
				for( int j = 0; j < Content.Length; j++ ) {
					s.Append( lines[ i ].GetLetter( j ).Symbol.Character );
				}

				s.Append( '\n' );
			}

			foreach( var c in colorCodes ) {
				s.Append( c.Key + ": " + c.Value );
				s.Append( '\n' );
			}

			return s.ToString();
		}
	}
}
