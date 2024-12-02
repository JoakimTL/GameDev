using Engine;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Entities.Components;
using Engine.Standard.Render;
using Engine.Standard.Render.Text;
using Engine.Transforms.Camera;
using OpenGL;
using System.Runtime.InteropServices;

namespace Sandbox;

public sealed class TestPipeline( WindowService windowService, DataBlockService dataBlockService, SceneService sceneService, FontService fontService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {
	private readonly WindowService _windowService = windowService;
	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly SceneService _sceneService = sceneService;
	private readonly FontService _fontService = fontService;
	private UniformBlock _testUniforms = null!;
	private ShaderStorageBlock _testShaderStorage = null!;
	private DataBlockCollection _dataBlocks = null!;
	private Scene _scene = null!;
	private View3? _view;
	private Perspective.Dynamic? _projection;
	private Camera? _camera;
	private Font? _font;

	public void Initialize() {
		_font = _fontService.Get( "JetBrainsMono-Bold" );

		if (!_dataBlockService.CreateUniformBlock( nameof( SceneCameraBlock ), 256, [ ShaderType.VertexShader ], out _testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!_dataBlockService.CreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out _testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		_dataBlocks = new DataBlockCollection( _testUniforms, _testShaderStorage );

		_view = new() {
			Translation = new( 1, 0, 3 )
		};
		//_view.Rotation = Rotor3.FromAxisAngle(Vector3<float>.UnitY, 0);
		_projection = new( _windowService.Window, 90 );
		_camera = new( _view, _projection );
		_scene = _sceneService.GetScene( "test" );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		//_view.Translation;
		if (_camera is null || _view is null || _projection is null)
			return;
		_view.Translation = new( MathF.Sin( (float) time ) * 3, 0, MathF.Cos( (float) time ) * 3 + 5 );
		_testUniforms.Buffer.Write<uint, SceneCameraBlock>( 0, new SceneCameraBlock( _camera.Matrix, _view.Rotation.Up, -_view.Rotation.Left ) );

	}

	public void DrawToScreen() {
		Gl.Clear( ClearBufferMask.ColorBufferBit );
		_scene.Render( "default", _dataBlocks, _ => { }, PrimitiveType.Triangles );
	}

	protected override bool InternalDispose() {
		return true;
	}
}

public sealed class RenderArchetype : ArchetypeBase {
	public RenderComponent RenderComponent { get; set; } = null!;
	public Transform2Component Transform2Component { get; set; } = null!;
}

public sealed class TestRenderBehaviour : SynchronizedRenderBehaviourBase<RenderArchetype> {

	private SceneInstance<Entity2SceneData>? _sceneInstance;

	private Matrix4x4<float> _preparedTransformMatrix = Matrix4x4<float>.MultiplicativeIdentity;
	private Matrix4x4<float> _transformMatrix = Matrix4x4<float>.MultiplicativeIdentity;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
		_sceneInstance = this.RenderEntity.RequestSceneInstance<SceneInstance<Entity2SceneData>>( "test", 0 );
		_sceneInstance.SetShaderBundle( this.RenderEntity.RequestShaderBundle<TestShaderBundle>() );
		_sceneInstance.SetVertexArrayObject( this.RenderEntity.RequestCompositeVertexArray<Vertex2, Entity2SceneData>() );
		_sceneInstance.SetMesh( this.RenderEntity.RequestNewMesh(
			[ new Vertex2( (-.5f, -.5f), (255, 255, 0, 255) ),
			new Vertex2( (-.5f, .5f), (255, 0, 255, 255) ),
			new Vertex2( (.5f, .5f), (0, 255, 255, 255) ),
			new Vertex2( (.5f, -.5f), (255, 0, 0, 255) ) ],
			[ 2, 1, 0, 0, 3, 2 ] ) );
	}

	public override void Update( double time, double deltaTime ) {
		base.Update( time, deltaTime );
		_sceneInstance?.Write( new Entity2SceneData { ModelMatrix = _transformMatrix } );
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		if (component is Transform2Component t2c) {
			_preparedTransformMatrix = t2c.Transform.Matrix.CastSaturating<double, float>();
			return true;
		}
		return false;
	}

	protected override void Synchronize() {
		_transformMatrix = _preparedTransformMatrix;
	}

	protected override bool InternalDispose() {
		return true;
	}
}

//TODO: Add camera (done?)
//TODO: Add render pipeline
//TODO: Add input (done?)
//TODO: Add input to RenderEntity
//TODO: Add sound (https://github.com/naudio/NAudio ?)
//TODO: Partial icosphere
//TODO: Add text rendering
//TODO: Add GUI?

//Game stuff:
//TODO: Add world entity and render the partial icosphere
//TODO: Add tiles
//TODO: Add players
//TODO: Add items
//TODO: Add population and needs
//TODO: Add structures
//TODO: Add resources
//TODO: Add trade
//TODO: Add tech tree/research
//TODO: Add culture / religion
//TODO: Add politics
//TODO: Add spheres of influence

//TODO: Add armies
//TODO: Add combat

//TODO: Add diplomacy

