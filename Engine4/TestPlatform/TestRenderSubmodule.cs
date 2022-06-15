using System.Numerics;
using Engine;
using Engine.Data.Datatypes.Transforms;
using Engine.Modularity.Modules;
using Engine.Modularity.Modules.Submodules;
using Engine.Rendering.Colors;
using Engine.Rendering.InputHandling;
using Engine.Rendering.Lighting;
using Engine.Rendering.Lighting.Directional;
using Engine.Rendering.Pipelines;
using Engine.Rendering.Shaders;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.Scenes.Particles;
using Engine.Rendering.Standard.Scenes.Particles.Systems;
using Engine.Rendering.Standard.Scenes.VerletParticles;
using Engine.Rendering.Standard.Scenes.VerletParticles.Systems;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;
using Engine.Structure;
using GLFW;
using TestPlatform.Verlet;

namespace TestPlatform;

[Engine.Structure.ProcessAfter( typeof( ContextUpdateSubmodule ), typeof( IUpdateable ) )]
public class TestRenderSubmodule : Submodule {

	private Render3Pipeline? _pipeline;
	private OpenSceneObject<Vertex3, Entity3SceneData>? _so;
	private OpenSceneObject<Vertex3, Entity3SceneData>? _so2;
	private OpenSceneObject<Vertex3, Entity3SceneData>? _so3;
	private OpenSceneObject<Vertex3, Entity3SceneData>? _so4;
	private ParticleSystem<Vertex2, Particle2Data>? _particles2;
	private ParticleSystem<Vertex2, Particle3Data>? _particles3;
	private DirectionalShadowLightRender? _dslr;

	private LayerlessScene _scene;
	private SceneRenderer _sceneRenderer;
	//private Voxels.World.FiniteVoxelWorld _world;
	//private Voxels.Rendering.VoxelWorldRenderManager _worldRender;
	private Voxels2.Data.VoxelWorld _world;
	private Voxels2.Render.VoxelWorldRenderer _worldRender;
	private VerletParticleSystem3<VerletParticle> _verletParticles;

	//private Voxels.World.FiniteVoxelWorld _world2;
	//private Voxels.Rendering.VoxelWorldRenderManager _worldRender2;

	private OpenSceneObject<Vertex3, Entity3SceneData>[] _frustumPoints;
	private OpenSceneObject<Vertex3, Entity3SceneData>[] _cascades;
	private DirectionalLight _light;
	private readonly PointLight _light2;
	private Random _random;

	public TestRenderSubmodule() : base( true ) {
		OnInitialization += Initialized;
		OnUpdate += Render;
	}

