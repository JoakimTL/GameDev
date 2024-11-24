using Engine.Module.Render.Glfw;

namespace Engine.Module.Render.Ogl.Utilities;

public static class ContextUtilities {

	public static void SwapInterval( int interval ) => GLFW.SwapInterval( interval );
	public static void MakeContextCurrent( nint window ) => GLFW.MakeContextCurrent( window );

}