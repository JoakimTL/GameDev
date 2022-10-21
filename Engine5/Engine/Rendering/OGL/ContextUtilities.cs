using GLFW;

namespace Engine.Rendering.OGL;

public static class ContextUtilities {

	public static void SwapInterval( int interval ) => Glfw.SwapInterval( interval );
	public static void MakeContextCurrent( WindowPtr window ) => Glfw.MakeContextCurrent( window );

}
