using System.Globalization;
using System.Numerics;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.UI.Standard.Text;
/// <summary>
/// This container is only for the text itself. Styling must be done outside.
/// </summary>
public class TextContainer : Identifiable {

	private readonly Font _font;
	private TextGlyphData[] _glyphs;
	private readonly List<Line> _lines;
	private uint _renderedGlyphs;
	public IReadOnlyList<TextGlyphData> Glyphs => this._glyphs;
	public IReadOnlyList<Line> Lines => this._lines;
	public uint RenderedGlyphs => this._renderedGlyphs;
	public float Textscale { get; private set; }
	public string Text { get; private set; }
	public bool Monospaced { get; private set; }
	public bool Kerned { get; private set; }
	public HorizontalAlignment HorizontalAlignment { get; private set; }
	public VerticalAlignment VerticalAlignment { get; private set; }
	public WordWrapMode WordWrapMode { get; private set; }
	public uint MaxCharacters { get; private set; }

	private bool _updated;
	private readonly char _escapeCharacter;

	/// <param name="width">The number of monospaced character a line can fit. Characters can have varying width if not monospaced, which means more can fit in the same line.</param>
	/// <param name="monospaced">If true all letters will have the same width.</param>
	public TextContainer( Font font, string text, float textscale, bool monospaced = false, bool kerned = true, HorizontalAlignment horizontalAlignment = HorizontalAlignment.LEFT, VerticalAlignment verticalAlignment = VerticalAlignment.TOP, WordWrapMode wordWrapMode = WordWrapMode.WrapWords, uint maxCharacters = 512 ) {
		this._font = font;
		this._renderedGlyphs = 0;
		this._glyphs = Array.Empty<TextGlyphData>();
		this._lines = new();
		this.Text = text;
		this.Textscale = textscale;
		this.Monospaced = monospaced;
		this.Kerned = kerned;
		this.HorizontalAlignment = horizontalAlignment;
		this.VerticalAlignment = verticalAlignment;
		this.WordWrapMode = wordWrapMode;
		this.MaxCharacters = maxCharacters;
		this._updated = true;
		this._escapeCharacter = '|';
	}

	public void SetText( string text ) {
		if ( text == this.Text )
			return;
		this.Text = text;
		this._updated = true;
	}

	public void SetTextscale( float textscale ) {
		if ( textscale == this.Textscale )
			return;
		this.Textscale = textscale;
		this._updated = true;
	}

	public void SetMonospaced( bool monospaced ) {
		if ( monospaced == this.Monospaced )
			return;
		this.Monospaced = monospaced;
		this._updated = true;
	}

	public void SetKerning( bool kerned ) {
		if ( kerned == this.Kerned )
			return;
		this.Kerned = kerned;
		this._updated = true;
	}

	public void SetAlignment( HorizontalAlignment horizontalAlignment ) {
		if ( horizontalAlignment == this.HorizontalAlignment )
			return;
		this.HorizontalAlignment = horizontalAlignment;
		this._updated = true;
	}

	public void SetAlignment( VerticalAlignment verticalAlignment ) {
		if ( verticalAlignment == this.VerticalAlignment )
			return;
		this.VerticalAlignment = verticalAlignment;
		this._updated = true;
	}

	public void SetWrapMode( WordWrapMode wordWrapMode ) {
		if ( wordWrapMode == this.WordWrapMode )
			return;
		this.WordWrapMode = wordWrapMode;
		this._updated = true;
	}

	public void SetMaxCharacters( uint maxCharacters ) {
		if ( maxCharacters == this.MaxCharacters )
			return;
		bool shouldUpdate = maxCharacters < this.MaxCharacters;
		this.MaxCharacters = maxCharacters;
		this._updated = shouldUpdate;
	}

	/// <returns>True if updated, false if not.</returns>
	public bool Update() {
		if ( !this._updated )
			return false;
		this._updated = false;

		UpdateLines();
		UpdateGlyphs();

		return true;
	}

