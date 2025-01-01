using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render;
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
	private VertexMesh<VertexSpecial2> _vertexMesh = null!;
	private Scene _scene = null!;
	private SceneInstance<Entity2SceneData> _sceneInstance1 = null!;
	private SceneInstance<Entity2SceneData> _sceneInstance2 = null!;
	private SceneInstance<Entity2SceneData> _sceneInstance3 = null!;

	public void Initialize() {
		if (!this._dataBlockService.CreateUniformBlock( "testUniformBlock", 256, [ ShaderType.VertexShader ], out this._testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!this._dataBlockService.CreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out this._testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		this._dataBlocks = new DataBlockCollection( this._testUniforms, this._testShaderStorage );

		this._vertexMesh = this._meshService.CreateEmptyMesh<VertexSpecial2>( 3, 3 ) ?? throw new NullReferenceException( "Mesh not found." );
		this._vertexMesh.ElementBufferSegment.WriteRange( [ 0, 1, 2 ], 0 );
		this._testVertexArrayObject = this._compositeVertexArrayObjectService.Get( typeof( VertexSpecial2 ), typeof( Entity2SceneData ) ) ?? throw new NullReferenceException( "VertexArrayObject not found." );
		this._shaderBundle = this._shaderBundleService.Get<TestShaderBundle>() ?? throw new NullReferenceException( "ShaderBundle not found." );

		this._scene = this._sceneService.GetScene( "test" );
		this._sceneInstance1 = this._scene.CreateInstance<SceneInstance<Entity2SceneData>>( 0 );
		this._sceneInstance1.SetMesh( this._vertexMesh );
		this._sceneInstance1.SetVertexArrayObject( this._testVertexArrayObject );
		this._sceneInstance1.SetShaderBundle( this._shaderBundle );

		this._sceneInstance2 = this._scene.CreateInstance<SceneInstance<Entity2SceneData>>( 0 );
		this._sceneInstance2.SetMesh( this._meshService.CreateMesh( [
			new VertexSpecial2( (-.5f, -.5f), (255, 255, 0, 255) ),
			new VertexSpecial2( (-.5f, .5f), (255, 0, 255, 255) ),
			new VertexSpecial2( (.5f, .5f), (0, 255, 255, 255) ),
			new VertexSpecial2( (.5f, -.5f), (255, 0, 0, 255) ) ], [ 2, 1, 0, 0, 3, 2 ] ) );
		this._sceneInstance2.SetVertexArrayObject( this._testVertexArrayObject );
		this._sceneInstance2.SetShaderBundle( this._shaderBundle );

		this._sceneInstance3 = this._scene.CreateInstance<SceneInstance<Entity2SceneData>>( 0 );
		this._sceneInstance3.SetMesh( this._meshService.CreateMesh( [
			new VertexSpecial2( (-.5f, -.5f), (255, 255, 0, 255) ),
			new VertexSpecial2( (-.5f, .5f), (255, 0, 255, 255) ),
			new VertexSpecial2( (.5f, .5f), (0, 255, 255, 255) ),
			new VertexSpecial2( (.5f, -.5f), (255, 0, 0, 255) ),
			new VertexSpecial2( (1, 0), (255, 0, 0, 255) ) ], [ 2, 1, 0, 0, 3, 2, 2, 3, 4 ] ) );
		this._sceneInstance3.SetVertexArrayObject( this._testVertexArrayObject );
		this._sceneInstance3.SetShaderBundle( this._shaderBundle );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		this._vertexMesh.VertexBufferSegment.WriteRange( [
			new VertexSpecial2 { Translation = (0.0f, 0.5f), Color = ((byte) (MathF.Sin((float) time) * 100 + 100), 255, 0, 255) },
			new VertexSpecial2 { Translation = (-0.5f, -0.5f), Color = (255, 0, 0, 255) },
			new VertexSpecial2 { Translation = (0.5f, -0.5f), Color = (255, 0, (byte) (MathF.Cos((float) time) * 100 + 100), 255) } ], 0 );
		this._sceneInstance1.Write( new Entity2SceneData( Matrix.Create4x4.RotationZ( -(float) time * 2 ), ushort.MaxValue ) );
		this._sceneInstance2.Write( new Entity2SceneData( Matrix.Create4x4.RotationZ( (float) time * 2 ), ushort.MaxValue ) );
		this._sceneInstance3.Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
		this._testUniforms.Buffer.WriteRange( [ MathF.Cos( (float) time ) / 2 ], 0 );
		this._testShaderStorage.Buffer.WriteRange( [ MathF.Sin( (float) time ) / 2 ], 0 );
	}

	public void DrawToScreen() {
		Gl.Clear( ClearBufferMask.ColorBufferBit );
		this._scene.Render( "default", this._dataBlocks, _ => { }, PrimitiveType.Triangles );
	}

	protected override bool InternalDispose() {
		this._testVertexArrayObject.Dispose();
		return true;
	}
}
