using Engine;
using Engine.Logging;
using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Entities.Services;
using Engine.Module.Render;
using Engine.Module.Render.Ogl;
using Engine.Module.Render.Ogl.OOP.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Services;
using Engine.Module.Render.Ogl.Utilities;
using OpenGL;
using System.Runtime.InteropServices;

namespace Sandbox;

internal sealed class GameLogicModule : ModuleBase {

	public GameLogicModule() : base( false, 50 ) {
		OnInitialize += Init;
	}

	private void Init() {
		EntityContainer container = this.InstanceProvider.Get<EntityContainerService>().CreateContainer();
		Entity entity = container.CreateEntity();
		entity.AddComponent<RenderComponent>();
	}
}

internal class SandboxRenderModule : RenderModuleBase {

	protected override void ContextAdded( Context context ) {
		context.InstanceProvider.Catalog.Host<ContextTest>();
		context.InstanceProvider.Catalog.Host<TestPipeline>();
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
		this._windowService.Window.Title = $"Time: {time:#,##0.###}s, DeltaTime: {deltaTime:#,##0.###}s, FPS: {(1 / deltaTime):#,##0.###}f/s";
	}

	protected override bool InternalDispose() {
		return true;
	}
}

//public sealed class TestVertexArrayObject : OglVertexArrayObjectBase {
//	private readonly OglBufferBase _vertexBuffer;
//	private readonly OglBufferBase _elementBuffer;

//	public TestVertexArrayObject( OglBufferBase vertexBuffer, OglBufferBase elementBuffer ) {
//		this._vertexBuffer = vertexBuffer;
//		this._elementBuffer = elementBuffer;
//	}
//	protected override void Setup() {
//		uint binding = BindBuffer( this._vertexBuffer.BufferId, 0, 2 * sizeof( float ) );
//		this.SetupAttrib( binding, 0, 2, VertexAttribType.Float, false, 0 );
//		SetElementBuffer( this._elementBuffer );
//	}
//}

[Identity( nameof( Vertex2 ) )]
[VAO.Setup( 0, 0, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Vertex2( Vector2<float> translation, Vector4<byte> color ) {
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 0 )]
	public Vector2<float> Translation = translation;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 8 )]
	public Vector4<byte> Color = color;
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
