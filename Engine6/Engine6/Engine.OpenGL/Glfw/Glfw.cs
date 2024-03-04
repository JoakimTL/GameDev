using Engine.OpenGL.Glfw.Enums;
using Engine.OpenGL.Glfw.Structs;
using GlfwBinding;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.OpenGL.Glfw;

public unsafe static class Glfw {
	#region Core
	public static void Init()
		=> GlfwInternal.Init();
	public static void InitHint( int hint, bool value )
		=> GlfwInternal.InitHint( hint, value );
	public static void Terminate()
		=> GlfwInternal.Terminate();
	public static void PollEvents()
		=> GlfwInternal.PollEvents();
	public static void SwapInterval( int interval )
		=> GlfwInternal.SwapInterval( interval );
	#endregion

	#region Window / Monitor
	#region Window
	public static void MakeContextCurrent( nint windowPtr )
		=> GlfwInternal.MakeContextCurrent( windowPtr );
	public static void WindowHintString( int hint, string value )
		=> GlfwInternal.WindowHintString( hint, Encoding.UTF8.GetBytes( value ) );
	public static bool WindowShouldClose( nint windowPtr )
		=> GlfwInternal.WindowShouldClose( windowPtr );
	public static void SetWindowTitle( nint windowPtr, string title )
		=> GlfwInternal.SetWindowTitle( windowPtr, Encoding.UTF8.GetBytes( title ) );
	public static void SetWindowSize( nint windowPtr, int w, int h )
		=> GlfwInternal.SetWindowSize( windowPtr, w, h );
	public static void SwapBuffers( nint windowPtr )
		=> GlfwInternal.SwapBuffers( windowPtr );
	public static void WindowHint( int hint, int value )
		=> GlfwInternal.WindowHint( hint, value );
	public static void DestroyWindow( nint windowPtr )
		=> GlfwInternal.DestroyWindow( windowPtr );
	public static void SetWindowPosition( nint windowPtr, int mx, int my )
		=> GlfwInternal.SetWindowPosition( windowPtr, mx, my );
	public static void SetWindowAttribute( nint windowPtr, int attribute, bool value )
		=> GlfwInternal.SetWindowAttribute( windowPtr, attribute, value );
	public static nint CreateWindow( int width, int height, string title, nint monitorPtr, nint shareWindowPtr )
		=> GlfwInternal.CreateWindow( width, height, Encoding.UTF8.GetBytes( title ), monitorPtr, shareWindowPtr );
	public static int GetWindowAttribute( nint windowPtr, int attribute )
		=> GlfwInternal.GetWindowAttribute( windowPtr, attribute );
	public static void GetWindowSize( nint windowPtr, out int w, out int h )
		=> GlfwInternal.GetWindowSize( windowPtr, out w, out h );
	#endregion
	//--------------------------------------------------------//
	#region Monitor
	public static Vector2 GetMonitorContentScale( nint monitorPtr ) {
		GlfwInternal.GetMonitorContentScale( monitorPtr, out float xScale, out float yScale );
		return new( xScale, yScale );
	}
	public static VideoMode GetVideoMode( nint monitorPtr )
		=> *(VideoMode*) GlfwInternal.GetVideoMode( monitorPtr );
	public static GammaRamp GetGammaRamp( nint monitorPtr )
		=> *(GammaRampInternal*) GlfwInternal.GetGammaRampInternal( monitorPtr );
	public static nint[] GetMonitors() {
		nint ptr = GlfwInternal.GetMonitors( out int count );
		nint[] monitors = new nint[ count ];
		for (int i = 0; i < count; i++)
			monitors[ i ] = *(nint*) (ptr + i * nint.Size);
		return monitors;
	}
	public static void GetMonitorPosition( nint monitorPtr, out int mx, out int my )
		=> GlfwInternal.GetMonitorPosition( monitorPtr, out mx, out my );
	public static nint GetPrimaryMonitor()
		=> GlfwInternal.GetPrimaryMonitor();
	#endregion
	#endregion

