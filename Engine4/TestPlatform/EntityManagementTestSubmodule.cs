//using System.Numerics;
//using Engine;
//using Engine.Data.Datatypes.Transforms;
//using Engine.Modularity.ECS;
//using Engine.Modularity.ECS.Networking;
//using Engine.Modularity.ECS.Organization;
//using Engine.Rendering.Colors;
//using Engine.Rendering.InputHandling;
//using Engine.Rendering.Pipelines;
//using Engine.Rendering.Shaders;
//using Engine.Rendering.Standard.Scenes;
//using Engine.Rendering.Standard.VertexArrayObjects.Layouts;
//using GLFW;

//namespace TestPlatform;

//[Engine.Structure.ProcessAfter( typeof( ContextUpdateSubmodule ), typeof( IUpdateable ) )]
//public class EntityManagementTestSubmodule : Submodule {

//	private Render3Pipeline? _pipeline;
//	private LayerlessScene? _scene;
//	private SceneRenderer? _sceneRenderer;
//	private EntityScene3Manager? _entityScene3Manager;
//	private OpenSceneObject<Vertex3, Entity3SceneData>? _origin;
//	//private OpenSceneObject<Vertex3, Entity3SceneData>? _map;

//	public EntityManagementTestSubmodule() : base( true ) {
//		OnInitialization += Initialized;
//		OnUpdate += Render;
//	}

//	private void Initialized() {
//		Resources.Render.Window.WindowEvents.Closing += WindowClosing;
//		this._pipeline = Resources.Render.PipelineManager.GetOrAdd<Render3Pipeline>();
//		if ( this._pipeline is null ) {
//			this.LogError( "Pipeline is null, make sure the render module is available!" );
//			return;
//		}
//		Singleton<EntityNetworkManager>();
//		this._scene = new LayerlessScene();
//		this._entityScene3Manager = new EntityScene3Manager( Singleton<EntityManager>(), this._scene );
//		this._sceneRenderer = new SceneRenderer( this._scene );
//		this._pipeline.Scenes.Add( this._sceneRenderer );

//		this._origin = new OpenSceneObject<Vertex3, Entity3SceneData>();
//		this._origin.SetSceneData( new Engine.Rendering.Standard.SceneInstanceData<Entity3SceneData>( 1, 1 ) );
//		Transform3 model1 = new();
//		model1.Translation = new Vector3( 0, 0, 0 );
//		model1.Scale = new Vector3( 1, 1, 1 );
//		this._origin.SceneData[ 0 ] = new Entity3SceneData() {
//			ModelMatrix = model1.Matrix,
//			Color = Color16x4.White,
//			NormalMapped = 0,
//			DiffuseTextureHandle = Resources.Render.Textures.Get( "test" ).GetHandleDirect(),
//			NormalTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
//			LightingTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
//			GlowTextureHandle = Resources.Render.Textures.Get( "test2" ).GetHandleDirect()
//		};
//		this._origin.SetMesh( Resources.Render.Mesh3.Cube );
//		this._origin.SetShaders( Resources.Render.Shader.Bundles.Get<Entity3ShaderBundle>() );

//		/*FiniteHeightmap heightmap = new FiniteHeightmap( 4, 0, 512, "res/textures/heightmap2.png" );
//		this._map = new OpenSceneObject<Vertex3, Entity3SceneData>();
//		this._map.SetSceneData( new Engine.Rendering.Standard.SceneInstanceData<Entity3SceneData>( 1, 1 ) );
//		this._map.SceneData[ 0 ] = new Entity3SceneData() {
//			ModelMatrix = Matrix4x4.Identity,
//			Color = Color16x4.White,
//			NormalMapped = 0,
//			DiffuseTextureHandle = RenderResourceManager.Context.Textures.Get( "test" ).GetHandleDirect(),
//			NormalTextureHandle = RenderResourceManager.Context.Textures.White1x1.GetHandleDirect(),
//			LightingTextureHandle = RenderResourceManager.Context.Textures.White1x1.GetHandleDirect(),
//			GlowTextureHandle = RenderResourceManager.Context.Textures.Get( "test2" ).GetHandleDirect()
//		};
//		this._map.SetMesh( heightmap.CreateMesh(1) );
//		this._map.SetShaders( RenderResourceManager.Context.Shader.Bundles.Get<Entity3ShaderBundle>() );*/

