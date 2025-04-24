using Engine.Module.Render.Glfw;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Modules.Rendering.Glfw;

internal static unsafe partial class GLFWInternal {
	//https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke-source-generation
	public const string LIBRARY = "Glfw/glfw3";

	#region Glfw
	#region Vulkan

	/// <summary>
	///     This function creates a Vulkan surface for the specified window.
	/// </summary>
	/// <param name="vulkan">A pointer to the Vulkan instance.</param>
	/// <param name="window">The window handle.</param>
	/// <param name="allocator">A pointer to the allocator to use, or <see cref="nint.Zero" /> to use default allocator.</param>
	/// <param name="surface">The handle to the created Vulkan surface.</param>
	/// <returns>VK_SUCCESS if successful, or a Vulkan error code if an error occurred.</returns>
	[LibraryImport( LIBRARY, EntryPoint = "glfwCreateWindowSurface" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial int CreateWindowSurface( nint vulkan, nint window, nint allocator, out ulong surface );

	/// <summary>
	///     This function returns whether the specified queue family of the specified physical device supports presentation to
	///     the platform GLFW was built for.
	/// </summary>
	/// <param name="instance">The instance that the physical device belongs to.</param>
	/// <param name="device">The physical device that the queue family belongs to.</param>
	/// <param name="family">The index of the queue family to query.</param>
	/// <returns><c>true</c> if the queue family supports presentation, or <c>false</c> otherwise.</returns>
	[LibraryImport( LIBRARY, EntryPoint = "glfwGetPhysicalDevicePresentationSupport" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static partial bool GetPhysicalDevicePresentationSupport( nint instance, nint device, uint family );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetInstanceProcAddress" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetInstanceProcAddress( nint vulkan, byte[] procName );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetRequiredInstanceExtensions" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetRequiredInstanceExtensions( out uint count );

	[LibraryImport( LIBRARY, EntryPoint = "glfwVulkanSupported" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static partial bool VulkanSupported();
	#endregion

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetError" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial int GetError( out nint description );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetProcAddress" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetProcAddress( byte[] procName );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetClipboardString" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetClipboardStringInternal( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetClipboardString" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetClipboardString( nint windowPtr, byte[] bytes );

	[LibraryImport( LIBRARY, EntryPoint = "glfwWaitEvents" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void WaitEvents();

	[LibraryImport( LIBRARY, EntryPoint = "glfwPollEvents" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void PollEvents();

	[LibraryImport( LIBRARY, EntryPoint = "glfwPostEmptyEvent" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void PostEmptyEvent();

	[LibraryImport( LIBRARY, EntryPoint = "glfwWaitEventsTimeout" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void WaitEventsTimeout( double timeout );

	[LibraryImport( LIBRARY, EntryPoint = "glfwExtensionSupported" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static partial bool GetExtensionSupported( byte[] extension );

	[LibraryImport( LIBRARY, EntryPoint = "glfwMakeContextCurrent" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void MakeContextCurrent( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetVersion" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetVersion( out int major, out int minor, out int revision );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetVersionString" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetVersionString();

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetTime" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial double GetTime();

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetTime" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetTime( double time );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetTimerFrequency" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial ulong GetTimerFrequency();

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetTimerValue" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial ulong GetTimerValue();

	[LibraryImport( LIBRARY, EntryPoint = "glfwInitHint" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void InitHint( int hint, [MarshalAs( UnmanagedType.Bool )] bool value );

	[LibraryImport( LIBRARY, EntryPoint = "glfwInit" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static partial bool Init();

	[LibraryImport( LIBRARY, EntryPoint = "glfwTerminate" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void Terminate();
	#endregion

	#region Mouse
	[LibraryImport( LIBRARY, EntryPoint = "glfwCreateCursor" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint CreateCursor( nint imagePtr, int xHotspot, int yHotspot );

	[LibraryImport( LIBRARY, EntryPoint = "glfwDestroyCursor" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void DestroyCursor( nint cursor );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetCursor" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetCursor( nint windowPtr, nint cursor );

	[LibraryImport( LIBRARY, EntryPoint = "glfwCreateStandardCursor" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint CreateStandardCursor( int type );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetCursorPos" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetCursorPosition( nint windowPtr, out double x, out double y );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetCursorPos" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetCursorPosition( nint windowPtr, double x, double y );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetMouseButton" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial int GetMouseButton( nint windowPtr, int button );

	[LibraryImport( LIBRARY, EntryPoint = "glfwRawMouseMotionSupported" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static partial bool RawMouseMotionSupported();
	#endregion

	#region Key
	[LibraryImport( LIBRARY, EntryPoint = "glfwGetKey" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial int GetKey( nint windowPtr, int key );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetKeyName" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetKeyNameInternal( int key, int scanCode );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetKeyScancode" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial int GetKeyScanCode( int key );
	#endregion

	#region Joystick
	[LibraryImport( LIBRARY, EntryPoint = "glfwJoystickPresent" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static partial bool JoystickPresent( int joystick );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetJoystickName" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetJoystickNameInternal( int joystick );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetJoystickAxes" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetJoystickAxes( int joystick, out int count );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetJoystickButtons" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetJoystickButtons( int joystick, out int count );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetJoystickHats" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetJoystickHats( int joystickId, out int count );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetJoystickGUID" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetJoystickGuid( int joystickId );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetJoystickUserPointer" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetJoystickUserPointer( int joystickId );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetJoystickUserPointer" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetJoystickUserPointer( int joystickId, nint pointer );

	[LibraryImport( LIBRARY, EntryPoint = "glfwJoystickIsGamepad" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static partial bool JoystickIsGamepad( int joystickId );

	[LibraryImport( LIBRARY, EntryPoint = "glfwUpdateGamepadMappings" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static partial bool UpdateGamepadMappings( byte[] mappings );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetGamepadName" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetGamepadName( int gamepadId );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetGamepadState" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static partial bool GetGamepadState( int id, out nint gamePadStatePtr );
	#endregion

	#region Window
	[LibraryImport( LIBRARY, EntryPoint = "glfwGetMonitorContentScale" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetMonitorContentScale( nint monitorPtr, out float xScale, out float yScale );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetMonitorUserPointer" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetMonitorUserPointer( nint monitorPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetMonitorUserPointer" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetMonitorUserPointer( nint monitorPtr, nint pointer );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetWindowOpacity" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial float GetWindowOpacity( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowOpacity" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowOpacity( nint windowPtr, float opacity );

	[LibraryImport( LIBRARY, EntryPoint = "glfwWindowHintString" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void WindowHintString( int hint, byte[] value );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowAttrib" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowAttribute( nint windowPtr, int windowAttribute, [MarshalAs( UnmanagedType.Bool )] bool value );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetWindowContentScale" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetWindowContentScale( nint windowPtr, out float xScale, out float yScale );

	[LibraryImport( LIBRARY, EntryPoint = "glfwRequestWindowAttention" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void RequestWindowAttention( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwWindowHint" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void WindowHint( int hint, int value );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetWindowAttrib" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial int GetWindowAttribute( nint windowPtr, int attribute );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetInputMode" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetInputMode( nint windowPtr, int mode, int value );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetInputMode" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial int GetInputMode( nint windowPtr, int mode );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowUserPointer" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowUserPointer( nint windowPtr, nint userPointer );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetWindowUserPointer" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetWindowUserPointer( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowSizeLimits" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowSizeLimits( nint windowPtr, int minWidth, int minHeight, int maxWidth, int maxHeight );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowAspectRatio" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowAspectRatio( nint windowPtr, int numerator, int denominator );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetCurrentContext" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetCurrentContext();

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetMonitorName" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetMonitorNameInternal( nint monitorPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetGammaRamp" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetGammaRampInternal( nint monitorPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetGammaRamp" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetGammaRamp( nint monitorPtr, nint gammaRampPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetGamma" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetGamma( nint monitorPtr, float gamma );

	[LibraryImport( LIBRARY, EntryPoint = "glfwCreateWindow" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint CreateWindow( int width, int height, byte[] title, nint monitorPtr, nint share );

	[LibraryImport( LIBRARY, EntryPoint = "glfwDestroyWindow" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void DestroyWindow( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwShowWindow" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void ShowWindow( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwHideWindow" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void HideWindow( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetWindowPos" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetWindowPosition( nint windowPtr, out int x, out int y );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowPos" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowPosition( nint windowPtr, int x, int y );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetWindowSize" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetWindowSize( nint windowPtr, out int width, out int height );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowSize" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowSize( nint windowPtr, int width, int height );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetFramebufferSize" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetFramebufferSize( nint windowPtr, out int width, out int height );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowTitle" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowTitle( nint windowPtr, byte[] title );

	[LibraryImport( LIBRARY, EntryPoint = "glfwFocusWindow" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void FocusWindow( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetWindowFrameSize" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetWindowFrameSize( nint windowPtr, out int left, out int top, out int right, out int bottom );

	[LibraryImport( LIBRARY, EntryPoint = "glfwMaximizeWindow" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void MaximizeWindow( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwIconifyWindow" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void IconifyWindow( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwRestoreWindow" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void RestoreWindow( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSwapBuffers" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SwapBuffers( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSwapInterval" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SwapInterval( int interval );

	[LibraryImport( LIBRARY, EntryPoint = "glfwDefaultWindowHints" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void DefaultWindowHints();

	[LibraryImport( LIBRARY, EntryPoint = "glfwWindowShouldClose" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static partial bool WindowShouldClose( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowShouldClose" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowShouldClose( nint windowPtr, [MarshalAs( UnmanagedType.Bool )] bool close );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowIcon" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowIcon( nint windowPtr, int count, nint imageArrayZeroIndexPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetPrimaryMonitor" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetPrimaryMonitor();

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetVideoMode" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetVideoMode( nint monitorPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetVideoModes" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetVideoModes( nint monitorPtr, out int count );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetWindowMonitor" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetWindowMonitor( nint windowPtr );

	[LibraryImport( LIBRARY, EntryPoint = "glfwSetWindowMonitor" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void SetWindowMonitor( nint windowPtr, nint monitorPtr, int x, int y, int width, int height, int refreshRate );
	#region Monitor
	[LibraryImport( LIBRARY, EntryPoint = "glfwGetMonitorWorkarea" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetMonitorWorkArea( nint monitorPtr, out int x, out int y, out int width, out int height );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetMonitorPhysicalSize" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetMonitorPhysicalSize( nint monitorPtr, out int width, out int height );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetMonitorPos" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial void GetMonitorPosition( nint monitorPtr, out int x, out int y );

	[LibraryImport( LIBRARY, EntryPoint = "glfwGetMonitors" )]
	[UnmanagedCallConv( CallConvs = [ typeof( CallConvCdecl ) ] )]
	public static partial nint GetMonitors( out int count );
	#endregion
	#endregion

	#region Callbacks
	#region Mouse
	[DllImport( LIBRARY, EntryPoint = "glfwSetScrollCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( MouseCallback ) )]
	public static extern MouseCallback SetScrollCallback( nint windowPtr, MouseCallback mouseCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetCursorPosCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( MouseCallback ) )]
	public static extern MouseCallback SetCursorPositionCallback( nint windowPtr, MouseCallback mouseCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetCursorEnterCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( MouseEnterCallback ) )]
	public static extern MouseEnterCallback SetCursorEnterCallback( nint windowPtr, MouseEnterCallback mouseCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetMouseButtonCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( MouseButtonCallback ) )]
	public static extern MouseButtonCallback SetMouseButtonCallback( nint windowPtr, MouseButtonCallback mouseCallback );
	#endregion

	#region Keyboard
	[DllImport( LIBRARY, EntryPoint = "glfwSetKeyCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( KeyCallback ) )]
	public static extern KeyCallback SetKeyCallback( nint windowPtr, KeyCallback keyCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetCharCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( CharCallback ) )]
	public static extern CharCallback SetCharCallback( nint windowPtr, CharCallback charCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetCharModsCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( CharModsCallback ) )]
	public static extern CharModsCallback SetCharModsCallback( nint windowPtr, CharModsCallback charCallback );
	#endregion

	#region Joystick
	[DllImport( LIBRARY, EntryPoint = "glfwSetJoystickCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( JoystickCallback ) )]
	public static extern JoystickCallback SetJoystickCallback( JoystickCallback callback );
	#endregion

	#region Window
	[DllImport( LIBRARY, EntryPoint = "glfwSetWindowMaximizeCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( WindowMaximizedCallback ) )]
	public static extern WindowMaximizedCallback SetWindowMaximizeCallback( nint windowPtr, WindowMaximizedCallback cb );

	[DllImport( LIBRARY, EntryPoint = "glfwSetWindowContentScaleCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( WindowContentsScaleCallback ) )]
	public static extern WindowContentsScaleCallback SetWindowContentScaleCallback( nint windowPtr, WindowContentsScaleCallback cb );

	[DllImport( LIBRARY, EntryPoint = "glfwSetDropCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( FileDropCallback ) )]
	public static extern FileDropCallback SetDropCallback( nint windowPtr, FileDropCallback dropCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetMonitorCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( MonitorCallback ) )]
	public static extern MonitorCallback SetMonitorCallback( MonitorCallback monitorCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetWindowIconifyCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( IconifyCallback ) )]
	public static extern IconifyCallback SetWindowIconifyCallback( nint windowPtr, IconifyCallback callback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetFramebufferSizeCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( SizeCallback ) )]
	public static extern SizeCallback SetFramebufferSizeCallback( nint windowPtr, SizeCallback sizeCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetWindowRefreshCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( WindowCallback ) )]
	public static extern WindowCallback SetWindowRefreshCallback( nint windowPtr, WindowCallback callback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetWindowPosCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( PositionCallback ) )]
	public static extern PositionCallback SetWindowPositionCallback( nint windowPtr, PositionCallback positionCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetWindowSizeCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( SizeCallback ) )]
	public static extern SizeCallback SetWindowSizeCallback( nint windowPtr, SizeCallback sizeCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetWindowCloseCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( WindowCallback ) )]
	public static extern WindowCallback SetCloseCallback( nint windowPtr, WindowCallback closeCallback );

	[DllImport( LIBRARY, EntryPoint = "glfwSetWindowFocusCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( FocusCallback ) )]
	public static extern FocusCallback SetWindowFocusCallback( nint windowPtr, FocusCallback focusCallback );
	#endregion

	[DllImport( LIBRARY, EntryPoint = "glfwSetErrorCallback", CallingConvention = CallingConvention.Cdecl )]
	[return: MarshalAs( UnmanagedType.FunctionPtr, MarshalTypeRef = typeof( ErrorCallback ) )]
	public static extern ErrorCallback SetErrorCallback( ErrorCallback errorHandler );
	#endregion
}
