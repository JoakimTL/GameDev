using Engine.Logging;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render.Text.Fonts.Meshing;
using Engine.Standard.Render.Text.Services;

namespace Engine.Standard.Render.Text.Typesetting;
public sealed class TextLayout( SceneInstanceCollection<GlyphVertex, Entity2SceneData> sceneInstanceCollection, FontMeshingService fontMeshingService ) : Identifiable, IUpdateable {

	//The entire point of this class is to place glyphs in the correct positions, such that they fill a rectangle. The rectangle is defined by the base matrix.

	//A collection of scene instances.
	//TODO Change from disposing everything to disposing only what needs to change. Reuse scene instances if possible.

	private readonly SceneInstanceCollection<GlyphVertex, Entity2SceneData> _sceneInstanceCollection = sceneInstanceCollection;
	private readonly FontMeshingService _fontMeshingService = fontMeshingService;

	private readonly List<GlyphInstance> _glyphInstances = [];

	private string _fontName = string.Empty;
	private string _text = string.Empty;
	private AABB<Vector2<float>> _textArea = AABB.Create<Vector2<float>>( [ -1, 1 ] );
	private float _textRotation = 0;
	private float _textScale = 1f; //Maybe get pixel scaling?
	private Alignment _horizontalAlignment = Alignment.Negative;
	private Alignment _verticalAlignment = Alignment.Positive;
	private Vector4<double> _color = (1, 1, 1, 1);
	private readonly List<Word> _words = [];
	private readonly List<Line> _lines = [];

	public string FontName {
		get => _fontName;
		set {
			if (_fontName == value)
				return;
			_fontName = value;
			_needsUpdate = true;
		}
	}

	public string Text {
		get => _text;
		set {
			if (_text == value)
				return;
			_text = value;
			_needsUpdate = true;
		}
	}

	public AABB<Vector2<float>> TextArea {
		get => _textArea;
		set {
			if (_textArea == value)
				return;
			_textArea = value;
			_needsUpdate = true;
		}
	}

	public float TextRotation {
		get => _textRotation;
		set {
			if (_textRotation == value)
				return;
			_textRotation = value;
			_needsUpdate = true;
		}
	}

	public float TextScale {
		get => _textScale;
		set {
			if (_textScale == value)
				return;
			_textScale = value;
			_needsUpdate = true;
		}
	}

	public Alignment HorizontalAlignment {
		get => _horizontalAlignment;
		set {
			if (_horizontalAlignment == value)
				return;
			_horizontalAlignment = value;
			_needsUpdate = true;
		}
	}

	public Alignment VerticalAlignment {
		get => _verticalAlignment;
		set {
			if (_verticalAlignment == value)
				return;
			_verticalAlignment = value;
			_needsUpdate = true;
		}
	}

	public Vector4<double> Color {
		get => _color;
		set {
			if (_color == value)
				return;
			_color = value;
			_needsUpdate = true;
		}
	}

	private bool _needsUpdate;

	public void Show() {
		foreach (GlyphInstance instance in _glyphInstances) {
			instance.SetAllocated( true );
			instance.UpdateInstanceData();
		}
	}

	public void Hide() {
		foreach (GlyphInstance instance in _glyphInstances)
			instance.SetAllocated( false );
	}