	private void UpdateLines() {
		//Turn text into lines of characters to be rendered.
		ReadOnlySpan<char> text = this.Text.AsSpan()[ ..Math.Min( (int) this.MaxCharacters, this.Text.Length ) ];
		this._lines.Clear();
		this._renderedGlyphs = 0;
		Line currentLine = GetEmptyLine();
		Word currentWord = new();
		bool escaped = false;
		Color16x4 currentColor = Color16x4.White;
		float currentRotation = 0;
		Vector2 currentScale = Vector2.One;
		Vector2 currentGlyphData = new( 0.45f, 0.15f );
		bool settingColor = false;
		bool settingRotation = false;
		bool settingScale = false;
		bool settingGlyphData = false;
		string escapeData = "";
		FontCharacter bindCharacter = this._font.Characters[ '-' ];
		for ( int i = 0; i < text.Length; i++ ) {
			char c = text[ i ];
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

			if ( escaped ) {
				if ( c == this._escapeCharacter ) {
					bool madeChanges = false;
					if ( settingColor ) {
						int lengthPerColor = escapeData.Length / 4;
						if ( lengthPerColor == 0 ) {
							this.LogWarning( $"Escape data [{escapeData}] has no data for colors!" );
						} else if ( lengthPerColor > 4 ) {
							this.LogWarning( $"Escape data [{escapeData}] has too much data for colors. Colors can at maximum be represented by a 16-bit integer!" );
						} else {
							if ( uint.TryParse( escapeData[ ..lengthPerColor ], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint red ) &&
								uint.TryParse( escapeData.AsSpan( lengthPerColor, lengthPerColor ), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint green ) &&
								uint.TryParse( escapeData.AsSpan( lengthPerColor * 2, lengthPerColor ), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint blue ) &&
								uint.TryParse( escapeData[ ( lengthPerColor * 3 ).. ], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint alpha )
								) {
								float inverseDivident = 1f / ( ( 1 << ( lengthPerColor * 4 ) ) - 1 );
								currentColor = new Vector4( red, green, blue, alpha ) * inverseDivident;
							}
						}
						settingColor = false;
						madeChanges = true;
					} else if ( settingRotation ) {
						if ( float.TryParse( escapeData, NumberStyles.Float, CultureInfo.InvariantCulture, out float rotation ) ) {
							currentRotation = rotation / 180 * MathF.PI;
						} else {
							this.LogWarning( $"Escape data [{escapeData}] couldn't be parsed as float!" );
						}
						settingRotation = false;
						madeChanges = true;
					} else if ( settingScale ) {
						string[] split = escapeData.Split( ',' );
						bool failed = true;
						if ( split.Length == 1 ) {
							if ( float.TryParse( split[ 0 ], NumberStyles.Float, CultureInfo.InvariantCulture, out float scale ) ) {
								currentScale = new( scale );
								failed = false;
							}
						} else if ( split.Length == 2 ) {
							if ( float.TryParse( split[ 0 ], NumberStyles.Float, CultureInfo.InvariantCulture, out float scaleX ) &&
								 float.TryParse( split[ 1 ], NumberStyles.Float, CultureInfo.InvariantCulture, out float scaleY ) ) {
								currentScale = new( scaleX, scaleY );
								failed = false;
							}
						}
						if ( failed )
							this.LogWarning( $"Escape data [{escapeData}] couldn't be parsed as 2d vector!" );
						settingScale = false;
						madeChanges = true;
					} else if ( settingGlyphData ) {
						string[] split = escapeData.Split( ',' );
						bool failed = true;
						if ( split.Length == 2 )
							if ( float.TryParse( split[ 0 ], NumberStyles.Float, CultureInfo.InvariantCulture, out float thickness ) &&
								 float.TryParse( split[ 1 ], NumberStyles.Float, CultureInfo.InvariantCulture, out float edge ) ) {
								currentGlyphData = new( thickness, edge );
								failed = false;
							}
						if ( failed )
							this.LogWarning( $"Escape data [{escapeData}] couldn't be parsed as 2d vector!" );
						settingGlyphData = false;
						madeChanges = true;
					}
					escapeData = "";
					escaped = false;
					if ( madeChanges )
						continue;
				} else {
					if ( !settingColor && !settingRotation && !settingScale && !settingGlyphData ) {
						switch ( c ) {
							case 'c':
								settingColor = true;
								break;
							case 'r':
								settingRotation = true;
								break;
							case 's':
								settingScale = true;
								break;
							case 'g':
								settingGlyphData = true;
								break;
							default:
								escaped = false;
								break;
						}
					} else {
						escapeData += c;
					}
				}
			} else {
				if ( c == this._escapeCharacter )
					escaped = true;
			}

			if ( !escaped ) {
				char nC = i < text.Length - 1 ? text[ i + 1 ] : '\n';
				FontCharacter fC = this._font.Characters[ c ];

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
						if ( this.WordWrapMode == WordWrapMode.WrapLetter || this.WordWrapMode == WordWrapMode.WrapWords ) {
							if ( currentLine.Width + currentWord.Width >= 0.99998474122f ) 
								currentLine = GetEmptyLine();
						}
						currentLine.AddWord( currentWord );
						currentWord.Clear();
						if ( currentLine.Count > 0 )
							currentLine.AddCharacter( fC, GetWidth( fC, currentScale.X ), GetHeight( currentScale.Y ), false, currentColor, currentRotation, currentScale, currentGlyphData );
						break;
					default:
						float width = GetWidth( fC, currentScale.X );
						if ( this.WordWrapMode == WordWrapMode.WrapLetter || ( currentWord.Width + this.Textscale >= 0.99998474122f && this.WordWrapMode == WordWrapMode.WrapWords ) ) {
							currentWord.AddCharacter( fC, width, GetHeight( currentScale.Y ), currentColor, currentRotation, currentScale, currentGlyphData );
							++this._renderedGlyphs;
							// width added to line, width of current word, width of a monospaced character (the binding sign)
							if ( currentLine.Width + currentWord.Width + this.Textscale >= 0.99998474122f ) {
								if ( currentWord.Characters.Count > 1 ) {
									currentWord.AddCharacter( bindCharacter, GetWidth( bindCharacter, currentScale.X ), GetHeight( currentScale.Y ), currentColor, currentRotation, currentScale, currentGlyphData );
									++this._renderedGlyphs;
									currentLine.AddWord( currentWord );
									currentWord.Clear();
								}
								currentLine = GetEmptyLine();
							}
							break;
						}
						currentWord.AddCharacter( fC, width, GetHeight( currentScale.Y ), currentColor, currentRotation, currentScale, currentGlyphData );
						++this._renderedGlyphs;
						break;
				}
			}
		}
		currentLine.AddWord( currentWord );
	}

	private void UpdateGlyphs() {
		if ( this._renderedGlyphs > this._glyphs.Length )
			this._glyphs = new TextGlyphData[ this._renderedGlyphs ];

		float lineHeight = this.Textscale;
		float cursorY = this.VerticalAlignment switch {
			VerticalAlignment.BOTTOM => this._lines.Count * lineHeight,
			VerticalAlignment.CENTER => this._lines.Count * lineHeight / 2,
			VerticalAlignment.TOP => 0,
			_ => 0,
		};

		int glyph = 0;
		Vector2 inverseFontResolution = this._font.Metadata.InverseResolution;
		for ( int i = 0; i < this._lines.Count; i++ ) {
			Line l = this._lines[ i ];
			float cursorX = this.HorizontalAlignment switch {
				HorizontalAlignment.LEFT => 0,
				HorizontalAlignment.CENTER => -l.Width / 2,
				HorizontalAlignment.RIGHT => -l.Width,
				_ => 0
			};
			for ( int j = 0; j < l.Characters.Count; j++ ) {
				LineCharacter c = l.Characters[ j ];
				if ( c.Rendered ) {
					Vector2 scaling = GetPixelToNDCScale( c.Scale );

					Vector2 position = new( cursorX, cursorY ); //On the cursor line
					Vector2 size = c.FontCharacter.Size * scaling * .5f;
					Vector2 offset = c.FontCharacter.Offset.NegateY() * scaling;
					if ( this.Monospaced ) {
						offset.X += ( c.Width - size.X ) / 2;
					}
					if ( this.Kerned && j > 0 )
						offset.X += this._font.GetKerning( l.Characters[ j - 1 ].FontCharacter.Character, c.FontCharacter.Character ) * scaling.X;
					Vector2 actualPosition = position + size * new Vector2( 1, -1 ) + offset;

					Vector2 uvStart = c.FontCharacter.PixelStart * inverseFontResolution;
					Vector2 uvSize = c.FontCharacter.Size * inverseFontResolution;
					Vector4 uv = new( uvStart.X, uvStart.Y, uvSize.X, uvSize.Y );
					this._glyphs[ glyph++ ] = new TextGlyphData( actualPosition, size, c.Rotation, uv, c.Color, c.GlyphData.X, c.GlyphData.Y );
				}
				cursorX += c.Width;
			}
			cursorY -= l.Height;
		}
	}

	private float GetWidth( FontCharacter currentCharacter, float scaleX ) {
		int xAdvance;
		if ( this.Monospaced ) {
			xAdvance = this._font.Metadata.Max.X * 3 / 5;
		} else {
			xAdvance = currentCharacter.XAdvance;
		}
		return xAdvance * GetPixelXToNDCScale( scaleX );
	}

	private float GetHeight( float scaleY ) => this._font.Metadata.RelativeLineHeight * this.Textscale * scaleY;

	private float GetPixelXToNDCScale( float scale ) => this._font.Metadata.InverseMax * this.Textscale * scale;
	private Vector2 GetPixelToNDCScale( Vector2 scale ) => this._font.Metadata.InverseMax * this.Textscale * scale;

	private Line GetEmptyLine() {
		for ( int i = 0; i < this._lines.Count; i++ ) {
			if ( this._lines[ i ].Count == 0 )
				return this._lines[ i ];
		}
		Line nLine = new();
		this._lines.Add( nLine );
		return nLine;
	}
}
