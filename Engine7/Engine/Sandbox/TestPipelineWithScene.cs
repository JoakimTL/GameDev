using Engine;
using Engine.Algorithms.Triangulation;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render.Text;
using OpenGL;

namespace Sandbox;

public sealed class TestPipelineWithScene( ShaderBundleService shaderBundleService, CompositeVertexArrayObjectService compositeVertexArrayObjectService, DataBlockService dataBlockService, MeshService meshService, SceneService sceneService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {
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
	private VertexMesh<Vertex2> _vertexMesh = null!;
	private Scene _scene = null!;
	private SceneInstance<Entity2SceneData> _sceneInstance1 = null!;
	private SceneInstance<Entity2SceneData> _sceneInstance2 = null!;
	private SceneInstance<Entity2SceneData> _sceneInstance3 = null!;

	public void Initialize() {
		if (!_dataBlockService.CreateUniformBlock( "testUniformBlock", 256, [ ShaderType.VertexShader ], out _testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!_dataBlockService.CreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out _testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		_dataBlocks = new DataBlockCollection( _testUniforms, _testShaderStorage );

		_vertexMesh = this._meshService.CreateEmptyMesh<Vertex2>( 3, 3 ) ?? throw new NullReferenceException( "Mesh not found." );
		_vertexMesh.ElementBufferSegment.WriteRange( [ 0, 1, 2 ], 0 );
		this._testVertexArrayObject = _compositeVertexArrayObjectService.Get( typeof( Vertex2 ), typeof( Entity2SceneData ) ) ?? throw new NullReferenceException( "VertexArrayObject not found." );
		this._shaderBundle = this._shaderBundleService.Get<TestShaderBundle>() ?? throw new NullReferenceException( "ShaderBundle not found." );

		_scene = _sceneService.GetScene( "test" );
		_sceneInstance1 = _scene.CreateInstance<SceneInstance<Entity2SceneData>>( 0 );
		_sceneInstance1.SetMesh( _vertexMesh );
		_sceneInstance1.SetVertexArrayObject( _testVertexArrayObject );
		_sceneInstance1.SetShaderBundle( _shaderBundle );

		_sceneInstance2 = _scene.CreateInstance<SceneInstance<Entity2SceneData>>( 0 );
		_sceneInstance2.SetMesh( this._meshService.CreateMesh( [
			new Vertex2( (-.5f, -.5f), (255, 255, 0, 255) ),
			new Vertex2( (-.5f, .5f), (255, 0, 255, 255) ),
			new Vertex2( (.5f, .5f), (0, 255, 255, 255) ),
			new Vertex2( (.5f, -.5f), (255, 0, 0, 255) ) ], [ 2, 1, 0, 0, 3, 2 ] ) );
		_sceneInstance2.SetVertexArrayObject( _testVertexArrayObject );
		_sceneInstance2.SetShaderBundle( _shaderBundle );

		_sceneInstance3 = _scene.CreateInstance<SceneInstance<Entity2SceneData>>( 0 );
		_sceneInstance3.SetMesh( this._meshService.CreateMesh( [
			new Vertex2( (-.5f, -.5f), (255, 255, 0, 255) ),
			new Vertex2( (-.5f, .5f), (255, 0, 255, 255) ),
			new Vertex2( (.5f, .5f), (0, 255, 255, 255) ),
			new Vertex2( (.5f, -.5f), (255, 0, 0, 255) ),
			new Vertex2( (1, 0), (255, 0, 0, 255) ) ], [ 2, 1, 0, 0, 3, 2, 2, 3, 4 ] ) );
		_sceneInstance3.SetVertexArrayObject( _testVertexArrayObject );
		_sceneInstance3.SetShaderBundle( _shaderBundle );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		this._vertexMesh.VertexBufferSegment.WriteRange( [
			new Vertex2 { Translation = (0.0f, 0.5f), Color = ((byte) (MathF.Sin((float) time) * 100 + 100), 255, 0, 255) },
			new Vertex2 { Translation = (-0.5f, -0.5f), Color = (255, 0, 0, 255) },
			new Vertex2 { Translation = (0.5f, -0.5f), Color = (255, 0, (byte) (MathF.Cos((float) time) * 100 + 100), 255) } ], 0 );
		_sceneInstance1.Write( new Entity2SceneData { ModelMatrix = Matrix.Create4x4.RotationZ( -(float) time * 2 ) } );
		_sceneInstance2.Write( new Entity2SceneData { ModelMatrix = Matrix.Create4x4.RotationZ( (float) time * 2 ) } );
		_sceneInstance3.Write( new Entity2SceneData { ModelMatrix = Matrix4x4<float>.MultiplicativeIdentity } );
		_testUniforms.Buffer.WriteRange( [ MathF.Cos( (float) time ) / 2 ], 0 );
		_testShaderStorage.Buffer.WriteRange( [ MathF.Sin( (float) time ) / 2 ], 0 );
	}

	public void DrawToScreen() {
		Gl.Clear( ClearBufferMask.ColorBufferBit );
		_scene.Render( "default", _dataBlocks, _ => { }, PrimitiveType.Triangles );
	}

	protected override bool InternalDispose() {
		this._testVertexArrayObject.Dispose();
		return true;
	}
}
public sealed class TestPipelineWithScene2( ShaderBundleService shaderBundleService, CompositeVertexArrayObjectService compositeVertexArrayObjectService, DataBlockService dataBlockService, MeshService meshService, SceneService sceneService, FontService fontService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {
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

	public void Initialize() {
		if (!_dataBlockService.CreateUniformBlock( "testUniformBlock", 256, [ ShaderType.VertexShader ], out _testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!_dataBlockService.CreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out _testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		_dataBlocks = new DataBlockCollection( _testUniforms, _testShaderStorage );

		this._testVertexArrayObject = _compositeVertexArrayObjectService.Get( typeof( Vertex2 ), typeof( Entity2SceneData ) ) ?? throw new NullReferenceException( "VertexArrayObject not found." );
		this._shaderBundle = this._shaderBundleService.Get<TestShaderBundle>() ?? throw new NullReferenceException( "ShaderBundle not found." );

		//Triangle2<float> t = new( (0, 0), (1, 0), (0, 1) );
		//var c = t.GetCircumcircle();

		Span<Vector2<float>> points =
		[
			new( 2, 2 ),
			new( 2, -2 ),
			new( -2, -2 ),
			new( -2, 2 ),
			new( 1, 1 ),
			new( 1, -1 ),
			new( -1, -1 ),
			new( -1, 1 ),
		];

		var a = Delaunay.Triangulate( points );

		_scene = _sceneService.GetScene( "test" );
		_sceneInstance1 = _scene.CreateInstance<SceneInstance<Entity2SceneData>>( 0 );
		_sceneInstance1.SetMesh( CreateMesh( a.ToArray() ) );
		_sceneInstance1.SetVertexArrayObject( _testVertexArrayObject );
		_sceneInstance1.SetShaderBundle( _shaderBundle );

		Font font = fontService.Get( "JetBrainsMono-Bold" );
		var b = font[ 'B' ];
		var bm = b.CreateMeshTriangles(0.005f);

		_sceneInstance2 = _scene.CreateInstance<SceneInstance<Entity2SceneData>>( 0 );
		_sceneInstance2.SetMesh( CreateMesh( bm.Select( p => p.Item1 ).ToArray() ) );
		_sceneInstance2.SetVertexArrayObject( _testVertexArrayObject );
		_sceneInstance2.SetShaderBundle( _shaderBundle );

		//_sceneInstance3 = _scene.CreateInstance<SceneInstance<Entity2SceneData>>( 0 );
		//_sceneInstance3.SetMesh( this._meshService.CreateMesh( [
		//	new Vertex2( (-.5f, -.5f), (255, 255, 0, 255) ),
		//	new Vertex2( (-.5f, .5f), (255, 0, 255, 255) ),
		//	new Vertex2( (.5f, .5f), (0, 255, 255, 255) ),
		//	new Vertex2( (.5f, -.5f), (255, 0, 0, 255) ) ], [ 2, 1, 0, 0, 3, 2 ] ) );
		//_sceneInstance3.SetVertexArrayObject( _testVertexArrayObject );
		//_sceneInstance3.SetShaderBundle( _shaderBundle );

	}

	private VertexMesh<Vertex2> CreateMesh( Triangle2<float>[] triangles ) {
		List<Vertex2> vertices = [];
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
		foreach (var triangle in triangles) {
			uint index = (uint) vertices.Count;
			Vector2<float> center = 0;//(triangle.A + triangle.B + triangle.C) / 3;
			vertices.Add( new( triangle.A + center, colors[ i ] ) );
			vertices.Add( new( triangle.B + center, colors[ i ] ) );
			vertices.Add( new( triangle.C + center, colors[ i ] ) );
			indices.Add( index );
			indices.Add( index + 1 );
			indices.Add( index + 2 );
			i++;
			if (i >= colors.Length)
				i = 0;
		}
		return this._meshService.CreateMesh<Vertex2>( vertices.ToArray(), indices.ToArray() );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		_sceneInstance1.Write( new Entity2SceneData { ModelMatrix = Matrix.Create4x4.Scaling( 0.1f, 0.1f ) * Matrix.Create4x4.Translation( -.5f, 0f ) } );
		_sceneInstance2.Write( new Entity2SceneData { ModelMatrix = Matrix.Create4x4.Scaling( 0.1f, 0.1f ) * Matrix.Create4x4.Translation( .25f, -.25f ) } );
		//_sceneInstance3.Write( new Entity2SceneData { ModelMatrix = Matrix4x4<float>.MultiplicativeIdentity } );

	}

	public void DrawToScreen() {
		Gl.Clear( ClearBufferMask.ColorBufferBit );
		_scene.Render( "default", _dataBlocks, _ => { }, PrimitiveType.Triangles );
	}

	protected override bool InternalDispose() {
		this._testVertexArrayObject.Dispose();
		return true;
	}
}