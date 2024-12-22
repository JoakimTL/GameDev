using Engine.Logging;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render.Text.Fonts.Meshing;
using Engine.Standard.Render.Text.Services;
using Engine.Transforms;

namespace Engine.Standard.Render.Text.Typesetting;
public sealed class TextLayout( SceneInstanceCollection<GlyphVertex, Entity2SceneData> sceneInstanceCollection, FontMeshingService fontMeshingService ) : DisposableIdentifiable, IUpdateable {

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
	private float _textScale = 0.1f; //Maybe get pixel scaling?
	private Alignment _horizontalAlignment = Alignment.Negative;
	private Alignment _verticalAlignment = Alignment.Positive;

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

	private bool _needsUpdate;

	public void Update( double time, double deltaTime ) {
		if (!_needsUpdate)
			return;
		_needsUpdate = false;

		if (string.IsNullOrEmpty( _fontName ) || _textArea.GetArea() == 0) {
			for (int i = 0; i < _glyphInstances.Count; i++)
				_glyphInstances[ i ].Dispose();
			_glyphInstances.RemoveAll( p => p.Disposed );
			return;
		}

		//Update the layout and buffer the data.
		MeshedFont meshedFont = _fontMeshingService.Get( _fontName );
		Fonts.Font font = meshedFont.Font;

		int glyphedLetters = 0;
		for (int i = 0; i < _text.Length; i++) {
			char c = _text[ i ];
			if (IsGlyphed( c ))
				glyphedLetters++;
		}

		for (int i = _glyphInstances.Count; i < glyphedLetters; i++)
			_glyphInstances.Add( _sceneInstanceCollection.Create<GlyphInstance>() );

		for (int i = glyphedLetters; i < _glyphInstances.Count; i++)
			_glyphInstances[ i ].Dispose();
		_glyphInstances.RemoveAll( p => p.Disposed );

		//What scale do we work at?
		//We have a text area, which confines the glyphs into a rectangle.
		//We have a text scale, which scales the glyphs.

		//We have rotation which rotates the text area.

		//Let's start the cursor at the appropriate area within the text area...
		//What does text scale mean?
		//It can mean:
		//1. The size of the text in screen space
		//2. The size of the text in text area space
		//	This would mean the text is scaled to fit the text area. A scale of 1 would look different depending on the size of the text area.
		//	How would we then find the sceen space size of the glyph?
		//		We could find the lowest length of the text area axis, and scale by that. This would mean 1/text scale glyphs would fit in the axis.

		//How about we mulitply the cursor by the inverse of the text scale?

		float shortestAxisOfTextArea = Math.Min( _textArea.Maxima.X - _textArea.Minima.X, _textArea.Maxima.Y - _textArea.Minima.Y );
		Vector2<float> realScale = shortestAxisOfTextArea * _textScale;

		float whitespaceSize = font[' ']?.Advance ?? throw new Exception( "Font does not contain whitespace." );
		float lineHeight = font.ScaledLineGap;

		Vector2<float> cursor = (-1, 0);
		int glyphIndex = 0;
		for (int i = 0; i < _text.Length; i++) {
			char c = _text[ i ];
			if (!IsGlyphed( c )) {
				if (c == ' ') {
					cursor += new Vector2<float>( whitespaceSize, 0 ) * TextScale;
				}
				continue;
			}

			GlyphInstance instance = _glyphInstances[ glyphIndex ];
			GlyphMesh? mesh = meshedFont[ c ];
			instance.SetGlyphMesh( mesh );
			glyphIndex++;
			if (mesh is null)
				continue;
			Matrix4x4<float> modelMatrix = Matrix.Create4x4.Scaling( TextScale, TextScale ) * Matrix.Create4x4.Translation( cursor + (mesh.GlyphDefinition.LeftSideBearing * TextScale, 0) );
			if (!instance.SetInstanceData( new Entity2SceneData( modelMatrix ) ))
				this.LogLine( "Failed to write instance data." );
			cursor += new Vector2<float>( mesh.GlyphDefinition.Advance, 0 ) * TextScale;
		}
		//for (int i = 0; i < _glyphInstances.Count; i++) {
		//	if (i >= _text.Length) {
		//		_glyphInstances[ i ].Dispose();
		//		continue;
		//	}

		//	GlyphInstance instance = _glyphInstances[ i ];
		//	GlyphMesh? mesh = meshedFont[ _text[ i ] ];

		//	if (mesh is null) {
		//		x += SpaceSizeEM;
		//		continue;
		//	}

		//	instance.SetGlyphMesh( mesh );
		//	Matrix4x4<float> modelMatrix = (BaseMatrix?.Matrix ?? Matrix4x4<float>.MultiplicativeIdentity) * Matrix.Create4x4.Scaling( 1f / EmsPerLine, 1f / EmsPerLine ) * Matrix.Create4x4.Translation( (x + mesh.GlyphDefinition.LeftSideBearing) / EmsPerLine, 0 );
		//	if (!instance.SetInstanceData( new Entity2SceneData( modelMatrix ) ))
		//		this.LogLine( "Failed to write instance data." );
		//	x += mesh.GlyphDefinition.Advance;

		//}
	}

	private static bool IsGlyphed( char c ) => c != ' ' && c != '\t' && c != '\n' && c != '\r';

	protected override bool InternalDispose() {
		_sceneInstanceCollection.Dispose();
		return true;
	}

}
