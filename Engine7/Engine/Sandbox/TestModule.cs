using Engine;
using Engine.Logging;
using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Entities.Services;
using Engine.Module.Render;
using Engine.Module.Render.Ogl;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;
using Engine.Module.Render.Ogl.Utilities;
using Engine.Standard.Entities.Components;

namespace Sandbox;

internal sealed class GameLogicModule : ModuleBase {

	private Entity? _entity;

	public GameLogicModule() : base( false, 120 ) {
		OnInitialize += Init;
		OnUpdate += Update;
	}

	private void Update( double time, double deltaTime ) {
		if (_entity?.TryGetComponent(out Transform2Component? t2c) ?? false) {
			t2c.Transform.Translation = new Vector2<double>( Math.Sin( time ) * 0.5f, Math.Cos( time ) * 0.5f );
		}
	}

	private void Init() {
		EntityContainer container = this.InstanceProvider.Get<EntityContainerService>().CreateContainer();
		_entity = container.CreateEntity();
		//_entity.AddComponent<RenderComponent>();
		_entity.AddComponent<Transform2Component>();
	}
}

internal class SandboxRenderModule : RenderModuleBase {

	protected override void ContextAdded( Context context ) {
		context.InstanceProvider.Catalog.Host<ContextTest>();
		context.InstanceProvider.Catalog.Host<TestPipelineWithScene2>();
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
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "test.vert" ) );
}
public sealed class TestFragmentShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "test.frag" ) );
}
