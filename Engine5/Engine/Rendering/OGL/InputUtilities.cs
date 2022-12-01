using GlfwBinding;
using GlfwBinding.Enums;

namespace Engine.Rendering.OGL;

public static class InputUtilities {
	public static void GetCursorPosition( nint windowPtr, out double x, out double y ) 
		=> Glfw.GetCursorPosition( windowPtr, out x, out y );

	public static int GetWindowAttribute( nint windowPtr, WindowAttribute attribute )
		=> Glfw.GetWindowAttribute( windowPtr, (int) attribute );

	public static int GetWindowAttribute( nint windowPtr, int attribute )
		=> Glfw.GetWindowAttribute( windowPtr, attribute );

	public static void SetInputMode( nint windowPtr, InputMode inputMode, int value )
		=> Glfw.SetInputMode( windowPtr, (int) inputMode, value );

	public static void SetInputMode( nint windowPtr, int inputMode, int value )
		=> Glfw.SetInputMode( windowPtr, inputMode, value );
}
