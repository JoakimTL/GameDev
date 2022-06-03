using System.Numerics;
using Engine;
using Engine.Data.Datatypes;
using Engine.Data.Datatypes.Transforms;
using Engine.Modularity.Modules;
using Engine.Modularity.Modules.Submodules;
using Engine.Rendering.Colors;
using Engine.Rendering.InputHandling;
using Engine.Rendering.Pipelines;
using Engine.Rendering.Shaders;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;
using GLFW;
using TestPlatform.Voxels2.Data.WorldGen;

namespace TestPlatform;

[Engine.Structure.ProcessAfter( typeof( ContextUpdateSubmodule ), typeof( IUpdateable ) )]
public class VoxelTestSubmodule : Submodule {

	private OpenSceneObject<Vertex3, Entity3SceneData>? _origin;
	private Scene? _scene;
	private Render3Pipeline? _pipeline;

	private Voxels2.Data.VoxelWorld? _world;
	private Voxels2.Render.VoxelWorldRenderer? _worldRenderer;

	public VoxelTestSubmodule() : base( true ) {
		OnInitialization += Initialized;
		OnUpdate += Update;
	}


	private void Initialized() {
		Resources.Render.Window.WindowEvents.Closing += WindowClosing;
		this._pipeline = Resources.Render.PipelineManager.Get<Render3Pipeline>();
		if ( this._pipeline is null ) {
			this.LogError( "Pipeline is null, make sure the render module is available!" );
			return;
		}

		Resources.Render.Window.MouseEvents.ButtonPressed += ButtonPress;
		Resources.Render.Window.MouseEvents.ButtonReleased += ButtonRelease;
		Resources.Render.Window.MouseEvents.MovedHidden += MovedHidden;

		this._scene = new LayerlessScene();
		this._origin = new OpenSceneObject<Vertex3, Entity3SceneData>();
		this._origin.SetSceneData( new Engine.Rendering.Standard.SceneInstanceData<Entity3SceneData>( 1, 1 ) );
		Transform3 model1 = new() {
			Translation = new Vector3( 0, 0, 0 ),
			Scale = new Vector3( 0.25f )
		};
		this._origin.SceneData?.SetInstance( 0, new Entity3SceneData() {
			ModelMatrix = model1.Matrix,
			Color = Color16x4.White,
			NormalMapped = 0,
			DiffuseTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			NormalTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			LightingTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			GlowTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect()
		} );
		this._origin.SetMesh( Resources.Render.Mesh3.Cube );
		this._origin.SetShaders( Resources.Render.Shader.Bundles.Get<Entity3ShaderBundle>() );
		this._scene.AddSceneObject( this._origin );

		this._world = new Voxels2.Data.VoxelWorld( new DefaultVoxelWorldGenerator( new Vector3( 12 ), new Vector2( 8 ), 40, 0, 0.3f, -5, 10 )/*, new Engine.Data.Datatypes.AABB3i( -8, 8 )*/ );
		this._worldRenderer = new Voxels2.Render.VoxelWorldRenderer( this._world, 32, 4 );
		this._world.Add( this._worldRenderer );

		this._pipeline.Scenes.Add( new SceneRenderer( this._scene ) );
		this._pipeline.Scenes.Add( this._worldRenderer.SceneRender );

	}

	private void Update( float time, float deltaTime ) {
		if ( this._pipeline is null )
			return;
		float speed = 10;
		if ( Resources.Render.Window.KeyboardEvents[ Keys.LeftShift ] )
			speed *= 10;
		if ( Resources.Render.Window.KeyboardEvents[ Keys.W ] )
			this._pipeline.View.Translation += this._pipeline.View.Rotation.Forward() * deltaTime * speed;
		if ( Resources.Render.Window.KeyboardEvents[ Keys.A ] )
			this._pipeline.View.Translation += this._pipeline.View.Rotation.Left() * deltaTime * speed;
		if ( Resources.Render.Window.KeyboardEvents[ Keys.S ] )
			this._pipeline.View.Translation += this._pipeline.View.Rotation.Backward() * deltaTime * speed;
		if ( Resources.Render.Window.KeyboardEvents[ Keys.D ] )
			this._pipeline.View.Translation += this._pipeline.View.Rotation.Right() * deltaTime * speed;
		if ( Resources.Render.Window.KeyboardEvents[ Keys.Space ] )
			this._pipeline.View.Translation += this._pipeline.View.Rotation.Up() * deltaTime * speed;
		if ( Resources.Render.Window.KeyboardEvents[ Keys.LeftControl ] )
			this._pipeline.View.Translation += this._pipeline.View.Rotation.Down() * deltaTime * speed;

		if ( this._world is null )
			return;
		if ( this._worldRenderer is null )
			return;
		this._world.Update();
		this._worldRenderer.Update( this._pipeline.View.Translation );

		Resources.Render.FrameDebugData.TitleInfo += $"[Avg. draw time: {( Voxels2.Render.VoxelChunkFaceRenderer.DrawTime / Voxels2.Render.VoxelChunkFaceRenderer.DrawCount * 1000 ):N2}ms][{this._worldRenderer.SceneCount},{this._worldRenderer.SceneValidCount},{this._worldRenderer.SceneRenderStages}]/[{this._world?.CHunksInGen}/{this._world?.ChunksInLodQueue}::{this._worldRenderer?.DisplayData}";
	}

	private void MovedHidden( MouseState state ) {
		if ( this._pipeline is null )
			return;
		float speed = 1f / ( MathF.PI * 2 * 100 );
		this._pipeline.View.Rotation = Quaternion.CreateFromAxisAngle( -Vector3.UnitY, ( state.Hidden.Position.X - state.LastHidden.Position.X ) * speed ) * this._pipeline.View.Rotation;
		this._pipeline.View.Rotation = Quaternion.CreateFromAxisAngle( -this._pipeline.View.Rotation.Right(), ( state.Hidden.Position.Y - state.LastHidden.Position.Y ) * speed ) * this._pipeline.View.Rotation;
		this._pipeline.View.Rotation = Quaternion.Normalize( this._pipeline.View.Rotation );
	}

	private void ButtonPress( MouseButton button, ModifierKeys modifiers, MouseState state ) {
		if ( button == MouseButton.Right )
			Resources.Render.Window.MouseEvents.SetLock( true );

	}

	private void ButtonRelease( MouseButton button, ModifierKeys modifiers, MouseState state ) {
		if ( button == MouseButton.Right ) {
			Resources.Render.Window.MouseEvents.SetLock( false );
		}
		if ( button == MouseButton.Left ) {
			if ( this._pipeline is not null && this._world is not null ) {
				Vector3i cameraVoxelTranlsation = this._world.ToVoxelCoordinate( this._pipeline.View.Translation );
				this._world?.SetVolumeId( cameraVoxelTranlsation - 10, cameraVoxelTranlsation + 10, ( voxel ) => (voxel - cameraVoxelTranlsation).AsFloat.Length() < 10 ? 0 : null );
			}
		}
	}

	private void WindowClosing() => Remove();

	protected override bool OnDispose() => true;
}