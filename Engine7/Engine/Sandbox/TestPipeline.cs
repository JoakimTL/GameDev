using Engine;
using Engine.Buffers;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using OpenGL;

namespace Sandbox;

public sealed class TestPipeline( DataBlockService dataBlockService, SceneService sceneService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {

	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly SceneService _sceneService = sceneService;

	private UniformBlock _testUniforms = null!;
	private ShaderStorageBlock _testShaderStorage = null!;
	private DataBlockCollection _dataBlocks = null!;
	private Scene _scene = null!;

	public void Initialize() {
		if (!_dataBlockService.CreateUniformBlock( "testUniformBlock", 256, [ ShaderType.VertexShader ], out _testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!_dataBlockService.CreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out _testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		_dataBlocks = new DataBlockCollection( _testUniforms, _testShaderStorage );

		_scene = _sceneService.GetScene( "test" );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		_testUniforms.Buffer.WriteRange( [ MathF.Cos( (float) time ) / 2 ], 0 );
		_testShaderStorage.Buffer.WriteRange( [ MathF.Sin( (float) time ) / 2 ], 0 );

	}

	public void DrawToScreen() {
		Gl.Clear( ClearBufferMask.ColorBufferBit );
		_scene.Render( "default", _dataBlocks, _ => { }, PrimitiveType.Triangles );
	}

	protected override bool InternalDispose() {
		return true;
	}
}
public sealed class TestRenderBehaviour : RenderBehaviourBase {

	private SceneInstance<Entity2SceneData>? _sceneInstance;

	public override void Update( double time, double deltaTime ) {
		if (_sceneInstance is null)
			_sceneInstance = this.RenderEntity.RequestSceneInstance<SceneInstance<Entity2SceneData>>( "test", 0 );
		_sceneInstance.Write( new Entity2SceneData { ModelMatrix = Matrix.Create4x4.RotationZ( -(float) time * 2 ) } );
	}

	protected override bool InternalDispose() {
		return true;
	}
}
