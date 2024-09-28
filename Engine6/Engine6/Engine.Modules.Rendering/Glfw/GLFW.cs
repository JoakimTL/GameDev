using Engine.Modules.Rendering.Glfw;
using Engine.Modules.Rendering.Glfw.Enums;
using Engine.Modules.Rendering.Glfw.Structs;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.Modules.Rendering.Ogl.Glfw;

public unsafe static class GLFW {
	#region Core
	public static void Init()
		=> GLFWInternal.Init();
	public static void InitHint( int hint, bool value )
		=> GLFWInternal.InitHint( hint, value );
	public static void Terminate()
		=> GLFWInternal.Terminate();
	public static void PollEvents()
		=> GLFWInternal.PollEvents();
	public static void SwapInterval( int interval )
		=> GLFWInternal.SwapInterval( interval );
	#endregion

	#region Window / Monitor
	#region Window
	public static void MakeContextCurrent( nint windowPtr )
		=> GLFWInternal.MakeContextCurrent( windowPtr );
	public static void WindowHintString( int hint, string value )
		=> GLFWInternal.WindowHintString( hint, Encoding.UTF8.GetBytes( value ) );
	public static bool WindowShouldClose( nint windowPtr )
		=> GLFWInternal.WindowShouldClose( windowPtr );
	public static void SetWindowTitle( nint windowPtr, string title )
		=> GLFWInternal.SetWindowTitle( windowPtr, Encoding.UTF8.GetBytes( title ) );
	public static void SetWindowSize( nint windowPtr, int w, int h )
		=> GLFWInternal.SetWindowSize( windowPtr, w, h );
	public static void SwapBuffers( nint windowPtr )
		=> GLFWInternal.SwapBuffers( windowPtr );
	public static void WindowHint( int hint, int value )
		=> GLFWInternal.WindowHint( hint, value );
	public static void DestroyWindow( nint windowPtr )
		=> GLFWInternal.DestroyWindow( windowPtr );
	public static void SetWindowPosition( nint windowPtr, int mx, int my )
		=> GLFWInternal.SetWindowPosition( windowPtr, mx, my );
	public static void SetWindowAttribute( nint windowPtr, int attribute, bool value )
		=> GLFWInternal.SetWindowAttribute( windowPtr, attribute, value );
	public static nint CreateWindow( uint width, uint height, string title, nint monitorPtr, nint shareWindowPtr )
		=> GLFWInternal.CreateWindow( (int) width, (int) height, Encoding.UTF8.GetBytes( title ), monitorPtr, shareWindowPtr );
	public static int GetWindowAttribute( nint windowPtr, int attribute )
		=> GLFWInternal.GetWindowAttribute( windowPtr, attribute );
	public static void GetWindowSize( nint windowPtr, out int w, out int h )
		=> GLFWInternal.GetWindowSize( windowPtr, out w, out h );
	#endregion
	//--------------------------------------------------------//
	#region Monitor
	public static Vector2 GetMonitorContentScale( nint monitorPtr ) {
		GLFWInternal.GetMonitorContentScale( monitorPtr, out float xScale, out float yScale );
		return new( xScale, yScale );
	}
	public static VideoMode GetVideoMode( nint monitorPtr )
		=> *(VideoMode*) GLFWInternal.GetVideoMode( monitorPtr );
	public static GammaRamp GetGammaRamp( nint monitorPtr )
		=> *(GammaRampInternal*) GLFWInternal.GetGammaRampInternal( monitorPtr );
	public static nint[] GetMonitors() {
		nint ptr = GLFWInternal.GetMonitors( out int count );
		nint[] monitors = new nint[ count ];
		for (int i = 0; i < count; i++)
			monitors[ i ] = *(nint*) (ptr + (i * nint.Size));
		return monitors;
	}
	public static void GetMonitorPosition( nint monitorPtr, out int mx, out int my )
		=> GLFWInternal.GetMonitorPosition( monitorPtr, out mx, out my );
	public static nint GetPrimaryMonitor()
		=> GLFWInternal.GetPrimaryMonitor();
	#endregion
	#endregion

