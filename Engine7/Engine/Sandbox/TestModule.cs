﻿using Engine.Logging;
using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render.Components;
using Engine.Module.Entities.Services;
using Engine.Module.Render;
using Engine.Module.Render.Ogl;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;
using Engine.Module.Render.Ogl.Utilities;
using Engine.Standard;
using Engine.Standard.Entities.Components;
using Engine.Standard.Render;
using Engine.Standard.Render.UserInterface;
using Sandbox.Logic;
using Sandbox.Logic.World;
using Sandbox.Render;

namespace Sandbox;

internal sealed class GameLogicModule : ModuleBase {

	private Entity? _textEntity;
	private Entity? _entity;

	private List<Entity> _players;

	public GameLogicModule() : base( false, 20, "gamelogic" ) {
		_players = [];
		OnInitialize += Init;
		OnUpdate += Update;
	}

	private void Update( double time, double deltaTime ) {
		if (this._entity?.TryGetComponent( out Transform3Component? t3c ) ?? false) {
			t3c.Transform.Translation = new Vector3<double>( Math.Sin( time ) * 0.15f, Math.Cos( time ) * 0.15f, 0 );
			t3c.Transform.Rotation = (Rotor3.FromAxisAngle( Vector3<double>.UnitY, 0.01 ) * t3c.Transform.Rotation).Normalize<Rotor3<double>, double>();
		}
	}

	private void Init() {
		//new DeepWater();
		//ChemicalList.GetMineral( "aBRAKZAN" );
		//Dictionary<Element, List<Chemical>> aaa = ChemicalList._chemicalsContainingElement;
		//Dictionary<string, List<Chemical>> bbb = ChemicalList._chemicalsByTag;

		//var technologyResearcher = new TechnologyResearcher( null, null );

		//LinearTimescaleProvider timescaleProvider = new( new( 0, 70 ), new( 5000, 28 ), new( 7500, 21 ), new( 9000, 14 ), new( 10000, 7 ), new( 10500, 4 ), new( 11000, 2 ), new( 11300, 1 ), new( 11750, 0.5 ), new( 11850, 0.25 ) );
		//GameClock gameClock = new();
		//gameClock.Start( timescaleProvider );

		//List<string> lines = [];
		//double lastDay = gameClock.DaysSinceStart;
		//for (int i = 0; i < 1000000; i++) {
		//	gameClock.Update();
		//	double delta = gameClock.DaysSinceStart - lastDay;
		//	lastDay = gameClock.DaysSinceStart;
		//	lines.Add( $"{i},{gameClock.YearsSinceStart.ToString( "N4", CultureInfo.InvariantCulture ).Replace( ",", "" )},{gameClock.DaysSinceStart.ToString( "N4", CultureInfo.InvariantCulture ).Replace( ",", "" )},{delta.ToString( "N4", CultureInfo.InvariantCulture ).Replace( ",", "" )}" );
		//	if (gameClock.YearsSinceStart > 12025) {
		//		this.LogLine( $"Took {i} ticks to reach the year 2025! Eq. {(double)i / 10:N1}s or {(double) i / (10 * 60 * 60):N1}h" );
		//		break;
		//	}
		//}
		//File.WriteAllLines( ".\\timescale.csv", lines );

		EntityContainer container = this.InstanceProvider.Get<EntityContainerService>().CreateContainer();
		this._entity = container.CreateEntity();
		_entity.AddComponent<RenderComponent>();
		Transform3Component t3c = this._entity.AddComponent<Transform3Component>();
		_entity.AddComponent<GlobeComponent>( p => {
			p.Layers = 7;
			p.SimulatedSurfaceArea = 509600000;
			p.Initialize();
		} );

		_players.Add( CreatePlayerEntity( container ) );

		//this._entity = container.CreateEntity();
		//_entity.AddComponent<RenderComponent>();
		//Transform3Component t3c1 = this._entity.AddComponent<Transform3Component>();
		//t3c1.Transform.Translation = (0, 0.5f, 0);
		//t3c1.Transform.Scale = 1;
		//RenderedTextComponent text1 = _entity.AddComponent<RenderedTextComponent>();
		//text1.Text = "Hello, World!";
		//text1.FontName = "JetBrainsMono-Bold";
		////_entity.AddComponent<TextJumblerComponent>();
		//_entity.AddComponent<WorldComponent>( p => {
		//	p.SimulatedSurfaceArea = 611000000;
		//} );
		//_entity.AddComponent<WorldTilingComponent>();
		//_entity.AddComponent<WorldCameraComponent>();
		//_entity.AddComponent<WorldSelectedTileComponent>();
		//this._textEntity = container.CreateEntity();
		//_textEntity.AddComponent<RenderComponent>();
		//Transform2Component t2c = _textEntity.AddComponent<Transform2Component>();
		//t2c.Transform.Translation = (-0.8, 1 - 0.3);
		//t2c.Transform.Scale = new Vector2<double>( 0.2, 0.2 );
		//t2c.Transform.Rotation = 0;
		//RenderedTextComponent text = _textEntity.AddComponent<RenderedTextComponent>( p => {
		//	p.Text = "Hello, World!";
		//	p.FontName = "JetBrainsMono-Bold";
		//} );
		//_textEntity.AddComponent<UiComponent>();
		//_textEntity.AddComponent<Collider2Component>( p => p.SetBaseVertices( [ (-1, -1), (1, -1), (1, 1), (-1, 1) ] ) );
		//_textEntity.AddComponent<RenderedPrimitive2Component>();
		//_textEntity.AddComponent<ButtonComponent>();
		//TextJumblerComponent aa = _textEntity.AddComponent<TextJumblerComponent>();
		//aa.Offset = 1;
	}