	#region Input / Events
	#region Input
	public static Hat GetJoystickHats( int joystickId ) {
		Hat hat = Hat.Centered;
		nint ptr = GlfwInternal.GetJoystickHats( joystickId, out int count );
		for (int i = 0; i < count; i++) {
			byte value = Marshal.ReadByte( ptr, i );
			hat |= (Hat) value;
		}

		return hat;
	}

	public static string? GetJoystickGuid( int joystickId )
		=> Util.PtrToStringUTF8( GlfwInternal.GetJoystickGuid( joystickId ) );
	public static string? GetGamepadName( int gamepadId )
		=> Util.PtrToStringUTF8( GlfwInternal.GetGamepadName( gamepadId ) );
	public static bool UpdateGamepadMappings( string mappings )
		=> GlfwInternal.UpdateGamepadMappings( Encoding.ASCII.GetBytes( mappings ) );
	public static bool RawMouseMotionSupported()
		=> GlfwInternal.RawMouseMotionSupported();
	public static void SetInputMode( nint windowPtr, int inputMode, int value )
		=> GlfwInternal.SetInputMode( windowPtr, inputMode, value );
	public static void GetCursorPosition( nint windowPtr, out double x, out double y )
		=> GlfwInternal.GetCursorPosition( windowPtr, out x, out y );
	#endregion
	//--------------------------------------------------------//
	#region Events
	public static void SetKeyCallback( nint windowPtr, KeyCallback callback )
		=> GlfwInternal.SetKeyCallback( windowPtr, callback );
	public static void SetCharModsCallback( nint windowPtr, CharModsCallback callback )
		=> GlfwInternal.SetCharModsCallback( windowPtr, callback );
	public static void SetDropCallback( nint windowPtr, FileDropCallback callback )
		=> GlfwInternal.SetDropCallback( windowPtr, callback );
	public static void SetWindowFocusCallback( nint windowPtr, FocusCallback callback )
		=> GlfwInternal.SetWindowFocusCallback( windowPtr, callback );
	public static void SetWindowContentScaleCallback( nint windowPtr, WindowContentsScaleCallback callback )
		=> GlfwInternal.SetWindowContentScaleCallback( windowPtr, callback );
	public static void SetWindowMaximizeCallback( nint windowPtr, WindowMaximizedCallback callback )
		=> GlfwInternal.SetWindowMaximizeCallback( windowPtr, callback );
	public static void SetWindowSizeCallback( nint windowPtr, SizeCallback callback )
		=> GlfwInternal.SetWindowSizeCallback( windowPtr, callback );
	public static void SetFramebufferSizeCallback( nint windowPtr, SizeCallback callback )
		=> GlfwInternal.SetFramebufferSizeCallback( windowPtr, callback );
	public static void SetCloseCallback( nint windowPtr, WindowCallback callback )
		=> GlfwInternal.SetCloseCallback( windowPtr, callback );
	public static void SetWindowRefreshCallback( nint windowPtr, WindowCallback callback )
		=> GlfwInternal.SetWindowRefreshCallback( windowPtr, callback );
	public static void SetWindowPositionCallback( nint windowPtr, PositionCallback callback )
		=> GlfwInternal.SetWindowPositionCallback( windowPtr, callback );
	public static void SetCursorEnterCallback( nint windowPtr, MouseEnterCallback callback )
		=> GlfwInternal.SetCursorEnterCallback( windowPtr, callback );
	public static void SetMouseButtonCallback( nint windowPtr, MouseButtonCallback callback )
		=> GlfwInternal.SetMouseButtonCallback( windowPtr, callback );
	public static void SetCursorPositionCallback( nint windowPtr, MouseCallback callback )
		=> GlfwInternal.SetCursorPositionCallback( windowPtr, callback );
	public static void SetScrollCallback( nint windowPtr, MouseCallback callback )
		=> GlfwInternal.SetScrollCallback( windowPtr, callback );
	public static void SetErrorCallback( ErrorCallback errorCallback )
		=> GlfwInternal.SetErrorCallback( errorCallback );
	#endregion

	#endregion
}