	#region Input / Events
	#region Input
	public static Hat GetJoystickHats( int joystickId ) {
		Hat hat = Hat.Centered;
		nint ptr = GLFWInternal.GetJoystickHats( joystickId, out int count );
		for (int i = 0; i < count; i++) {
			byte value = Marshal.ReadByte( ptr, i );
			hat |= (Hat) value;
		}

		return hat;
	}

	public static string? GetJoystickGuid( int joystickId )
		=> Util.PtrToStringUTF8( GLFWInternal.GetJoystickGuid( joystickId ) );
	public static string? GetGamepadName( int gamepadId )
		=> Util.PtrToStringUTF8( GLFWInternal.GetGamepadName( gamepadId ) );
	public static bool UpdateGamepadMappings( string mappings )
		=> GLFWInternal.UpdateGamepadMappings( Encoding.ASCII.GetBytes( mappings ) );
	public static bool RawMouseMotionSupported()
		=> GLFWInternal.RawMouseMotionSupported();
	public static void SetInputMode( nint windowPtr, int inputMode, int value )
		=> GLFWInternal.SetInputMode( windowPtr, inputMode, value );
	public static void GetCursorPosition( nint windowPtr, out double x, out double y )
		=> GLFWInternal.GetCursorPosition( windowPtr, out x, out y );
	#endregion
	//--------------------------------------------------------//
	#region Events
	public static void SetKeyCallback( nint windowPtr, KeyCallback callback )
		=> GLFWInternal.SetKeyCallback( windowPtr, callback );
	public static void SetCharModsCallback( nint windowPtr, CharModsCallback callback )
		=> GLFWInternal.SetCharModsCallback( windowPtr, callback );
	public static void SetDropCallback( nint windowPtr, FileDropCallback callback )
		=> GLFWInternal.SetDropCallback( windowPtr, callback );
	public static void SetWindowFocusCallback( nint windowPtr, FocusCallback callback )
		=> GLFWInternal.SetWindowFocusCallback( windowPtr, callback );
	public static void SetWindowContentScaleCallback( nint windowPtr, WindowContentsScaleCallback callback )
		=> GLFWInternal.SetWindowContentScaleCallback( windowPtr, callback );
	public static void SetWindowMaximizeCallback( nint windowPtr, WindowMaximizedCallback callback )
		=> GLFWInternal.SetWindowMaximizeCallback( windowPtr, callback );
	public static void SetWindowSizeCallback( nint windowPtr, SizeCallback callback )
		=> GLFWInternal.SetWindowSizeCallback( windowPtr, callback );
	public static void SetFramebufferSizeCallback( nint windowPtr, SizeCallback callback )
		=> GLFWInternal.SetFramebufferSizeCallback( windowPtr, callback );
	public static void SetCloseCallback( nint windowPtr, WindowCallback callback )
		=> GLFWInternal.SetCloseCallback( windowPtr, callback );
	public static void SetWindowRefreshCallback( nint windowPtr, WindowCallback callback )
		=> GLFWInternal.SetWindowRefreshCallback( windowPtr, callback );
	public static void SetWindowPositionCallback( nint windowPtr, PositionCallback callback )
		=> GLFWInternal.SetWindowPositionCallback( windowPtr, callback );
	public static void SetCursorEnterCallback( nint windowPtr, MouseEnterCallback callback )
		=> GLFWInternal.SetCursorEnterCallback( windowPtr, callback );
	public static void SetMouseButtonCallback( nint windowPtr, MouseButtonCallback callback )
		=> GLFWInternal.SetMouseButtonCallback( windowPtr, callback );
	public static void SetCursorPositionCallback( nint windowPtr, MouseCallback callback )
		=> GLFWInternal.SetCursorPositionCallback( windowPtr, callback );
	public static void SetScrollCallback( nint windowPtr, MouseCallback callback )
		=> GLFWInternal.SetScrollCallback( windowPtr, callback );
	public static void SetErrorCallback( ErrorCallback errorCallback )
		=> GLFWInternal.SetErrorCallback( errorCallback );
	#endregion

	#endregion
}
