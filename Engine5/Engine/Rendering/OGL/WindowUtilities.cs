using GlfwBinding;
using GlfwBinding.Enums;
using GlfwBinding.Structs;

namespace Engine.Rendering.OGL;

public static class WindowUtilities {

	public static bool ShouldWindowClose( nint windowPtr ) => Glfw.WindowShouldClose( windowPtr );
	public static void SetTitle( nint windowPtr, string title ) => Glfw.SetWindowTitle( windowPtr, title );
	public static void SetSize( nint windowPtr, int w, int h ) => Glfw.SetWindowSize( windowPtr, w, h );
	public static void SwapBuffer( nint windowPtr ) => Glfw.SwapBuffers( windowPtr );
	public static void WindowHint( int hint, int value ) => Glfw.WindowHint( hint, value );
	public static void WindowHint( Hint hint, int value ) => Glfw.WindowHint( (int) hint, value );
	public static void WindowHint( int hint, string value ) => Glfw.WindowHintString( hint, value );
	public static void WindowHint( Hint hint, string value ) => Glfw.WindowHintString( (int) hint, value );
	public static void DestroyWindow( nint windowPtr ) => Glfw.DestroyWindow( windowPtr );
	public static void SetWindowAttribute( nint windowPtr, WindowAttribute attribute, bool value ) => Glfw.SetWindowAttribute( windowPtr, (int) attribute, value );
	public static void SetWindowPosition( nint windowPtr, int mx, int my ) => Glfw.SetWindowPosition( windowPtr, mx, my );
	public static nint CreateWindow( int width, int height, string title, nint monitorPtr, nint shareWindowPtr ) => Glfw.CreateWindow( width, height, title, monitorPtr, shareWindowPtr );



	public static void GetMonitorPosition( nint monitorPtr, out int mx, out int my ) => Glfw.GetMonitorPosition( monitorPtr, out mx, out my );
	public static VideoMode GetVideoMode( nint monitorPtr ) => Glfw.GetVideoMode( monitorPtr );
}
