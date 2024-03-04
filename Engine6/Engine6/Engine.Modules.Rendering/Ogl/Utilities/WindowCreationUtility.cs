using Engine.Modules.Rendering.Glfw.Enums;
using Engine.Modules.Rendering.Glfw.Structs;
using Engine.Modules.Rendering.Ogl.OOP;

namespace Engine.Modules.Rendering.Ogl.Utilities;

internal static class WindowCreationUtility {

	internal static nint Create( WindowSettings settings )
		=> settings.FullscreenMode switch {
			WindowFullscreenMode.Windowed => CreateWindow( settings.Title, settings.Width, settings.Height ),
			WindowFullscreenMode.Fullscreen => CreateFullscreen( settings.Title, settings.Monitor ),
			WindowFullscreenMode.BorderlessFullscreen => CreateBorderlessFullscreen( settings.Title, settings.Monitor ),
			_ => CreateWindow( "Untitled", 800, 600 )
		};

	private static nint CreateWindow( string title, int width, int height/*, Window? share = null*/ ) {
		GlfwUtilities.SetHints( true, 0 );
		nint winPtr = WindowUtilities.CreateWindow( width, height, title, nint.Zero, /*share?.Pointer ??*/ nint.Zero );
		return winPtr;
	}

	/// <summary>
	/// Creates a new fullscreen window, and adds it to the list of windows.
	/// </summary>
	/// <param name="title">The initial title of the window.</param>
	/// <param name="monitorPtr">The monitor the window will fill.</param>
	/// <returns>The window handle.</returns>
	private static nint CreateFullscreen( string title, nint monitorPtr/*, Window? share = null*/ ) {
		VideoMode vm = WindowUtilities.GetVideoMode( monitorPtr );
		GlfwUtilities.SetHints( true, 0 );
		nint winPtr = WindowUtilities.CreateWindow( vm.Width, vm.Height, title, monitorPtr, /*share?.Pointer ??*/ nint.Zero );
		return winPtr;
	}

	/// <summary>
	/// Creates a new borderless windowed fullscreen window, and adds it to the list of windows.
	/// </summary>
	/// <param name="title">The initial title of the window.</param>
	/// <param name="monitorPtr">The monitor the window will fill.</param>
	/// <returns>The window handle.</returns>
	private static nint CreateBorderlessFullscreen( string title, nint monitorPtr/*, Window? share = null*/ ) {
		VideoMode vm = WindowUtilities.GetVideoMode( monitorPtr );
		GlfwUtilities.SetHints( true, 0 );
		nint winPtr = WindowUtilities.CreateWindow( vm.Width, vm.Height, title, nint.Zero, /*share?.Pointer ??*/ nint.Zero );
		WindowUtilities.SetWindowAttribute( winPtr, WindowAttribute.Floating, true );
		WindowUtilities.SetWindowAttribute( winPtr, WindowAttribute.Decorated, false );
		WindowUtilities.GetMonitorPosition( monitorPtr, out int mx, out int my );
		WindowUtilities.SetWindowPosition( winPtr, mx, my );
		return winPtr;
	}
}