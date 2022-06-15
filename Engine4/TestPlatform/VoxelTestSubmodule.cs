using System.Numerics;
using Engine;
using Engine.Data.Datatypes;
using Engine.Data.Datatypes.Transforms;
using Engine.Modularity.Modules;
using Engine.Modularity.Modules.Submodules;
using Engine.Rendering.Colors;
using Engine.Rendering.Framebuffers;
using Engine.Rendering.InputHandling;
using Engine.Rendering.Pipelines;
using Engine.Rendering.Shaders;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.Scenes.VerletParticles.Systems;
using Engine.Rendering.Standard.UI;
using Engine.Rendering.Standard.UI.Standard;
using Engine.Rendering.Standard.UI.Standard.Text;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;
using Engine.Structure;
using GLFW;
using OpenGL;
using TestPlatform.Verlet;
using TestPlatform.Voxels2.Data.WorldGen;

namespace TestPlatform;

[Engine.Structure.ProcessAfter( typeof( ContextUpdateSubmodule ), typeof( IUpdateable ) )]
public class VoxelTestSubmodule : Submodule {

	private OpenSceneObject<Vertex3, Entity3SceneData>? _origin;
	private Scene? _scene;
	private Render3Pipeline? _pipeline;

	private Voxels2.Data.VoxelWorld? _world;
	private Voxels2.Render.VoxelWorldRenderer? _worldRenderer;

	//private VerletParticleSystem3<VerletParticle>? _verletParticles;

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

		Button b = new Button();
		b.Activate();
		Resources.Render.Get<UIManager>().Add( b );

		TextContainer tc = new( new Font( "res/textures/fonts/calibri" ), "this is a test\nobviously it's not going to work properly at the moment, but we'll get it fixed!\nthe quick brown fox jumps over the lazy dog. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", 0.02f );
		tc.Update();
		Console.WriteLine( string.Join( '\n', tc.Lines.Select(p => p.ToString()) ) );

		Resources.Render.Window.MouseEvents.ButtonPressed += ButtonPress;
		Resources.Render.Window.MouseEvents.ButtonReleased += ButtonRelease;
		Resources.Render.Window.MouseEvents.MovedHidden += MovedHidden;

		this._scene = new LayerlessScene();
		this._origin = new OpenSceneObject<Vertex3, Entity3SceneData>();
		this._origin.SetSceneData( new Engine.Rendering.Standard.SceneInstanceData<Entity3SceneData>( 1, 1 ) );
		Transform3 model1 = new() {
			Translation = new Vector3( 0, 0, 0 ),
			Scale = new Vector3( 4 )
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

		/*this._verletParticles = new VerletParticleSystem3<VerletParticle>( 512, 16 );
		this._verletParticles.SetActiveParticles( 512 );
		this._verletParticles.Updated += VerletConstraints.GlobalUpdate;
		this._verletParticles.SubUpdate += VerletConstraints.AddGravity;
		this._verletParticles.SubUpdate += VerletConstraints.IncreaseTemperature;
		this._verletParticles.SubUpdate += VerletConstraints.TemperatureBleed;
		this._verletParticles.SubUpdate += VerletConstraints.ForceFromTemperature;
		this._verletParticles.SubUpdate += VerletConstraints.KeepInScene;
		this._verletParticles.SubUpdate += VerletConstraints.Collision;
		this._verletParticles.SubUpdate += VerletConstraints.Resolve;
		this._scene.AddSceneObject( this._verletParticles );*/

		this._pipeline.Scenes.Add( new SceneRenderer( this._scene, blendActivationFunction: Transparency ) );
		this._pipeline.Scenes.Add( this._worldRenderer.SceneRender );

	}

	private void Transparency( bool transparent ) {
		if ( transparent ) {
			Gl.Enable( EnableCap.Blend );
			Gl.BlendFunc( GeometryBuffer.TransparencyColorTextureTarget, BlendingFactor.One, BlendingFactor.One );
			Gl.BlendFunc( GeometryBuffer.TransparencyRevealTextureTarget, BlendingFactor.Zero, BlendingFactor.OneMinusSrcColor );
			Gl.BlendEquation( BlendEquationMode.FuncAdd );
			Gl.DepthFunc( DepthFunction.Less );
			Gl.DepthMask( false );
		} else {
			Gl.Disable( EnableCap.Blend );
			Gl.DepthFunc( DepthFunction.Less );
			Gl.DepthMask( true );
		}
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
		//this._world.Update();
		//this._worldRenderer.Update( this._pipeline.View.Translation );
		//this._verletParticles?.Update( time, deltaTime );

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
				this._world?.SetVolumeId( cameraVoxelTranlsation - 10, cameraVoxelTranlsation + 10, ( voxel ) => ( voxel - cameraVoxelTranlsation ).AsFloat.Length() < 10 ? 0 : null );
			}
		}
	}

	private void WindowClosing() => Remove();

	protected override bool OnDispose() => true;
}