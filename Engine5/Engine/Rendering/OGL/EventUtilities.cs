using GlfwBinding;

namespace Engine.Rendering.OGL;

public static class EventUtilities {
	public static void SetCloseCallback( nint windowPtr, WindowCallback callback )
		=> Glfw.SetCloseCallback( windowPtr, callback );
	public static void SetDropCallback( nint windowPtr, FileDropCallback callback )
		=> Glfw.SetDropCallback( windowPtr, callback );
	public static void SetFramebufferSizeCallback( nint windowPtr, SizeCallback callback )
		=> Glfw.SetFramebufferSizeCallback( windowPtr, callback );
	public static void SetWindowContentScaleCallback( nint windowPtr, WindowContentsScaleCallback callback )
		=> Glfw.SetWindowContentScaleCallback( windowPtr, callback );
	public static void SetWindowFocusCallback( nint windowPtr, FocusCallback callback )
		=> Glfw.SetWindowFocusCallback( windowPtr, callback );
	public static void SetWindowMaximizeCallback( nint windowPtr, WindowMaximizedCallback callback )
		=> Glfw.SetWindowMaximizeCallback( windowPtr, callback );
	public static void SetWindowPositionCallback( nint windowPtr, PositionCallback callback )
		=> Glfw.SetWindowPositionCallback( windowPtr, callback );
	public static void SetWindowRefreshCallback( nint windowPtr, WindowCallback callback )
		=> Glfw.SetWindowRefreshCallback( windowPtr, callback );
	public static void SetWindowSizeCallback( nint windowPtr, SizeCallback callback )
		=> Glfw.SetWindowSizeCallback( windowPtr, callback );
	public static void SetCursorEnterCallback( nint windowPtr, MouseEnterCallback callback )
		=> Glfw.SetCursorEnterCallback( windowPtr, callback );
	public static void SetCursorPositionCallback( nint windowPtr, MouseCallback callback )
		=> Glfw.SetCursorPositionCallback( windowPtr, callback );
	public static void SetMouseButtonCallback( nint windowPtr, MouseButtonCallback callback )
			=> Glfw.SetMouseButtonCallback( windowPtr, callback );
	public static void SetScrollCallback( nint windowPtr, MouseCallback callback )
			=> Glfw.SetScrollCallback( windowPtr, callback );
	public static void SetKeyCallback( nint windowPtr, KeyCallback callback )
			=> Glfw.SetKeyCallback( windowPtr, callback );
	public static void SetCharModsCallback( nint windowPtr, CharModsCallback callback )
			=> Glfw.SetCharModsCallback( windowPtr, callback );
}