	private void Initialized() {
		Resources.Render.Window.WindowEvents.Closing += WindowClosing;
		this._pipeline = Resources.Render.PipelineManager.Get<Render3Pipeline>();
		if ( this._pipeline is null ) {
			this.LogError( "Pipeline is null, make sure the render module is available!" );
			return;
		}

		this._scene = new LayerlessScene();

		this._so = new OpenSceneObject<Vertex3, Entity3SceneData>();
		this._so.SetMesh( Resources.Render.Mesh3.Cube );
		this._so.SetSceneData( new SceneInstanceData<Entity3SceneData>( 1, 1 ) );
		Transform3 model1 = new();
		model1.Translation = new Vector3( 0, 0, -3.5f );
		model1.Scale = new Vector3( 1, 4, 1 );
		this._so.SceneData[ 0 ] = new Entity3SceneData() {
			ModelMatrix = model1.Matrix,
			Color = Color16x4.White,
			NormalMapped = 0,
			DiffuseTextureHandle = Resources.Render.Textures.Get( "test" ).GetHandleDirect(),
			NormalTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			LightingTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			GlowTextureHandle = Resources.Render.Textures.Get( "test2" ).GetHandleDirect()
		};
		this._so.SetShaders( Resources.Render.Shader.Bundles.Get<Entity3ShaderBundle>() );

		this._so2 = new OpenSceneObject<Vertex3, Entity3SceneData>();
		this._so2.SetMesh( Resources.Render.Mesh3.Cube );
		this._so2.SetSceneData( new SceneInstanceData<Entity3SceneData>( 1, 1 ) );
		Transform3 model2 = new();
		model2.Translation = new Vector3( 1f, 0, -3 );
		model2.Rotation = Quaternion.CreateFromYawPitchRoll( 6, 2, 1.2f );
		this._so2.SceneData[ 0 ] = new Entity3SceneData() {
			ModelMatrix = model2.Matrix,
			Color = new Vector4( 1, 0, 0, 0.2f ),
			NormalMapped = 0,
			DiffuseTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			NormalTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			LightingTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			GlowTextureHandle = Resources.Render.Textures.Black1x1.GetHandleDirect()
		};
		//this._so2.SetShaders( RenderResourceManager.Context.Shader.Bundles.Get<Entity3Shaders>() );
		this._so2.SetShaders( Resources.Render.Shader.Bundles.Get<Entity3TransparencyShaderBundle>() );

		this._so3 = new OpenSceneObject<Vertex3, Entity3SceneData>();
		this._so3.SetMesh( Resources.Render.Mesh3.Cube );
		this._so3.SetSceneData( new SceneInstanceData<Entity3SceneData>( 1, 1 ) );
		Transform3 model3 = new();
		model3.Translation = new Vector3( 1.5f, 0, -2 );
		model3.Scale = new Vector3( 0.5f, 0.5f, 0.5f );
		this._so3.SceneData[ 0 ] = new Entity3SceneData() {
			ModelMatrix = model3.Matrix,
			Color = new Vector4( 0, 0, 1, 0.6f ),
			NormalMapped = 0,
			DiffuseTextureHandle = Resources.Render.Textures.Get( "test" ).GetHandleDirect(),
			NormalTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			LightingTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			GlowTextureHandle = Resources.Render.Textures.Black1x1.GetHandleDirect()
		};
		this._so3.SetShaders( Resources.Render.Shader.Bundles.Get<Entity3ShaderBundle>() );

		this._so4 = new OpenSceneObject<Vertex3, Entity3SceneData>();
		this._so4.SetMesh( Resources.Render.Mesh3.Cube );
		this._so4.SetSceneData( new SceneInstanceData<Entity3SceneData>( 1, 1 ) );
		Transform3 model4 = new();
		model4.Translation = new Vector3( 0, -2, 0 );
		model4.Scale = new Vector3( 90f, 0.5f, 90f );
		this._so4.SceneData[ 0 ] = new Entity3SceneData() {
			ModelMatrix = model4.Matrix,
			Color = new Vector4( 1, 1, 1, 1 ),
			NormalMapped = 0,
			DiffuseTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			NormalTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			LightingTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			GlowTextureHandle = Resources.Render.Textures.Black1x1.GetHandleDirect()
		};
		this._so4.SetShaders( Resources.Render.Shader.Bundles.Get<Entity3ShaderBundle>() );

		this._random = new Random();
		this._particles2 = new Particle2System( 512 );
		this._particles3 = new Particle3System( 512 );
		this._pipeline.Lights.AddLight( this._light = new DirectionalLight( new Vector3( 0.1f, -1, 1 ), new Vector3( 1, 1, 1 ), 1, true ) );
		//this._pipeline.Lights.AddLight( _light2 = new PointLight( new Vector3( 0, 1, -1 ), 3, new Vector3( 1 ), 3, false ) );
		this._scene.AddSceneObject( this._so );
		this._scene.AddSceneObject( this._so2 );
		this._scene.AddSceneObject( this._so3 );
		this._scene.AddSceneObject( this._so4 );
		//this._pipeline._scene.AddSceneObject( this._particles2 );
		//this._pipeline._scene.AddSceneObject( this._particles3 );

		this._sceneRenderer = new SceneRenderer( this._scene );

		//this._world = new( "test", new Vector3i( 64, 64, 64 ), new DefaultVoxelWorldGenerator( new Vector3( 32, 32, 32 ), new Vector2( 32, 32 ), 10, 30, 0.4f, 10, 5 ) );
		//this._world.SetScale( new Vector3( .5f ) );
		//this._world = new Voxels2.Data.InfiniteVoxelWorld();
		this._worldRender = new( this._world, 256, 128 );
		//	this._world2 = new( "test2", 64 );
		//	this._world2.SetScale( new Vector3( 0.25f, 0.25f, 0.25f ) );
		//	this._worldRender2 = new( this._world2, 64 );
		this._pipeline.Scenes.Add( this._sceneRenderer );
		this._pipeline.Scenes.Add( this._worldRender.SceneRender );
		//	this._pipeline.Scenes.Add( this._worldRender2.SceneRender );

		this._pipeline.Perspective.ZFar = 512;

		Resources.Render.Window.MouseEvents.ButtonPressed += ButtonPress;
		Resources.Render.Window.MouseEvents.ButtonReleased += ButtonRelease;
		Resources.Render.Window.MouseEvents.MovedHidden += MovedHidden;
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
		if ( button == MouseButton.Right ) {
			Resources.Render.Window.MouseEvents.SetLock( true );
		}
		if ( button == MouseButton.Left ) {
			if ( this._pipeline is not null ) {
				Vector3 r = this._pipeline.View.Translation;
				//this._world.SetId( r, 0 );
			}
		}

	}

