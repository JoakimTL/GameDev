using Engine;
using Engine.Logging;
using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Services;
using Engine.Module.Render;
using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.OOP.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
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
		InstanceProvider.Get<EntityContainerService>().CreateContainer();
	}
}

internal class SandboxRenderModule : RenderModuleBase {

	public SandboxRenderModule() : base() {
		OnInitialize += Init;
	}

	private void Init() {
		Gl.DebugMessageCallback( ( source, type, id, severity, length, message, userParam ) => {
			string msg = Marshal.PtrToStringAnsi( message, length );
			this.LogLine( $"OpenGL: {msg}" );
		}, IntPtr.Zero );
		InstanceProvider.Get<ContextManagementService>().Contexts[ 0 ].InstanceProvider.Catalog.Host<ContextTest>();
	}

}

public sealed class ContextTest : DisposableIdentifiable, IInitializable, IUpdateable {
	private readonly ShaderBundleService _shaderBundleService;
	private readonly WindowService _windowService;
	private OglStaticBuffer _vertexBuffer;
	private OglStaticBuffer _elementBuffer;
	private TestVertexArrayObject _testVertexArrayObject;
	private ShaderBundleBase _shaderBundle;

	public ContextTest( ShaderBundleService shaderBundleService, WindowService windowService ) {
		this._shaderBundleService = shaderBundleService;
		this._windowService = windowService;
		this._vertexBuffer = null!;
		this._elementBuffer = null!;
		this._testVertexArrayObject = null!;
		this._shaderBundle = null!;
	}

	public void Initialize() {
		this.LogLine( "SandboxRenderModule initialized." );
		ContextUtilities.SwapInterval( 1 );
		_vertexBuffer = new( BufferUsage.DynamicDraw, 6 * sizeof( float ) );
		_vertexBuffer.WriteRange( [ 0.0f, 0.5f, 0.5f, -0.5f, -0.5f, -0.5f ], 0 );
		_elementBuffer = new( BufferUsage.DynamicDraw, 3 * sizeof( int ) );
		_elementBuffer.WriteRange( [ 0, 1, 2 ], 0 );
		_testVertexArrayObject = new( _vertexBuffer, _elementBuffer );
		_shaderBundle = _shaderBundleService.Get<TestShaderBundle>() ?? throw new NullReferenceException( "ShaderBundle not found." );
	}

	public void Update( double time, double deltaTime ) {
		_windowService.Window.Title = $"Time: {time:#,##0.###}s, DeltaTime: {deltaTime:#,##0.###}s, FPS: {(1 / deltaTime):#,##0.###}f/s";
		_vertexBuffer.WriteRange( [ MathF.Sin( (float) time ), 0.5f, 0.5f, -0.5f, -0.5f, -0.5f ], 0 );
		Gl.Clear( ClearBufferMask.ColorBufferBit );
		_shaderBundle.Get( "default" )!.Bind();
		_testVertexArrayObject.Bind();
		Gl.DrawElements( PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0 );
		OglShaderPipelineBase.Unbind();
		OglVertexArrayObjectBase.Unbind();
	}

	protected override bool InternalDispose() {
		_vertexBuffer.Dispose();
		_elementBuffer.Dispose();
		_testVertexArrayObject.Dispose();
		return true;
	}
}

public sealed class TestVertexArrayObject : OglVertexArrayObjectBase {
	private readonly OglBufferBase _vertexBuffer;
	private readonly OglBufferBase _elementBuffer;

	public TestVertexArrayObject( OglBufferBase vertexBuffer, OglBufferBase elementBuffer ) {
		this._vertexBuffer = vertexBuffer;
		this._elementBuffer = elementBuffer;
	}
	protected override void Setup() {
		uint binding = BindBuffer( _vertexBuffer.BufferId, 0, 2 * sizeof( float ) );
		this.SetupAttrib( binding, 0, 2, VertexAttribType.Float, false, 0 );
		SetElementBuffer( _elementBuffer );
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
