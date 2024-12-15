using Engine.Logging;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render.Text.Fonts.Meshing;
using Engine.Transforms;

namespace Engine.Standard.Render.Text.Typesetting;
public sealed class TextLayout( SceneInstanceCollection<GlyphVertex, Entity2SceneData> sceneInstanceCollection, FontMeshingService fontMeshingService ) : DisposableIdentifiable, IUpdateable {

	//A collection of scene instances.
	//TODO Change from disposing everything to disposing only what needs to change. Reuse scene instances if possible.

	private readonly SceneInstanceCollection<GlyphVertex, Entity2SceneData> _sceneInstanceCollection = sceneInstanceCollection;
	private readonly FontMeshingService _fontMeshingService = fontMeshingService;

	private readonly List<GlyphInstance> _glyphInstances = [];

	private string _fontName = string.Empty;
	private string _text = string.Empty;
	private float _emsPerLine = 30;

	//private string _incomingFontName = string.Empty;
	//private string _incomingText = string.Empty;

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

	public float EmsPerLine {
		get => _emsPerLine;
		set {
			if (_emsPerLine == value)
				return;
			_emsPerLine = value;
			_needsUpdate = true;
		}
	}

	private IMatrixProvider<float>? _baseMatrix;
	public IMatrixProvider<float>? BaseMatrix { get => _baseMatrix; set => SetBaseMatrix( value ); }

	private bool _needsUpdate;

	private void SetBaseMatrix( IMatrixProvider<float>? value ) {
		if (_baseMatrix == value)
			return;
		if (_baseMatrix is not null)
			_baseMatrix.OnMatrixChanged -= OnMatrixChanged;
		_baseMatrix = value;
		if (_baseMatrix is not null)
			_baseMatrix.OnMatrixChanged += OnMatrixChanged;
		_needsUpdate = true;
	}

	private void OnMatrixChanged( IMatrixProvider<float> provider ) => _needsUpdate = true;

	public void Update( double time, double deltaTime ) {
		if (!_needsUpdate)
			return;
		_needsUpdate = false;

		if (string.IsNullOrEmpty( _fontName )) {
			for (int i = 0; i < _glyphInstances.Count; i++)
				_glyphInstances[ i ].Dispose();
			return;
		}

		//Update the layout and buffer the data.
		MeshedFont meshedFont = _fontMeshingService.Get( _fontName );
		Fonts.Font font = meshedFont.Font;

		//Care for glyphs with no mesh.
		for (int i = _glyphInstances.Count; i < _text.Length; i++)
			_glyphInstances.Add( _sceneInstanceCollection.Create<GlyphInstance>() );

		const float SpaceSizeEM = 0.333f;
		const float LineHeightEM = 1.3f;

		float x = -1;
		float y = 0;

		for (int i = 0; i < _glyphInstances.Count; i++) {
			if (i >= _text.Length) {
				_glyphInstances[ i ].Dispose();
				continue;
			}

			GlyphInstance instance = _glyphInstances[ i ];
			GlyphMesh? mesh = meshedFont[ _text[ i ] ];

			if (mesh is null) {
				x += SpaceSizeEM;
				continue;
			}

			instance.SetGlyphMesh( mesh );
			Matrix4x4<float> modelMatrix = Matrix.Create4x4.Scaling( 2f / EmsPerLine, 2f / EmsPerLine ) * Matrix.Create4x4.Translation( x / EmsPerLine, 0 ) * (BaseMatrix?.Matrix ?? Matrix4x4<float>.MultiplicativeIdentity);
			if (!instance.SetInstanceData( new Entity2SceneData( modelMatrix ) ))
				this.LogLine( "Failed to write instance data." );
			x += SpaceSizeEM * 4;

		}

		_glyphInstances.RemoveAll( p => p.Disposed );
	}

	protected override bool InternalDispose() {
		_sceneInstanceCollection.Dispose();
		return true;
	}

}

public sealed class GlyphInstance : SceneInstanceCollection<GlyphVertex, Entity2SceneData>.InstanceBase {
	internal void SetGlyphMesh( GlyphMesh? mesh ) => base.SetMesh( mesh );
	internal bool SetInstanceData( Entity2SceneData data ) => base.Write( data );
}