//		this._scene.AddSceneObject( this._origin );
//		//this._scene.AddSceneObject( this._map );
//		//this._pipeline.Lights.AddLight( new DirectionalLight( new Vector3( 0.1f, -1, -.4f ), new Vector3( 1, 1, 1 ), 10, true ) );

//		Resources.Render.Window.MouseEvents.ButtonPressed += ButtonPress;
//		Resources.Render.Window.MouseEvents.ButtonReleased += ButtonRelease;
//		Resources.Render.Window.MouseEvents.MovedHidden += MovedHidden;
//	}
//	private void MovedHidden( MouseState state ) {
//		if ( this._pipeline is null )
//			return;
//		float speed = 1f / ( MathF.PI * 2 * 100 );
//		this._pipeline.View.Rotation = Quaternion.CreateFromAxisAngle( -Vector3.UnitY, ( state.Hidden.Position.X - state.LastHidden.Position.X ) * speed ) * this._pipeline.View.Rotation;
//		this._pipeline.View.Rotation = Quaternion.CreateFromAxisAngle( -this._pipeline.View.Rotation.Right(), ( state.Hidden.Position.Y - state.LastHidden.Position.Y ) * speed ) * this._pipeline.View.Rotation;
//		this._pipeline.View.Rotation = Quaternion.Normalize( this._pipeline.View.Rotation );
//	}

//	private void ButtonPress( MouseButton button, ModifierKeys modifiers, MouseState state ) {
//		if ( button == MouseButton.Right ) {
//			Resources.Render.Window.MouseEvents.SetLock( true );
//		}
//	}

//	private void ButtonRelease( MouseButton button, ModifierKeys modifiers, MouseState state ) {
//		if ( button == MouseButton.Right ) {
//			Resources.Render.Window.MouseEvents.SetLock( false );
//		}
//	}

//	private void Render( float time, float deltaTime ) {
//		if ( this._pipeline is null )
//			return;
//		float speed = 10;
//		if ( Resources.Render.Window.KeyboardEvents[ Keys.LeftShift ] )
//			speed *= 10;
//		if ( Resources.Render.Window.KeyboardEvents[ Keys.W ] )
//			this._pipeline.View.Translation += this._pipeline.View.Rotation.Forward() * deltaTime * speed;
//		if ( Resources.Render.Window.KeyboardEvents[ Keys.A ] )
//			this._pipeline.View.Translation += this._pipeline.View.Rotation.Left() * deltaTime * speed;
//		if ( Resources.Render.Window.KeyboardEvents[ Keys.S ] )
//			this._pipeline.View.Translation += this._pipeline.View.Rotation.Backward() * deltaTime * speed;
//		if ( Resources.Render.Window.KeyboardEvents[ Keys.D ] )
//			this._pipeline.View.Translation += this._pipeline.View.Rotation.Right() * deltaTime * speed;
//		if ( Resources.Render.Window.KeyboardEvents[ Keys.Space ] )
//			this._pipeline.View.Translation += this._pipeline.View.Rotation.Up() * deltaTime * speed;
//		if ( Resources.Render.Window.KeyboardEvents[ Keys.LeftControl ] )
//			this._pipeline.View.Translation += this._pipeline.View.Rotation.Down() * deltaTime * speed;
//		this._entityScene3Manager.Update();
//	}

//	private void WindowClosing() => Remove();

//	protected override bool OnDispose() => true;
//}
