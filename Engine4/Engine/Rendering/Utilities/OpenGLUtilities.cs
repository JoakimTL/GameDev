using GLFW;
using OpenGL;

namespace Engine.Rendering.Utilities;
public static class OpenGLUtilities {
	public static bool Initialized { get; private set; } = false;
	private static readonly ManualResetEvent _initializationWaiter = new(false);

	public static void WaitInitialization() {
		Log.Line( "Waiting for OpenGL initialization...", Log.Level.LOW );
		_initializationWaiter.WaitOne();
	}

	public static void InitializeGL() {
		if ( Initialized )
			return;

		Gl.Initialize();

		Glfw.SetErrorCallback( ErrorCallback );

		if ( !Glfw.Init() )
			throw new System.Exception( "Initialization of GLFW failed!" );

		Initialized = true;
		_initializationWaiter.Set();
		Log.Line( "OpenGL initialized!", Log.Level.NORMAL, ConsoleColor.Green );
	}

	public static void SetHints() {
		Glfw.WindowHint( Hint.Samples, 0 );
		Glfw.WindowHint( Hint.ContextVersionMajor, 4 );
		Glfw.WindowHint( Hint.ContextVersionMinor, 6 );
		Glfw.WindowHint( Hint.OpenglForwardCompatible, Constants.True );
		Glfw.WindowHint( Hint.OpenglProfile, (int) Profile.Core );
		Glfw.WindowHint( Hint.OpenglDebugContext, true );
	}

	public static void PollEvents() => Glfw.PollEvents();

	public static void Terminate() {
		Log.Line( "Terminating GLFW!", Log.Level.HIGH, ConsoleColor.Blue );
		Glfw.Terminate();
	}

	public static void SetVSync( int vsync ) => Glfw.SwapInterval( vsync );

	private static void ErrorCallback( GLFW.ErrorCode code, IntPtr desc ) {
		Log.Error( "GLFW Error: " + code + ": " + desc );
		Console.ReadLine();
	}

	public static WindowPtr CreateWindow( string title, int width, int height, Window? share = null ) {
		SetHints();
		WindowPtr winPtr = Glfw.CreateWindow( width, height, title, MonitorPtr.None, share?.Pointer ?? WindowPtr.None );
		return winPtr;
	}

	/// <summary>
	/// Creates a new fullscreen window, and adds it to the list of windows.
	/// </summary>
	/// <param name="title">The initial title of the window.</param>
	/// <param name="m">The monitor the window will fill.</param>
	/// <returns>The window handle.</returns>
	public static WindowPtr CreateFullscreen( string title, MonitorPtr m, Window? share = null ) {
		VideoMode vm = Glfw.GetVideoMode( m );
		SetHints();
		WindowPtr winPtr = Glfw.CreateWindow( vm.Width, vm.Height, title, m, share?.Pointer ?? WindowPtr.None );
		return winPtr;
	}

	/// <summary>
	/// Creates a new borderless windowed fullscreen window, and adds it to the list of windows.
	/// </summary>
	/// <param name="title">The initial title of the window.</param>
	/// <param name="m">The monitor the window will fill.</param>
	/// <returns>The window handle.</returns>
	public static WindowPtr CreateWindowedFullscreen( string title, MonitorPtr m, Window? share = null ) {
		VideoMode vm = Glfw.GetVideoMode( m );
		SetHints();
		WindowPtr winPtr = Glfw.CreateWindow( vm.Width, vm.Height, title, MonitorPtr.None, share?.Pointer ?? WindowPtr.None );
		Glfw.SetWindowAttribute( winPtr, WindowAttribute.Floating, true );
		Glfw.SetWindowAttribute( winPtr, WindowAttribute.Decorated, false );
		Glfw.GetMonitorPosition( m, out int mx, out int my );
		Glfw.SetWindowPosition( winPtr, mx, my );
		return winPtr;
	}

	internal static string GLDebugMessageDecipher( DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam ) {
		unsafe {
			string errorDescription = "";
			for ( int i = 0; i < length; i++ )
				errorDescription += (char) ( (byte*) message )[ i ];

			string sourceText = source switch {
				DebugSource.DontCare => "None",
				DebugSource.DebugSourceWindowSystem => "Window System",
				DebugSource.DebugSourceThirdParty => "3rd Party",
				DebugSource.DebugSourceShaderCompiler => "Shader Compiler",
				DebugSource.DebugSourceOther => "Other",
				DebugSource.DebugSourceApplication => "Application",
				DebugSource.DebugSourceApi => "API",
				_ => "Unknown",
			};
			string typeText = type switch {
				DebugType.DontCare => "None",
				DebugType.DebugTypeUndefinedBehavior => "Undefined",
				DebugType.DebugTypePushGroup => "Push",
				DebugType.DebugTypePortability => "Portability",
				DebugType.DebugTypePopGroup => "Pop",
				DebugType.DebugTypePerformance => "Performance",
				DebugType.DebugTypeOther => "Other",
				DebugType.DebugTypeMarker => "Marker",
				DebugType.DebugTypeError => "Error",
				DebugType.DebugTypeDeprecatedBehavior => "Deprecated",
				_ => "Unknown",
			};
			string severityText = severity switch {
				DebugSeverity.DontCare => "None",
				DebugSeverity.DebugSeverityNotification => "Note",
				DebugSeverity.DebugSeverityLow => "Low",
				DebugSeverity.DebugSeverityMedium => "Med",
				DebugSeverity.DebugSeverityHigh => "High",
				_ => "Unknown",
			};
			return $"[{id}/{severityText}] {sourceText} {typeText}: {errorDescription}";
		}
	}
}
