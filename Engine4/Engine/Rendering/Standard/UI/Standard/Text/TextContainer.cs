using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Engine.Data.Datatypes;

namespace Engine.Rendering.Standard.UI.Standard.Text;
/// <summary>
/// This container is only for the text itself. Styling must be done outside.
/// </summary>
public class TextContainer {

	private readonly Font _font;
	private List<Line> _lines;
	public IReadOnlyList<Line> Lines => _lines;
	public float Textscale { get; private set; }
	public string Text { get; private set; }
	public bool Monospaced { get; private set; }
	public bool Kerned { get; private set; }
	public HorizontalAlignment HorizontalAlignment { get; private set; }
	public VerticalAlignment VerticalAlignment { get; private set; }
	public WordWrapMode WordWrapMode { get; private set; }
	private bool _updated;

	/// <param name="width">The number of monospaced character a line can fit. Characters can have varying width if not monospaced, which means more can fit in the same line.</param>
	/// <param name="monospaced">If true all letters will have the same width.</param>
	public TextContainer( Font font, string text, float textscale, bool monospaced = false, bool kerned = true, HorizontalAlignment horizontalAlignment = HorizontalAlignment.CENTER, VerticalAlignment verticalAlignment = VerticalAlignment.CENTER, WordWrapMode wordWrapMode = WordWrapMode.WrapWords ) {
		this._font = font;
		this._lines = new();
		this.Text = text;
		this.Textscale = textscale;
		this.Monospaced = monospaced;
		this.Kerned = kerned;
		this.HorizontalAlignment = horizontalAlignment;
		this.VerticalAlignment = verticalAlignment;
		this.WordWrapMode = wordWrapMode;
		_updated = true;
	}

	public void SetText( string text ) {
		if ( text == Text )
			return;
		Text = text;
		_updated = true;
	}

	public void SetTextscale( float textscale ) {
		if ( textscale == Textscale )
			return;
		Textscale = textscale;
		_updated = true;
	}

	public void SetMonospaced( bool monospaced ) {
		if ( monospaced == Monospaced )
			return;
		Monospaced = monospaced;
		_updated = true;
	}

	public void SetKerning( bool kerned ) {
		if ( kerned == Kerned )
			return;
		Kerned = kerned;
		_updated = true;
	}

	public void SetAlignment( HorizontalAlignment horizontalAlignment ) {
		if ( horizontalAlignment == HorizontalAlignment )
			return;
		HorizontalAlignment = horizontalAlignment;
		_updated = true;
	}

	public void SetAlignment( VerticalAlignment verticalAlignment ) {
		if ( verticalAlignment == VerticalAlignment )
			return;
		VerticalAlignment = verticalAlignment;
		_updated = true;
	}

	public void SetWrapMode( WordWrapMode wordWrapMode ) {
		if ( wordWrapMode == WordWrapMode )
			return;
		WordWrapMode = wordWrapMode;
		_updated = true;
	}