	public void Update( double time, double deltaTime ) {
		if (!_needsUpdate)
			return;
		_needsUpdate = false;

		if (string.IsNullOrEmpty( _fontName ) || _textArea.GetArea() == 0) {
			for (int i = 0; i < _glyphInstances.Count; i++)
				_glyphInstances[ i ].Remove();
			_glyphInstances.RemoveAll( p => p.Removed );
			return;
		}

		//Update the layout and buffer the data.
		MeshedFont meshedFont = _fontMeshingService.Get( _fontName );
		Fonts.Font font = meshedFont.Font;

		float shortestAxisOfTextArea = Math.Min( _textArea.Maxima.X - _textArea.Minima.X, (_textArea.Maxima.Y - _textArea.Minima.Y) / font.ScaledLineHeight );
		float realScale = shortestAxisOfTextArea * _textScale;
		float whitespaceSize = (font[ ' ' ]?.Advance ?? 0) * realScale;
		float glyphAscent = font.ScaledAscent * realScale;
		float glyphDescent = font.ScaledDescent * realScale;
		float lineGap = font.ScaledLineGap * realScale;
		float lineHeight = glyphAscent - glyphDescent;

		int wordIndex = 0;
		int index = 0;
		Word currentWord;
		ReadOnlySpan<char> textAsSpan = _text.AsSpan();
		while (index < _text.Length) {
			if (Word.IsWordBreak( textAsSpan[ index ] )) {
				index++;
				continue;
			}
			if (wordIndex < _words.Count)
				currentWord = _words[ wordIndex ];
			else {
				currentWord = new();
				_words.Add( currentWord );
			}
			index += currentWord.Read( font, textAsSpan[ index.. ], realScale );
			index++;
			wordIndex++;
		}

		for (int i = _words.Count - 1; i >= wordIndex; i--)
			_words.RemoveAt( i );

		wordIndex = 0;
		int lineIndex = 0;
		Line currentLine;
		while (wordIndex < _words.Count) {
			if (lineIndex < _lines.Count)
				currentLine = _lines[ lineIndex ];
			else {
				currentLine = new();
				_lines.Add( currentLine );
			}
			int wordsRead = currentLine.Read( _words[ wordIndex.. ], whitespaceSize, _textArea.Maxima.X - _textArea.Minima.X );
			if (wordsRead == 0)
				throw new Exception( "Unable to read in any words. Something is wrong with the text logic." );
			wordIndex += wordsRead;
			lineIndex++;
		}

		for (int i = _lines.Count - 1; i >= lineIndex; i--)
			_lines.RemoveAt( i );

		int glyphedLetters = 0;
		for (int i = 0; i < _words.Count; i++)
			glyphedLetters += _words[ i ].Characters.Length;

		for (int i = _glyphInstances.Count; i < glyphedLetters; i++)
			_glyphInstances.Add( _sceneInstanceCollection.Create<GlyphInstance>() );

		for (int i = glyphedLetters; i < _glyphInstances.Count; i++)
			_glyphInstances[ i ].Remove();
		_glyphInstances.RemoveAll( p => p.Removed );

		int glyphIndex = 0;
		float width = _textArea.Maxima.X - _textArea.Minima.X;
		float height = _textArea.Maxima.Y - _textArea.Minima.Y;

		float textHeight = lineHeight * _lines.Count;

		float cursorY = _textArea.Maxima.Y - glyphAscent;
		if (VerticalAlignment == Alignment.Negative) {
			cursorY = _textArea.Minima.Y + lineHeight * _lines.Count - glyphAscent;
		} else if (VerticalAlignment == Alignment.Center) {
			float initialOffset = height * 0.5f;
			float heightOffset = textHeight * 0.5f - glyphAscent;

			cursorY = _textArea.Minima.Y + initialOffset + heightOffset;
		}

		Vector4<ushort> color = (_color * ushort.MaxValue).Clamp<Vector4<double>, double>( 0, ushort.MaxValue ).CastSaturating<double, ushort>();

		for (int i = 0; i < _lines.Count; i++) {
			float cursorX = 0;
			cursorX = HorizontalAlignment switch {
				Alignment.Negative => _textArea.Minima.X,
				Alignment.Positive => _textArea.Maxima.X - _lines[ i ].ScaledWidth,
				Alignment.Center => _textArea.Minima.X + (width - _lines[ i ].ScaledWidth) * .5f,
				_ => throw new ArgumentOutOfRangeException(),
			};
			Vector2<float> cursor = (cursorX, cursorY);

			Line line = _lines[ i ];

			foreach (Word word in line.Words) {
				foreach (char c in word.Characters) {
					GlyphInstance instance = _glyphInstances[ glyphIndex++ ];
					GlyphMesh? mesh = meshedFont[ c ];
					instance.SetGlyphMesh( mesh );
					if (mesh is not null) {
						Matrix4x4<float> modelMatrix = Matrix.Create4x4.Scaling( realScale, realScale ) * Matrix.Create4x4.Translation( cursor + (mesh.GlyphDefinition.LeftSideBearing * realScale, 0) ) * Matrix.Create4x4.RotationZ( _textRotation );
						if (!instance.SetInstanceData( new Entity2SceneData( modelMatrix, color ) ))
							this.LogLine( "Failed to write instance data." );
						cursor += new Vector2<float>( mesh.GlyphDefinition.Advance, 0 ) * realScale;
					}
				}
				cursor += new Vector2<float>( whitespaceSize, 0 );
			}

			cursorY -= lineHeight;
		}
	}

}
