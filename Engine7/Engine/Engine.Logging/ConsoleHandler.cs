﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Logging;
internal static class ConsoleHandler {
	[DllImport( "kernel32.dll" )]
	static extern IntPtr GetConsoleWindow();

	[DllImport( "user32.dll" )]
	static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );

	const int SW_HIDE = 0;
	const int SW_SHOW = 5;

	public static void Hide() {
		var handle = GetConsoleWindow();
		ShowWindow( handle, SW_HIDE );
	}

	public static void Show() {
		var handle = GetConsoleWindow();
		ShowWindow( handle, SW_SHOW );
	}

}