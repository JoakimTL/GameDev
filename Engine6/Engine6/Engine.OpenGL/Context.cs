using Engine.OpenGL.OOP;

namespace Engine.OpenGL;

public sealed class Context {

	public ContextWarningLog WarningLog { get; }
	public ViewportState Viewport { get; }
	public FramebufferState Framebuffer { get; }
	public OglWindow Window { get; }

	public Context() {
		WarningLog = new();
		Viewport = new( WarningLog );
		Framebuffer = new( WarningLog );
		Window = new( Viewport,  );
	}

}
