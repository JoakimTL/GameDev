using Engine.Logging;
using Engine.Modularity;
using Engine.Module.Render;
using Engine.Module.Render.Ogl.OOP.Buffers;
using OpenGL;

namespace Sandbox;
internal class TestModule : ModuleBase {
	public TestModule() : base( true, 100 ) {
		OnUpdate += Tick;
	}

	private void Tick( double time, double deltaTime ) {
		this.LogLine( $"Time: {time}, DeltaTime: {deltaTime}" );
		if (time > 5)
			Stop();
	}
}
internal class SandboxRenderModule : RenderModuleBase {

	private OglStaticBuffer _vertexBuffer;
	private OglStaticBuffer _elementBuffer;

	public SandboxRenderModule() : base() {
		OnInitialize += Init;
	}

	private void Init() {
		this.LogLine( "SandboxRenderModule initialized." );
		_vertexBuffer = new( BufferUsage.DynamicDraw, 9 * sizeof( float ) );
		_vertexBuffer.WriteRange( [ 0.0f, 0.5f, 0.0f, 0.5f, -0.5f, 0.0f, -0.5f, -0.5f, 0.0f ], 0 );
		_elementBuffer = new( BufferUsage.DynamicDraw, 3 * sizeof( int ) );
		_elementBuffer.WriteRange( [ 0, 1, 2 ], 0 );


	}
}
