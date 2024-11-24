using Engine.Module.Render.Glfw;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Glfw.Structs;

namespace Engine.Module.Render.Ogl.Utilities;

public static class WindowUtilities {

	public static bool ShouldWindowClose( nint windowPtr ) => GLFW.WindowShouldClose( windowPtr );
	public static void SetTitle( nint windowPtr, string title ) => GLFW.SetWindowTitle( windowPtr, title );
	public static void SetSize( nint windowPtr, int w, int h ) => GLFW.SetWindowSize( windowPtr, w, h );
	public static void SwapBuffer( nint windowPtr ) => GLFW.SwapBuffers( windowPtr );
	public static void WindowHint( int hint, int value ) => GLFW.WindowHint( hint, value );
	public static void WindowHint( Hint hint, int value ) => GLFW.WindowHint( (int) hint, value );
	public static void WindowHint( int hint, string value ) => GLFW.WindowHintString( hint, value );
	public static void WindowHint( Hint hint, string value ) => GLFW.WindowHintString( (int) hint, value );
	public static void DestroyWindow( nint windowPtr ) => GLFW.DestroyWindow( windowPtr );
	public static void SetWindowAttribute( nint windowPtr, WindowAttribute attribute, bool value ) => GLFW.SetWindowAttribute( windowPtr, (int) attribute, value );
	public static void SetWindowPosition( nint windowPtr, int mx, int my ) => GLFW.SetWindowPosition( windowPtr, mx, my );
	public static nint CreateWindow( uint width, uint height, string title, nint monitorPtr, nint shareWindowPtr ) => GLFW.CreateWindow( width, height, title, monitorPtr, shareWindowPtr );
	public static void GetMonitorPosition( nint monitorPtr, out int mx, out int my ) => GLFW.GetMonitorPosition( monitorPtr, out mx, out my );
	public static VideoMode GetVideoMode( nint monitorPtr ) => GLFW.GetVideoMode( monitorPtr );
	public static void GetWindowSize( nint windowPtr, out int w, out int h ) => GLFW.GetWindowSize( windowPtr, out w, out h );
}
