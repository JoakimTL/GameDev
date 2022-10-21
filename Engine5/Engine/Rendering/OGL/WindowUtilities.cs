using GLFW;

namespace Engine.Rendering.OGL;

public static class WindowUtilities {

	public static bool ShouldWindowClose( WindowPtr window ) => Glfw.WindowShouldClose( window );
	public static void SetTitle( WindowPtr window, string title ) => Glfw.SetWindowTitle( window, title );
	public static void SetSize( WindowPtr window, int w, int h ) => Glfw.SetWindowSize( window, w, h );
	public static void SwapBuffer( WindowPtr window ) => Glfw.SwapBuffers( window );
	public static void WindowHint( int hint, int value ) => WindowHint( (Hint) hint, value );
	public static void WindowHint( Hint hint, int value ) => Glfw.WindowHint( hint, value );
	public static void DestroyWindow( WindowPtr window ) => Glfw.DestroyWindow( window );

}
