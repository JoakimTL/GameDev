using Engine;
using Engine.Algorithms.Triangulation;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render.Text;
using OpenGL;

namespace Sandbox;

public sealed class TestPipelineWithScene2( ShaderBundleService shaderBundleService, CompositeVertexArrayObjectService compositeVertexArrayObjectService, DataBlockService dataBlockService, MeshService meshService, SceneService sceneService, FontService fontService, UserInputEventService userInputEventService, WindowService windowService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {
	private readonly ShaderBundleService _shaderBundleService = shaderBundleService;
	private readonly CompositeVertexArrayObjectService _compositeVertexArrayObjectService = compositeVertexArrayObjectService;
	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly MeshService _meshService = meshService;
	private readonly SceneService _sceneService = sceneService;

	private OglVertexArrayObjectBase _testVertexArrayObject = null!;
	private ShaderBundleBase _shaderBundle = null!;
	private UniformBlock _testUniforms = null!;
	private ShaderStorageBlock _testShaderStorage = null!;
	private DataBlockCollection _dataBlocks = null!;
	private Scene _scene = null!;
	private SceneInstance<Entity2SceneData> _sceneInstance1 = null!;
	private SceneInstance<Entity2SceneData> _sceneInstance2 = null!;
	private SceneInstance<Entity2SceneData> _sceneInstance3 = null!;
	private SceneInstance<Entity2SceneData>[] _letters = [];
	private SceneInstance<Entity2SceneData>[] _letterVertices = [];
	private bool _showWithConstraint = false;

	public void Initialize() {
		if (!_dataBlockService.CreateUniformBlock( "testUniformBlock", 256, [ ShaderType.VertexShader ], out _testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!_dataBlockService.CreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out _testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		_dataBlocks = new DataBlockCollection( _testUniforms, _testShaderStorage );

		this._testVertexArrayObject = _compositeVertexArrayObjectService.Get( typeof( LetterVertex ), typeof( Entity2SceneData ) ) ?? throw new NullReferenceException( "VertexArrayObject not found." );
		this._shaderBundle = this._shaderBundleService.Get<TestShaderBundle>() ?? throw new NullReferenceException( "ShaderBundle not found." );

		userInputEventService.OnCharacter += OnCharacterTyped;
		userInputEventService.OnKey += OnKey;

		_scene = _sceneService.GetScene( "test" );

		_showWithConstraint = true;

		Font font = fontService.Get( "JetBrainsMono-Bold" );
		CreateText( font, "ABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅabcdefghijklmnopqrstuvwxyzæøå", _showWithConstraint );

		windowService.Window.Title = $"With constraints: {_showWithConstraint}";
	}

	private void OnKey( KeyboardEvent @event ) {
		if (@event.InputType == TactileInputType.Press && @event.Key == Keys.Space) {
			Font font = fontService.Get( "JetBrainsMono-Bold" );
			CreateText( font, "ABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅabcdefghijklmnopqrstuvwxyzæøå", _showWithConstraint );
		}
		if (@event.InputType == TactileInputType.Press && @event.Key == Keys.F1) {
			_showWithConstraint = !_showWithConstraint;
			windowService.Window.Title = $"With constraints: {_showWithConstraint}";
		}
	}

	private void OnCharacterTyped( KeyboardCharacterEvent @event ) {
		if (@event.Character == ' ')
			return;
		Font font = fontService.Get( "JetBrainsMono-Bold" );
		CreateText( font, @event.Character.ToString(), _showWithConstraint );
	}

	private void CreateText( Font font, string v, bool showWithConstraint ) {
		foreach (SceneInstance<Entity2SceneData> letter in _letters) {
			letter.Dispose();
		}
		foreach (SceneInstance<Entity2SceneData> letterVertices in _letterVertices) {
			letterVertices.Dispose();
		}
		_letters = new SceneInstance<Entity2SceneData>[ v.Where( p => p != ' ' ).Count() ];
		_letterVertices = new SceneInstance<Entity2SceneData>[ _letters.Length ];
		int i = 0;
		foreach (char c in v) {
			if (c == ' ') {
				continue;
			}
			_letters[ i ] = _scene.CreateInstance<SceneInstance<Entity2SceneData>>( 0 );
			_letters[ i ].SetMesh( CreateMesh( font[ c ].CreateMeshTriangles( 0.0025f, showWithConstraint ).ToArray() ) );
			_letters[ i ].SetVertexArrayObject( _testVertexArrayObject );
			_letters[ i ].SetShaderBundle( _shaderBundle );
			_letterVertices[ i ] = _scene.CreateInstance<SceneInstance<Entity2SceneData>>( 1 );
			_letterVertices[ i ].SetMesh( CreateContourIndexMesh( font[ c ].GetPointsInContours(), 0.0025f ) );
			_letterVertices[ i ].SetVertexArrayObject( _testVertexArrayObject );
			_letterVertices[ i ].SetShaderBundle( _shaderBundle );
			i++;
		}
	}

	private VertexMesh<LetterVertex> CreateMesh( (Triangle2<float>, bool filled, bool flipped)[] triangles ) {
		List<LetterVertex> vertices = [];
		List<uint> indices = [];
		Vector4<byte>[] colors = [
			(255, 0, 0, 255),
			(0, 255, 0, 255),
			(0, 0, 255, 255),
			(255, 255, 0, 255),
			(255, 0, 255, 255),
			(0, 255, 255, 255),
			(255, 255, 255, 255),
			(122, 255, 255, 255),
			(255, 122, 255, 255),
			(255, 255, 122, 255),
			(122, 122, 255, 255),
			(122, 255, 122, 255),
			(255, 122, 122, 255),
			(122, 122, 122, 255),
		];
		int i = 0;
		foreach ((Triangle2<float> triangle, bool filled, bool flipped) t in triangles) {
			uint index = (uint) vertices.Count;
			Vector4<byte> color = t.filled
				? new Vector4<byte>( 255, 255, 255, 255 )
				: new Vector4<byte>( 255, 0, 0, 255 );
			vertices.Add( new( t.triangle.A, 0, colors[ i++ ], t.filled, t.flipped ) );
			if (i >= colors.Length)
				i = 0;
			vertices.Add( new( t.triangle.B, (0.5f, 0), colors[ i++ ], t.filled, t.flipped ) );
			if (i >= colors.Length)
				i = 0;
			vertices.Add( new( t.triangle.C, 1, colors[ i++ ], t.filled, t.flipped ) );
			if (i >= colors.Length)
				i = 0;
			indices.Add( index );
			indices.Add( index + 1 );
			indices.Add( index + 2 );
		}
		return this._meshService.CreateMesh<LetterVertex>( vertices.ToArray(), indices.ToArray() );
	}

	public VertexMesh<LetterVertex> CreateContourIndexMesh( (Vector2<float>, uint, bool)[] pointsInContour, float scale ) {
		List<LetterVertex> vertices = [];
		List<uint> indices = [];
		//Create a small box around each point with alternating colours to indicate the order of the points
		Vector4<byte>[] colors = [
			(255, 0, 0, 255),
			(0, 255, 0, 255),
			(0, 0, 255, 255),
		];
		for (int i = 0; i < pointsInContour.Length; i++) {
			uint index = (uint) vertices.Count;
			Vector4<byte> color = pointsInContour[ i ].Item2 == 0 ? (0, 0, 0, 255) : colors[ pointsInContour[ i ].Item2 % colors.Length ];
			Vector4<byte> otherColor = pointsInContour[ i ].Item3 ? (255, 255, 255, 255) : color;
			float size = 0.005f;
			vertices.Add( new( pointsInContour[ i ].Item1 * scale + new Vector2<float>( -size, -size ), 0, otherColor, true, false ) );
			vertices.Add( new( pointsInContour[ i ].Item1 * scale + new Vector2<float>( size, -size ), 0, otherColor, true, false ) );
			vertices.Add( new( pointsInContour[ i ].Item1 * scale + new Vector2<float>( size, size ), 0, color, true, false ) );
			vertices.Add( new( pointsInContour[ i ].Item1 * scale + new Vector2<float>( -size, size ), 0, color, true, false ) );
			indices.Add( index );
			indices.Add( index + 1 );
			indices.Add( index + 2 );
			indices.Add( index );
			indices.Add( index + 2 );
			indices.Add( index + 3 );
		}

		return this._meshService.CreateMesh<LetterVertex>( vertices.ToArray(), indices.ToArray() );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		float scale = MathF.Min( MathF.Max( 0.8f / _letters.Length, 0.12f ), 0.8F );
		float x = _letters.Length > 1 ? -1f : -0.4f;
		float y = _letters.Length > 1 ? 0.75f : -.9f;
		for (int i = 0; i < _letters.Length; i++) {
			_letters[ i ].Write( new Entity2SceneData { ModelMatrix = Matrix.Create4x4.Scaling( scale, scale ) * Matrix.Create4x4.Translation( x, y ) } );
			_letterVertices[ i ].Write( new Entity2SceneData { ModelMatrix = Matrix.Create4x4.Scaling( scale, scale ) * Matrix.Create4x4.Translation( x, y ) } );
			x += scale * 1.5f;
			if (x > 0.825f) {
				x = -0.98f;
				y -= scale * 2;
			}
		}
	}

	public void DrawToScreen() {
		Gl.Clear( ClearBufferMask.ColorBufferBit );
		Gl.Enable( EnableCap.CullFace );
		Gl.CullFace( CullFaceMode.Back );
		_scene.Render( "default", _dataBlocks, _ => { }, PrimitiveType.Triangles );
	}

	protected override bool InternalDispose() {
		this._testVertexArrayObject.Dispose();
		return true;
	}
}