	/// <returns>True if updated, false if not.</returns>
	public bool Update() {
		if ( !_updated )
			return false;
		_updated = false;

		//Turn text into lines of characters to be rendered.
		ReadOnlySpan<char> text = Text;
		_lines.Clear();
		Line currentLine = GetEmptyLine();
		Word currentWord = new();
		FontCharacter bindCharacter = _font.Characters[ '-' ];
		for ( int i = 0; i < text.Length; i++ ) {
			char c = text[ i ];
			char nC = i < text.Length - 1 ? text[ i + 1 ] : '\n';
			FontCharacter fC = _font.Characters[ c ];
			/*
			- Linebreak causes new line
			- Spaces can cause new line
			- Wordwrap modes
				- None (No wrapping at all)
				- Auto (Scales the text to best fit the bounds, best suited for short texts)
				- Word (Wraps at the end of words)
				- Letter (Wraps whenever needed, might split words. This will give the text a rectangular shape)
			- Selection
				- Selection will be multiple boxes that indicate what text has been chosen.
				- Holding alt while selecting will enable the rectangular selection shape (try it in vs2022)
				- Selection can be disabled
					- Discern between normal and rectangular selection?
				- Selection can be hidden
					- Discern between normal and rectangular selection?
				- Selected text can be copied into clipboard
					- With rectangular selection, new lines and spaces are added to maintain rectangular look when pasted. (assumed monospacing when adding spaces)
			- Cursor
				- When text is clicked, the cursor position changes.
				- Arrow navigation for cursor
					- Holding CTRL allows the cursor to jump from word to word, rather than letter to letter
					- Holding CTRL+ALT allows the cursor to jump to the start of the line (left arrow) or end of the line (right arrow)
					- Holding SHIFT creates a selection
					- Holding SHIFT+ALT creates a rectangular selection
				- Cursor can be disabled
					- Disabled means the cursor can't be moved and clicking doesn't move it.
				- Cursor can be hidden
					- Hidden means the cursor is still there, but not visible
			- Readonly mode
			- Colorize and stylize text
			*/
			switch ( c ) {
				case '\r':
					break;
				case '\n':
					//new line
					currentLine.AddWord( currentWord );
					currentWord.Clear();
					currentLine = GetEmptyLine();
					break;
				case ' ':
					if ( WordWrapMode == WordWrapMode.WrapLetter || WordWrapMode == WordWrapMode.WrapWords ) {
						if ( currentLine.Width + currentWord.Width >= 0.99998474122f )
							currentLine = GetEmptyLine();
					}
					currentLine.AddWord( currentWord );
					currentWord.Clear();
					if ( currentLine.Count > 0 )
						currentLine.AddCharacter( fC, GetWidth( fC, nC ) );
					break;
				default:
					float width = GetWidth( fC, nC );
					if ( WordWrapMode == WordWrapMode.WrapLetter ) {
						currentWord.AddCharacter( fC, width );
						// width added to line, width of current word, width of a monospaced character (the binding sign)
						if ( currentLine.Width + currentWord.Width + Textscale >= 0.99998474122f ) {
							if ( currentWord.Characters.Count > 1 ) {
								currentWord.AddCharacter( bindCharacter, GetWidth( bindCharacter, '\n' ) );
								currentLine.AddWord( currentWord );
								currentWord.Clear();
							}
							currentLine = GetEmptyLine();
						}
						break;
					}
					currentWord.AddCharacter( fC, width );
					break;
			}
		}
		currentLine.AddWord( currentWord );

		return true;
	}

	private float GetWidth( FontCharacter currentCharacter, char nextCharacter ) {
		int xAdvance;
		if ( Monospaced ) {
			xAdvance = _font.Metadata.MaxWidth;
		} else {
			xAdvance = currentCharacter.XAdvance;
		}
		if ( Kerned )
			xAdvance += _font.GetKerning( currentCharacter.Character, nextCharacter );
		return xAdvance * _font.Metadata.InverseMaxWidth * Textscale;
	}

	private Line GetEmptyLine() {
		for ( int i = 0; i < _lines.Count; i++ ) {
			if ( _lines[ i ].Count == 0 )
				return _lines[ i ];
		}
		Line nLine = new();
		_lines.Add( nLine );
		return nLine;
	}
}

public class Word {

	private readonly List<FontCharacter> _characters;
	public float Width { get; private set; }
	public IReadOnlyList<FontCharacter> Characters => _characters;

	public Word() {
		_characters = new List<FontCharacter>();
	}

	public void Clear() {
		_characters.Clear();
		Width = 0;
	}

	public void AddCharacter( FontCharacter fontCharacter, float characterWidth ) {
		_characters.Add( fontCharacter );
		Width += characterWidth;
	}
}

public class Line {

	private readonly List<FontCharacter> _characters;
	public float Width { get; private set; }

	public int Count => _characters.Count;

	public Line() {
		_characters = new List<FontCharacter>();
	}

	public void Clear() {
		_characters.Clear();
		Width = 0;
	}

	public void AddCharacter( FontCharacter fontCharacter, float characterWidth ) {
		_characters.Add( fontCharacter );
		Width += characterWidth;
	}

	public void AddWord( Word word ) {
		_characters.AddRange( word.Characters );
		Width += word.Width;
	}

	public override string ToString() => new( _characters.Select( p => p.Character ).ToArray() );
}

public class FontMetadata : Identifiable {
	public int MaxWidth { get; }
	public float InverseMaxWidth { get; }

	public int MaxHeight { get; }
	public float InverseMaxHeight { get; }

	public FontMetadata( Font font ) {
		int minOffset = int.MaxValue;
		int maxHeight = 0;
		foreach ( FontCharacter c in font.Characters.Values ) {
			if ( c.XAdvance > MaxWidth )
				MaxWidth = c.XAdvance;
			if ( c.Offset.Y < minOffset )
				minOffset = c.Offset.Y;
			if ( c.Size.Y > maxHeight )
				maxHeight = c.Size.Y;
		}
		InverseMaxWidth = 1f / MaxWidth;
		MaxHeight = maxHeight - minOffset;
		InverseMaxHeight = 1f / MaxHeight;
	}
}

