using Engine.Module.Render.Glfw;

namespace Engine.Module.Render.Input;

public static class EventUtilities {
	public static void SetCloseCallback( nint windowPtr, WindowCallback callback )
		=> GLFW.SetCloseCallback( windowPtr, callback );
	public static void SetDropCallback( nint windowPtr, FileDropCallback callback )
		=> GLFW.SetDropCallback( windowPtr, callback );
	public static void SetFramebufferSizeCallback( nint windowPtr, SizeCallback callback )
		=> GLFW.SetFramebufferSizeCallback( windowPtr, callback );
	public static void SetWindowContentScaleCallback( nint windowPtr, WindowContentsScaleCallback callback )
		=> GLFW.SetWindowContentScaleCallback( windowPtr, callback );
	public static void SetWindowFocusCallback( nint windowPtr, FocusCallback callback )
		=> GLFW.SetWindowFocusCallback( windowPtr, callback );
	public static void SetWindowMaximizeCallback( nint windowPtr, WindowMaximizedCallback callback )
		=> GLFW.SetWindowMaximizeCallback( windowPtr, callback );
	public static void SetWindowPositionCallback( nint windowPtr, PositionCallback callback )
		=> GLFW.SetWindowPositionCallback( windowPtr, callback );
	public static void SetWindowRefreshCallback( nint windowPtr, WindowCallback callback )
		=> GLFW.SetWindowRefreshCallback( windowPtr, callback );
	public static void SetWindowSizeCallback( nint windowPtr, SizeCallback callback )
		=> GLFW.SetWindowSizeCallback( windowPtr, callback );
	public static void SetCursorEnterCallback( nint windowPtr, MouseEnterCallback callback )
		=> GLFW.SetCursorEnterCallback( windowPtr, callback );
	public static void SetCursorPositionCallback( nint windowPtr, MouseCallback callback )
		=> GLFW.SetCursorPositionCallback( windowPtr, callback );
	public static void SetMouseButtonCallback( nint windowPtr, MouseButtonCallback callback )
		=> GLFW.SetMouseButtonCallback( windowPtr, callback );
	public static void SetScrollCallback( nint windowPtr, MouseCallback callback )
		=> GLFW.SetScrollCallback( windowPtr, callback );
	public static void SetKeyCallback( nint windowPtr, KeyCallback callback )
		=> GLFW.SetKeyCallback( windowPtr, callback );
	public static void SetCharModsCallback( nint windowPtr, CharModsCallback callback )
		=> GLFW.SetCharModsCallback( windowPtr, callback );
}