	private void ButtonRelease( MouseButton button, ModifierKeys modifiers, MouseState state ) {
		if ( button == MouseButton.Right ) {
			Resources.Render.Window.MouseEvents.SetLock( false );
		}
	}

	private void Render( float time, float deltaTime ) {
		if ( this._pipeline is null )
			return;
		float speed = 10;
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
		if ( Resources.Render.Window.KeyboardEvents[ Keys.Z ] ) {

			Vector3 r = this._pipeline.View.Translation + (new Vector3( (this._random.NextSingle() * 2) - 1, (this._random.NextSingle() * 2) - 1, (this._random.NextSingle() * 2) - 1 ) * 5);
			//this._world.SetId( r, 0 );
		}

		//this._world.SetTranslation( this._world.Transform.Translation + new Vector3( 0, 0, deltaTime ) );
		//this._world.SetRotation( Quaternion.CreateFromYawPitchRoll( time, 0, 0 ) );
		//this._world.SetScale( new Vector3( MathF.Sin( time ), MathF.Cos( time ), 1 ) );
		//this._worldRender.Update( time, this._pipeline.View.Translation );
		//	this._worldRender2.Update( time, this._pipeline.View.Translation );

		this._dslr = this._pipeline.Lights.DirLight;
		if ( this._dslr is not null ) {
			if ( this._frustumPoints is null ) {
				this._frustumPoints = new OpenSceneObject<Vertex3, Entity3SceneData>[ 8 * CascadeData.NUMCASCADES ];
				for ( int i = 0; i < this._dslr._cascades.Length; i++ ) {
					for ( int j = 0; j < 8; j++ ) {
						this._frustumPoints[ ( i * 8 ) + j ] = new OpenSceneObject<Vertex3, Entity3SceneData>();
						this._frustumPoints[ ( i * 8 ) + j ].SetMesh( Resources.Render.Mesh3.Cube );
						this._frustumPoints[ ( i * 8 ) + j ].SetShaders( Resources.Render.Shader.Bundles.Get<Entity3ShaderBundle>() );
						this._frustumPoints[ ( i * 8 ) + j ].SetSceneData( new SceneInstanceData<Entity3SceneData>( 1, 1 ) );
						this._scene.AddSceneObject( this._frustumPoints[ ( i * 8 ) + j ] );
					}
				}
			}
			if ( this._cascades is null ) {
				this._cascades = new OpenSceneObject<Vertex3, Entity3SceneData>[ this._dslr._cascades.Length ];
				for ( int i = 0; i < this._dslr._cascades.Length; i++ ) {
					this._cascades[ i ] = new OpenSceneObject<Vertex3, Entity3SceneData>();
					this._cascades[ i ].SetMesh( Resources.Render.Mesh3.Cube );
					this._cascades[ i ].SetShaders( Resources.Render.Shader.Bundles.Get<Entity3TransparencyShaderBundle>() );
					this._cascades[ i ].SetSceneData( new SceneInstanceData<Entity3SceneData>( 1, 1 ) );
					this._scene.AddSceneObject( this._cascades[ i ] );
				}
			}
			if ( Resources.Render.Window.KeyboardEvents[ Keys.T ] )
				for ( int i = 0; i < this._dslr._cascades.Length; i++ ) {
					for ( int j = 0; j < 8; j++ ) {
						Transform3 t = new();
						t.Translation = this._dslr._cascades[ i ]._frustumPoints[ j ];
						t.Scale = new Vector3( j >= 4 ? 1 : 4, j >= 4 ? 4 : 1, 0.1f ) * 0.05f;
						t.Rotation = this._dslr.Light.DirectionQuaternion;
						this._frustumPoints[ ( i * 8 ) + j ].SceneData[ 0 ] = new Entity3SceneData {
							ModelMatrix = t.Matrix,
							Color = new Vector4( 1, 1, 1, 1 ),
							NormalMapped = 0,
							DiffuseTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
							NormalTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
							LightingTextureHandle = Resources.Render.Textures.Black1x1.GetHandleDirect(),
							GlowTextureHandle = Resources.Render.Textures.Black1x1.GetHandleDirect()
						};
					}
					Transform3 t2 = new();
					t2.Translation = this._dslr._cascades[ i ].Translation;
					t2.Scale = this._dslr._cascades[ i ].Scale;
					t2.Rotation = this._dslr._cascades[ i ].Rotation;
					this._cascades[ i ].SceneData[ 0 ] = new Entity3SceneData {
						ModelMatrix = t2.Matrix,
						Color = new Vector4( 1, i * 0.25f, MathF.Max( MathF.Sign( i - 2 ), 0 ), 0.2f ),
						NormalMapped = 0,
						DiffuseTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
						NormalTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
						LightingTextureHandle = Resources.Render.Textures.Black1x1.GetHandleDirect(),
						GlowTextureHandle = Resources.Render.Textures.Black1x1.GetHandleDirect()
					};
				}

		}
		//_light.SetDirection( Vector3.Normalize( new Vector3( MathF.Sin( time ), -.4f, MathF.Cos( time ) ) ) );

		/*this._particles3.Add( new Particle3( new Particle3Data() {
			Translation = new Vector3( ( this._random.NextSingle() * 2 ) - 1, ( this._random.NextSingle() * 2 ) - 1, ( this._random.NextSingle() * 2 ) - 1 ),
			Color = new Vector4( this._random.NextSingle(), this._random.NextSingle(), this._random.NextSingle(), this._random.NextSingle() ),
			Scale = new Vector2( ( this._random.NextSingle() * 0.05f ) + 0.025f ),
			Texture1 = RenderResourceManager.Context.Textures.Get( "particle1" ).GetHandleDirect(),
			Texture2 = RenderResourceManager.Context.Textures.Get( "particle2" ).GetHandleDirect(),
		}, time, ( this._random.NextSingle() * 5 ) + 2 ) );
		this._particles2.Update( time, deltaTime );
		this._particles3.Update( time, deltaTime );*/

	}

	private void WindowClosing() => Remove();

	protected override bool OnDispose() => true;
}