public class Font : Identifiable {

	public readonly string FontTexturePath;
	public readonly string FontInformationPath;

	private readonly Dictionary<char, FontCharacter> _characters;
	public IReadOnlyDictionary<char, FontCharacter> Characters => this._characters;

	private readonly Dictionary<char, Dictionary<char, int>> _kerningValues;

	public FontMetadata Metadata { get; }

	public Font( string fontPathWithoutExtension ) : base( fontPathWithoutExtension ) {
		this.FontTexturePath = $"{fontPathWithoutExtension}.png";
		this.FontInformationPath = $"{fontPathWithoutExtension}.fnt";

		this._characters = new();
		this._kerningValues = new();
		ReadFontInformation( out IReadOnlyList<string> characterData, out IReadOnlyList<string> kerningData );
		LoadCharacters( characterData );
		LoadKerning( kerningData );
		Metadata = new( this );
	}

	public int GetKerning( char first, char second ) => this._kerningValues.TryGetValue( first, out var dict ) ? dict.TryGetValue( second, out var value ) ? value : 0 : 0;

	private void ReadFontInformation( out IReadOnlyList<string> characterData, out IReadOnlyList<string> kerningData ) {
		if ( !File.Exists( this.FontInformationPath ) )
			throw new FileNotFoundException( this.FontInformationPath );
		List<string> characters = new();
		List<string> kerning = new();
		string[] lines = File.ReadAllLines( this.FontInformationPath );

		for ( int i = 0; i < lines.Length; i++ ) {
			if ( lines[ i ].StartsWith( "char " ) )
				characters.Add( lines[ i ] );
			if ( lines[ i ].StartsWith( "kerning " ) )
				kerning.Add( lines[ i ] );
		}

		characterData = characters;
		kerningData = kerning;
	}

	private void LoadCharacters( IReadOnlyList<string> characterData ) {
		for ( int i = 0; i < characterData.Count; i++ ) {
			string line = characterData[ i ];
			IEnumerable<string> splitLine = line.Split( ' ' ).Select( p => p.Trim() ).Where( p => p.Length > 0 && p.Contains( '=' ) );
			Dictionary<string, int> data = splitLine.Select( p => p.Split( '=' ) ).ToDictionary( p => p[ 0 ], p => int.Parse( p[ 1 ] ) );
			FontCharacter c = new( (char) data[ "id" ], data[ "x" ], data[ "y" ], data[ "width" ], data[ "height" ], data[ "xoffset" ], data[ "yoffset" ], data[ "xadvance" ] );
			this._characters.Add( c.Character, c );
		}
	}

	private void LoadKerning( IReadOnlyList<string> kerningData ) {
		for ( int i = 0; i < kerningData.Count; i++ ) {
			string line = kerningData[ i ];
			IEnumerable<string> splitLine = line.Split( ' ' ).Select( p => p.Trim() ).Where( p => p.Length > 0 && p.Contains( '=' ) );
			Dictionary<string, int> data = splitLine.Select( p => p.Split( '=' ) ).ToDictionary( p => p[ 0 ], p => int.Parse( p[ 1 ] ) );
			char first = (char) data[ "first" ];
			if ( !this._kerningValues.TryGetValue( first, out Dictionary<char, int>? dict ) )
				this._kerningValues.Add( first, dict = new() );
			dict[ (char) data[ "second" ] ] = data[ "amount" ];
		}
	}
}

public class FontCharacter {
	public readonly Vector2i PixelStart;
	public readonly Vector2i Size;
	public readonly Vector2i Offset;
	/// <summary>
	/// The number of pixels to advance the cursor after this character. This is affected by kerning.
	/// </summary>
	public readonly int XAdvance;
	public readonly char Character;

	public FontCharacter( char character, int x, int y, int sX, int sY, int oX, int oY, int aX ) {
		this.Character = character;
		this.PixelStart = new( x, y );
		this.Size = new( sX, sY );
		this.Offset = new( oX, oY );
		this.XAdvance = aX;
	}
}

public class FontDataManager : Identifiable {

	//LOAD IN REQUESTED FONT DATA, BUT NOT TEXTURE. THE UI ELEMENT WILL HAVE TO REQUEST THAT THEMSELVES.

}
