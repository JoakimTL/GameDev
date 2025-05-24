using Engine.Logging;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render.Text.Fonts.Meshing;
using Engine.Standard.Render.Text.Services;

namespace Engine.Standard.Render.Text.Typesetting;
public sealed class TextLayout( SceneInstanceCollection<GlyphVertex, Entity2SceneData> sceneInstanceCollection, FontMeshingService fontMeshingService ) : Identifiable, IUpdateable, IRemovable {

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
		get => this._fontName;
		set {
			if (this._fontName == value)
				return;
			this._fontName = value;
			this._needsUpdate = true;
		}
	}

	public string Text {
		get => this._text;
		set {
			if (this._text == value)
				return;
			this._text = value;
			this._needsUpdate = true;
		}
	}

	public AABB<Vector2<float>> TextArea {
		get => this._textArea;
		set {
			if (this._textArea == value)
				return;
			this._textArea = value;
			this._needsUpdate = true;
		}
	}

	public float TextRotation {
		get => this._textRotation;
		set {
			if (this._textRotation == value)
				return;
			this._textRotation = value;
			this._needsUpdate = true;
		}
	}

	public float TextScale {
		get => this._textScale;
		set {
			if (this._textScale == value)
				return;
			this._textScale = value;
			this._needsUpdate = true;
		}
	}

	public Alignment HorizontalAlignment {
		get => this._horizontalAlignment;
		set {
			if (this._horizontalAlignment == value)
				return;
			this._horizontalAlignment = value;
			this._needsUpdate = true;
		}
	}

	public Alignment VerticalAlignment {
		get => this._verticalAlignment;
		set {
			if (this._verticalAlignment == value)
				return;
			this._verticalAlignment = value;
			this._needsUpdate = true;
		}
	}

	public Vector4<double> Color {
		get => this._color;
		set {
			if (this._color == value)
				return;
			this._color = value;
			this._needsUpdate = true;
		}
	}

	public uint RenderLayer => this._sceneInstanceCollection.RenderLayer;

	public bool Removed { get; private set; }

	private bool _needsUpdate;

	public event RemovalHandler? OnRemoved;

	public void Show() {
		foreach (GlyphInstance instance in this._glyphInstances) {
			instance.SetAllocated( true );
			instance.UpdateInstanceData();
		}
	}

	public void Hide() {
		foreach (GlyphInstance instance in this._glyphInstances)
			instance.SetAllocated( false );
	}

	public void Update( double time, double deltaTime ) {
		if (!this._needsUpdate)
			return;
		this._needsUpdate = false;

		if (string.IsNullOrEmpty( this._fontName ) || this._textArea.GetArea() == 0) {
			for (int i = 0; i < this._glyphInstances.Count; i++)
				this._glyphInstances[ i ].Remove();
			this._glyphInstances.RemoveAll( p => p.Removed );
			return;
		}

		//Update the layout and buffer the data.
		MeshedFont meshedFont = this._fontMeshingService.Get( this._fontName );
		Fonts.Font font = meshedFont.Font;

		float shortestAxisOfTextArea = Math.Min( this._textArea.Maxima.X - this._textArea.Minima.X, (this._textArea.Maxima.Y - this._textArea.Minima.Y) / font.ScaledLineHeight );
		float realScale = shortestAxisOfTextArea * this._textScale;
		float whitespaceSize = (font[ ' ' ]?.Advance ?? 0) * realScale;
		float glyphAscent = font.ScaledAscent * realScale;
		float glyphDescent = font.ScaledDescent * realScale;
		float lineGap = font.ScaledLineGap * realScale;
		float lineHeight = glyphAscent - glyphDescent;

		int wordIndex = 0;
		int index = 0;
		Word currentWord;
		ReadOnlySpan<char> textAsSpan = this._text.AsSpan();
		while (index < this._text.Length) {
			if (Word.IsWordBreak( textAsSpan[ index ] )) {
				index++;
				continue;
			}
			if (wordIndex < this._words.Count)
				currentWord = this._words[ wordIndex ];
			else {
				currentWord = new();
				this._words.Add( currentWord );
			}
			index += currentWord.Read( font, textAsSpan[ index.. ], realScale );
			index++;
			wordIndex++;
		}

		for (int i = this._words.Count - 1; i >= wordIndex; i--)
			this._words.RemoveAt( i );

		wordIndex = 0;
		int lineIndex = 0;
		Line currentLine;
		while (wordIndex < this._words.Count) {
			if (lineIndex < this._lines.Count)
				currentLine = this._lines[ lineIndex ];
			else {
				currentLine = new();
				this._lines.Add( currentLine );
			}
			int wordsRead = currentLine.Read( this._words[ wordIndex.. ], whitespaceSize, this._textArea.Maxima.X - this._textArea.Minima.X );
			if (wordsRead == 0)
				throw new Exception( "Unable to read in any words. Something is wrong with the text logic." );
			wordIndex += wordsRead;
			lineIndex++;
		}

		for (int i = this._lines.Count - 1; i >= lineIndex; i--)
			this._lines.RemoveAt( i );

		int glyphedLetters = 0;
		for (int i = 0; i < this._words.Count; i++)
			glyphedLetters += this._words[ i ].Characters.Length;

		for (int i = this._glyphInstances.Count; i < glyphedLetters; i++)
			this._glyphInstances.Add( this._sceneInstanceCollection.Create<GlyphInstance>() );

		for (int i = glyphedLetters; i < this._glyphInstances.Count; i++)
			this._glyphInstances[ i ].Remove();
		this._glyphInstances.RemoveAll( p => p.Removed );

		int glyphIndex = 0;
		float width = this._textArea.Maxima.X - this._textArea.Minima.X;
		float height = this._textArea.Maxima.Y - this._textArea.Minima.Y;

		float textHeight = lineHeight * this._lines.Count;

		float cursorY = this._textArea.Maxima.Y - glyphAscent;
		if (this.VerticalAlignment == Alignment.Negative) {
			cursorY = this._textArea.Minima.Y + (lineHeight * this._lines.Count) - glyphAscent;
		} else if (this.VerticalAlignment == Alignment.Center) {
			float initialOffset = height * 0.5f;
			float heightOffset = (textHeight * 0.5f) - glyphAscent;

			cursorY = this._textArea.Minima.Y + initialOffset + heightOffset;
		}

		Vector4<ushort> color = (this._color * ushort.MaxValue).Clamp<Vector4<double>, double>( 0, ushort.MaxValue ).CastSaturating<double, ushort>();

		for (int i = 0; i < this._lines.Count; i++) {
			float cursorX = 0;
			cursorX = this.HorizontalAlignment switch {
				Alignment.Negative => this._textArea.Minima.X,
				Alignment.Positive => this._textArea.Maxima.X - this._lines[ i ].ScaledWidth,
				Alignment.Center => this._textArea.Minima.X + ((width - this._lines[ i ].ScaledWidth) * .5f),
				_ => throw new ArgumentOutOfRangeException(),
			};
			Vector2<float> cursor = (cursorX, cursorY);

			Line line = this._lines[ i ];

			foreach (Word word in line.Words) {
				foreach (char c in word.Characters) {
					GlyphInstance instance = this._glyphInstances[ glyphIndex++ ];
					GlyphMesh? mesh = meshedFont[ c ];
					instance.SetGlyphMesh( mesh );
					if (mesh is not null) {
						Matrix4x4<float> modelMatrix = Matrix.Create4x4.Scaling( realScale, realScale ) * Matrix.Create4x4.Translation( cursor + (mesh.GlyphDefinition.LeftSideBearing * realScale, 0) ) * Matrix.Create4x4.RotationZ( this._textRotation );
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

	public void Remove() {
		if (this.Removed)
			return;
		this._sceneInstanceCollection.Remove();
		this.Removed = true;
		OnRemoved?.Invoke( this );
	}
}