	private Entity CreatePlayerEntity( EntityContainer container ) {
		Entity player = container.CreateEntity();
		player.AddComponent<PlayerComponent>();
		return player;
	}
}

public sealed class TextJumblerComponent : ComponentBase {
	public int Offset { get; set; } = 0;
}

//public sealed class TextJumblerArchetype : ArchetypeBase {
//	public TextJumblerComponent TextJumblerComponent { get; set; }
//	public RenderedTextComponent RenderedTextComponent { get; set; }
//}

//public sealed class TextJumblerSystem : SystemBase<TextJumblerArchetype> {

//	protected override void ProcessEntity( TextJumblerArchetype archetype, double time, double deltaTime ) {
//		string s = "";
//		for (int i = 0; i < 5; i++) {
//			char c = ((archetype.TextJumblerComponent.Offset + i) % 10).ToString()[ 0 ];
//			s += c;
//		}
//		archetype.TextJumblerComponent.Offset++;
//		if (archetype.TextJumblerComponent.Offset > 9)
//			archetype.TextJumblerComponent.Offset = 0;
//		archetype.RenderedTextComponent.Text = s;//time.ToString("n5");
//	}
//}

internal class SandboxRenderModule : RenderModuleBase {

	public SandboxRenderModule() : base( "render" ) {
		OnUpdate += Update;
	}

	protected override void ContextAdded( Context context ) {
		//context.InstanceProvider.Inject( InstanceProvider.Get<GameStateProvider>(), true );
		context.InstanceProvider.Catalog.Host<ContextTest>();
		context.InstanceProvider.Catalog.Host<Render3Pipeline>();
		context.InstanceProvider.Catalog.Host<UserInterfaceRenderPipeline>();
		UserInterfaceService ui = context.InstanceProvider.Get<UserInterfaceService>();
		ui.UserInterfaceStateManager.AddAllElements();
		//ui.UserInterfaceStateManager.AddElement<StartMenu>(); //TODO: Automate. Assume all element types that exists should be included. There are only custom element types.
		//ui.UserInterfaceStateManager.AddElement<TileDataDisplay>();
	}

	private void Update( double time, double deltaTime ) {
		if (InstanceProvider.Get<GameStateProvider>().Get<bool>( "closegame" ))
			InstanceProvider.Get<ContextManagementService>().CloseAll();
	}
}

public sealed class ContextTest : DisposableIdentifiable, IInitializable, IUpdateable {
	private readonly WindowService _windowService;

	public ContextTest( WindowService windowService ) {
		this._windowService = windowService;
	}

	public void Initialize() {
		this.LogLine( "SandboxRenderModule initialized." );
		ContextUtilities.SwapInterval( 1 );
	}

	public void Update( double time, double deltaTime ) {
		//this._windowService.Window.Title = $"Time: {time:#,##0.###}s, DeltaTime: {deltaTime:#,##0.###}s, FPS: {(1 / deltaTime):#,##0.###}f/s";
	}

	protected override bool InternalDispose() {
		return true;
	}
}

//[Identity( nameof( Entity3SceneData ) )]
//[VAO.Setup( 0, 1, 4 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
//public struct Entity3SceneData {
//	[VAO.Data( VertexAttribType.Float, 16 ), FieldOffset( 0 )]
//	public Matrix4x4 ModelMatrix;
//	[VAO.Data( VertexAttribType.UnsignedShort, 4 ), FieldOffset( 64 )]
//	public Color16x4 Color;
//	[VAO.Data( VertexAttribType.UnsignedByte, 1, normalized: true ), FieldOffset( 72 )]
//	public byte NormalMapped;

//	public static Entity3SceneData Interpolate( Entity3SceneData a, Entity3SceneData b, float interpolation ) => new() {
//		ModelMatrix = a.ModelMatrix * interpolation + b.ModelMatrix * (1 - interpolation),
//		Color = (Vector4) a.Color * interpolation + (Vector4) b.Color * (1 - interpolation),
//		NormalMapped = a.NormalMapped
//	};
//}

[Identity( nameof( TestShaderBundle ) )]
public sealed class TestShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) => AddPipeline( "default", pipelineService.Get<TestShaderPipeline>() );
}

public sealed class TestShaderPipeline : OglShaderPipelineBase {
	public override bool UsesTransparency => false;

	protected override IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<TestVertexShaderProgram>();
		yield return shaderProgramService.Get<TestFragmentShaderProgram>();
	}
}

public sealed class TestVertexShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "geometry3.vert" ) );
}
public sealed class TestFragmentShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "test.frag" ) );
}

