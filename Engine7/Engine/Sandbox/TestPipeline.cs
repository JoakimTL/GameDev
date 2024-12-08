using Engine;
using Engine.Algorithms.Triangulation;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Entities.Components;
using Engine.Standard.Render;
using Engine.Standard.Render.Text;
using Engine.Standard.Render.Text.Fonts;
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
		//_font = _fontService.Get( "JetBrainsMono-Bold" );

		//var g = _font[ 'A' ];
		//g.CreateMeshTriangles();

		if (!this._dataBlockService.CreateUniformBlock( nameof( SceneCameraBlock ), 256, [ ShaderType.VertexShader ], out this._testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!this._dataBlockService.CreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out this._testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		this._dataBlocks = new DataBlockCollection( this._testUniforms, this._testShaderStorage );

		this._view = new() {
			Translation = new( 1, 0, 3 )
		};
		//_view.Rotation = Rotor3.FromAxisAngle(Vector3<float>.UnitY, 0);
		this._projection = new( this._windowService.Window, 90 );
		this._camera = new( this._view, this._projection );
		this._scene = this._sceneService.GetScene( "test" );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		//_view.Translation;
		if (this._camera is null || this._view is null || this._projection is null)
			return;
		this._view.Translation = new( MathF.Sin( (float) time ) * 3, 0, MathF.Cos( (float) time ) * 3 + 5 );
		this._testUniforms.Buffer.Write<uint, SceneCameraBlock>( 0, new SceneCameraBlock( this._camera.Matrix, this._view.Rotation.Up, -this._view.Rotation.Left ) );

	}

	public void DrawToScreen() {
		Gl.Clear( ClearBufferMask.ColorBufferBit );
		this._scene.Render( "default", this._dataBlocks, _ => { }, PrimitiveType.Triangles );
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
		this._sceneInstance = this.RenderEntity.RequestSceneInstance<SceneInstance<Entity2SceneData>>( "test", 0 );
		this._sceneInstance.SetShaderBundle( this.RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<TestShaderBundle>() );
		this._sceneInstance.SetVertexArrayObject( this.RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex2, Entity2SceneData>() );
		this._sceneInstance.SetMesh( this.RenderEntity.ServiceAccess.MeshProvider.CreateMesh(
			[ new Vertex2( (-.5f, -.5f), (255, 255, 0, 255) ),
			new Vertex2( (-.5f, .5f), (255, 0, 255, 255) ),
			new Vertex2( (.5f, .5f), (0, 255, 255, 255) ),
			new Vertex2( (.5f, -.5f), (255, 0, 0, 255) ) ],
			[ 2, 1, 0, 0, 3, 2 ] ) );
	}

	public override void Update( double time, double deltaTime ) {
		base.Update( time, deltaTime );
		this._sceneInstance?.Write( new Entity2SceneData { ModelMatrix = this._transformMatrix } );
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		if (component is Transform2Component t2c) {
			this._preparedTransformMatrix = t2c.Transform.Matrix.CastSaturating<double, float>();
			return true;
		}
		return false;
	}

	protected override void Synchronize() {
		this._transformMatrix = this._preparedTransformMatrix;
	}

	protected override bool InternalDispose() {
		return true;
	}
}

