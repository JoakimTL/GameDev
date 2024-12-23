﻿using System.Runtime.InteropServices;

namespace Engine.Logging;
internal static class ConsoleHandler {
	[DllImport( "kernel32.dll" )]
	static extern IntPtr GetConsoleWindow();

	[DllImport( "user32.dll" )]
	static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );

	const int SW_HIDE = 0;
	const int SW_SHOW = 5;

	public static void Hide() {
		nint handle = GetConsoleWindow();
		ShowWindow( handle, SW_HIDE );
	}

	public static void Show() {
		nint handle = GetConsoleWindow();
		ShowWindow( handle, SW_SHOW );
	}

}
