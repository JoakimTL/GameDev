namespace GLFW;

/// <summary>
///     Strongly-typed description for possible client APIs to be used.
/// </summary>
public enum ClientApi {
	/// <summary>
	///     No context
	/// </summary>
	None = 0x00000000,

	/// <summary>
	///     OpenGL
	/// </summary>
	OpenGL = 0x00030001,

	/// <summary>
	///     OpenGL ES
	/// </summary>
	OpenGLES = 0x00030002
}

/// <summary>
///     Strongly-typed values indicating connection status of joysticks, monitors, etc.
/// </summary>
public enum ConnectionStatus {
	/// <summary>
	///     Unknown connection status.
	/// </summary>
	Unknown = 0x00000000,

	/// <summary>
	///     Device is currently connected and visible to GLFW.
	/// </summary>
	Connected = 0x00040001,

	/// <summary>
	///     Device is disconnected and/or not visible to GLFW.
	/// </summary>
	Disconnected = 0x00040002
}

/// <summary>
///     Strongly-typed values for commonly used constants.
///     <para>You are free to use the integer value instead of these constants.</para>
/// </summary>
public enum Constants {
	/// <summary>
	///     No preference or don't care, use default value.
	/// </summary>
	Default = -1,

	/// <summary>
	///     Boolean false
	/// </summary>
	False = 0,

	/// <summary>
	///     Boolean true
	/// </summary>
	True = 1
}

/// <summary>
///     Describes the API used for creating the OpenGL context.
/// </summary>
public enum ContextApi {
	/// <summary>
	///     The native platform default.
	/// </summary>
	Native = 0x00036001,

	/// <summary>
	///     EGL
	/// </summary>
	Egl = 0x00036002,

	/// <summary>
	///     OS Mesa
	/// </summary>
	Mesa = 0x00036003
}

/// <summary>
///     Used internally to consolidate strongly-typed values for getting/setting window attributes.
/// </summary>
internal enum ContextAttributes {
	ClientApi = 0x00022001,
	ContextCreationApi = 0x0002200B,
	ContextVersionMajor = 0x00022002,
	ContextVersionMinor = 0x00022003,
	ContextVersionRevision = 0x00022004,
	OpenglForwardCompat = 0x00022006,
	OpenglDebugContext = 0x00022007,
	OpenglProfile = 0x00022008,
	ContextRobustness = 0x00022005
}

/// <summary>
///     Indicates the behavior of the mouse cursor.
/// </summary>
public enum CursorMode {
	/// <summary>
	///     The cursor is visible and behaves normally.
	/// </summary>
	Normal = 0x00034001,

	/// <summary>
	///     The cursor is invisible when it is over the client area of the window but does not restrict the cursor from
	///     leaving.
	/// </summary>
	Hidden = 0x00034002,

	/// <summary>
	///     Hides and grabs the cursor, providing virtual and unlimited cursor movement. This is useful for implementing for
	///     example 3D camera controls.
	/// </summary>
	Disabled = 0x00034003
}

/// <summary>
///     Strongly-typed values describing possible cursor shapes.
/// </summary>
public enum CursorType {
	/// <summary>
	///     The regular arrow cursor.
	/// </summary>
	Arrow = 0x00036001,

	/// <summary>
	///     The text input I-beam cursor shape.
	/// </summary>
	Beam = 0x00036002,

	/// <summary>
	///     The crosshair shape.
	/// </summary>
	Crosshair = 0x00036003,

	/// <summary>
	///     The hand shape.
	/// </summary>
	Hand = 0x00036004,

	/// <summary>
	///     The horizontal resize arrow shape.
	/// </summary>
	ResizeHorizontal = 0x00036005,

	/// <summary>
	///     The vertical resize arrow shape.
	/// </summary>
	ResizeVertical = 0x00036006
}

/// <summary>
///     Strongly-typed error codes for error handling.
/// </summary>
public enum ErrorCode {
	/// <summary>
	///     An unknown or undefined error.
	/// </summary>
	[Obsolete( "Use None" )]
	Unknown = 0x00000000,

	/// <summary>
	///     No error has occurred.
	/// </summary>
	None = 0x00000000,

	/// <summary>
	///     This occurs if a GLFW function was called that must not be called unless the library is initialized.
	/// </summary>
	NotInitialized = 0x00010001,

	/// <summary>
	///     This occurs if a GLFW function was called that needs and operates on the current OpenGL or OpenGL ES context but no
	///     context is current on the calling thread.
	/// </summary>
	NoCurrentContext = 0x00010002,

	/// <summary>
	///     One of the arguments to the function was an invalid enum value.
	/// </summary>
	InvalidEnum = 0x00010003,

	/// <summary>
	///     One of the arguments to the function was an invalid value, for example requesting a non-existent OpenGL or OpenGL
	///     ES version like 2.7.
	/// </summary>
	InvalidValue = 0x00010004,

	/// <summary>
	///     A memory allocation failed.
	/// </summary>
	OutOfMemory = 0x00010005,

	/// <summary>
	///     GLFW could not find support for the requested API on the system.
	/// </summary>
	ApiUnavailable = 0x00010006,

	/// <summary>
	///     The requested OpenGL or OpenGL ES version (including any requested context or framebuffer hints) is not available
	///     on this machine.
	/// </summary>
	VersionUnavailable = 0x00010007,

	/// <summary>
	///     A platform-specific error occurred that does not match any of the more specific categories.
	/// </summary>
	PlatformError = 0x00010008,

	/// <summary>
	///     If emitted during window creation, the requested pixel format is not supported, else if emitted when querying the
	///     clipboard, the contents of the clipboard could not be converted to the requested format.
	/// </summary>
	FormatUnavailable = 0x00010009,

	/// <summary>
	///     A window that does not have an OpenGL or OpenGL ES context was passed to a function that requires it to have one.
	/// </summary>
	NoWindowContext = 0x0001000A
}

/// <summary>
///     Represents a gamepad axis.
/// </summary>
public enum GamePadAxis {
	LeftX = 0,
	LeftY = 1,
	RightX = 2,
	RightY = 3,
	LeftTrigger = 4,
	RightTrigger = 5
}

/// <summary>
///     Represents gamepad buttons.
///     <para>
///         Duplicate values convenience for providing naming conventions for common gamepads (PlayStation,
///         X-Box, etc).
///     </para>
/// </summary>
public enum GamePadButton : byte {
	A = 0,
	B = 1,
	X = 2,
	Y = 3,
	LeftBumper = 4,
	RightBumper = 5,
	Back = 6,
	Start = 7,
	Guide = 8,
	LeftThumb = 9,
	RightThumb = 10,
	DpadUp = 11,
	DpadRight = 12,
	DpadDown = 13,
	DpadLeft = 14,
	Cross = A,
	Circle = B,
	Square = X,
	Triangle = Y
}

/// <summary>
///     Describes joystick hat states.
/// </summary>
[Flags]
public enum Hat {
	/// <summary>
	///     Centered
	/// </summary>
	Centered = 0x00,

	/// <summary>
	///     Up
	/// </summary>
	Up = 0x01,

	/// <summary>
	///     Right
	/// </summary>
	Right = 0x02,

	/// <summary>
	///     Down
	/// </summary>
	Down = 0x04,

	/// <summary>
	///     Left
	/// </summary>
	Left = 0x08,

	/// <summary>
	///     Right and up
	/// </summary>
	RightUp = Right | Up,

	/// <summary>
	///     Right and down
	/// </summary>
	RightDown = Right | Down,

	/// <summary>
	///     Left and up
	/// </summary>
	LeftUp = Left | Up,

	/// <summary>
	///     Left and down
	/// </summary>
	LeftDown = Left | Down
}

/// <summary>
///     Strongly-typed values for setting window hints.
/// </summary>
public enum Hint {
	/// <summary>
	///     Specifies whether the windowed mode window will be given input focus when created. This hint is ignored for full
	///     screen and initially hidden windows.
	///     <para>Default Value: <see cref="Constants.True" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	Focused = 0x00020001,

	/// <summary>
	///     Specifies whether the windowed mode window will be resizable by the user. The window will still be resizable
	///     programmatically. This hint is ignored for full screen windows.
	///     <para>Default Value: <see cref="Constants.True" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	Resizable = 0x00020003,

	/// <summary>
	///     Specifies whether the windowed mode window will be initially visible.This hint is ignored for full screen windows.
	///     <para>Default Value: <see cref="Constants.True" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	Visible = 0x00020004,

	/// <summary>
	///     Specifies whether the windowed mode window will have window decorations such as a border, a close widget, etc.An
	///     undecorated window may still allow the user to generate close events on some platforms.This hint is ignored for
	///     full screen windows.
	///     <para>Default Value: <see cref="Constants.True" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	Decorated = 0x00020005,

	/// <summary>
	///     Specifies whether the full screen window will automatically iconify and restore the previous video mode on input
	///     focus loss. This hint is ignored for windowed mode windows.
	///     <para>Default Value: <see cref="Constants.True" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	AutoIconify = 0x00020006,

	/// <summary>
	///     Specifies whether the windowed mode window will be floating above other regular windows, also called topmost or
	///     always-on-top.This is intended primarily for debugging purposes and cannot be used to implement proper full screen
	///     windows. This hint is ignored for full screen windows.
	///     <para>Default Value: <see cref="Constants.False" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	Floating = 0x00020007,

	/// <summary>
	///     Specifies whether the windowed mode window will be maximized when created. This hint is ignored for full screen
	///     windows.
	///     <para>Default Value: <see cref="Constants.False" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	Maximized = 0x00020008,

	/// <summary>
	///     Specifies the desired bit depth of the red component for default framebuffer. <see cref="Constants.Default" />
	///     means
	///     the application has no preference.
	///     <para>Default Value: <c>8</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	RedBits = 0x00021001,

	/// <summary>
	///     Specifies the desired bit depth of the green component for default framebuffer. <see cref="Constants.Default" />
	///     means
	///     the application has no preference.
	///     <para>Default Value: <c>8</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	GreenBits = 0x00021002,

	/// <summary>
	///     Specifies the desired bit depth of the blue component for default framebuffer. <see cref="Constants.Default" />
	///     means
	///     the application has no preference.
	///     <para>Default Value: <c>8</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	BlueBits = 0x00021003,

	/// <summary>
	///     Specifies the desired bit depth of the alpha component for default framebuffer. <see cref="Constants.Default" />
	///     means
	///     the application has no preference.
	///     <para>Default Value: <c>8</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	AlphaBits = 0x00021004,

	/// <summary>
	///     Specifies the desired bit depth of for default framebuffer. <see cref="Constants.Default" />"/> means the
	///     application
	///     has no preference.
	///     <para>Default Value: <c>24</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	DepthBits = 0x00021005,

	/// <summary>
	///     Specifies the desired stencil bits for default framebuffer. <see cref="Constants.Default" /> means the application
	///     has
	///     no preference.
	///     <para>Default Value: <c>0</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	StencilBits = 0x00021006,

	/// <summary>
	///     Specify the desired bit depths of the red component of the accumulation buffer. <see cref="Constants.Default" />
	///     means
	///     the application has no preference.
	///     <para>Accumulation buffers are a legacy OpenGL feature and should not be used in new code.</para>
	///     <para>Default Value: <c>0</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	[Obsolete]
	AccumRedBits = 0x00021007,

	/// <summary>
	///     Specify the desired bit depths of the green component of the accumulation buffer. <see cref="Constants.Default" />
	///     means the application has no preference.
	///     <para>Accumulation buffers are a legacy OpenGL feature and should not be used in new code.</para>
	///     <para>Default Value: <c>0</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	[Obsolete]
	AccumGreenBits = 0x00021008,

	/// <summary>
	///     Specify the desired bit depths of the blue component of the accumulation buffer. <see cref="Constants.Default" />
	///     means the application has no preference.
	///     <para>Accumulation buffers are a legacy OpenGL feature and should not be used in new code.</para>
	///     <para>Default Value: <c>0</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	[Obsolete]
	AccumBlueBits = 0x00021009,

	/// <summary>
	///     Specify the desired bit depths of the alpha component of the accumulation buffer.
	///     <para><see cref="Constants.Default" /> means the application has no preference.</para>
	///     <para>Accumulation buffers are a legacy OpenGL feature and should not be used in new code.</para>
	///     <para>Default Value: <c>0</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	[Obsolete]
	AccumAlphaBits = 0x0002100a,

	/// <summary>
	///     Specifies the desired number of auxiliary buffers.<see cref="Constants.Default" /> means the application has no
	///     preference.
	///     <para>Auxiliary buffers are a legacy OpenGL feature and should not be used in new code.</para>
	///     <para>Default Value: <c>0</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	[Obsolete]
	AuxBuffers = 0x0002100b,

	/// <summary>
	///     Specifies whether to use stereoscopic rendering.
	///     <para>This is a hard constraint.</para>
	///     <para>Default Value: <see cref="Constants.False" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	Stereo = 0x0002100c,

	/// <summary>
	///     Specifies the desired number of samples to use for multisampling.Zero disables multisampling.
	///     <para><see cref="Constants.Default" /> means the application has no preference.</para>
	///     <para>Default Value: <c>0</c></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	Samples = 0x0002100d,

	/// <summary>
	///     Specifies whether the framebuffer should be sRGB capable. If supported, a created OpenGL context will support the
	///     GL_FRAMEBUFFER_SRGB enable, also called GL_FRAMEBUFFER_SRGB_EXT) for controlling sRGB rendering and a created
	///     OpenGL ES context will always have sRGB rendering enabled.
	///     <para>Default Value: <see cref="Constants.False" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	SrgbCapable = 0x0002100e,

	/// <summary>
	///     Specifies whether the framebuffer should be double buffered.You nearly always want to use double buffering.
	///     <para>This is a hard constraint.</para>
	///     <para>Default Value: <see cref="Constants.True" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	Doublebuffer = 0x00021010,

	/// <summary>
	///     Specifies the desired refresh rate for full screen windows.
	///     <para>If set to <see cref="Constants.Default" />, the highest available refresh rate will be used.</para>
	///     <para>This hint is ignored for windowed mode windows.</para>
	///     <para>Default Value: <see cref="Constants.Default" /></para>
	///     <para>Possible Values: <c>0</c> to <see cref="int.MaxValue" /> or <see cref="Constants.Default" />.</para>
	/// </summary>
	RefreshRate = 0x0002100f,

	/// <summary>
	///     Specifies which client API to create the context for.
	///     <para>This is a hard constraint.</para>
	///     <para>Default Value: <see cref="GLFW.ClientApi.OpenGL" /></para>
	///     <para>Possible Values: Any of <see cref="GLFW.ClientApi" /> values.</para>
	/// </summary>
	ClientApi = 0x00022001,

	/// <summary>
	///     Specifies which context creation API to use to create the context.
	///     <para>If no client API is requested, this hint is ignored.</para>
	///     <para>This is a hard constraint. </para>
	///     <para>Default Value: <see cref="ContextApi.Native" /></para>
	///     <para>Possible Values: Any of <see cref="ContextApi" /> values.</para>
	/// </summary>
	ContextCreationApi = 0x0002200b,

	/// <summary>
	///     Specify the client API major version that the created context must be compatible with.
	///     <para>The exact behavior of this hint depends on the requested client API, see remarks for details.</para>
	///     <para>Default Value: <c>1</c></para>
	///     <para>Possible Values: Any valid major version of the chosen client API</para>
	/// </summary>
	ContextVersionMajor = 0x00022002,

	/// <summary>
	///     Specify the client API minor version that the created context must be compatible with.
	///     <para>The exact behavior of this hint depends on the requested client API, see remarks for details.</para>
	///     <para>Default Value: <c>0</c></para>
	///     <para>Possible Values: Any valid minor version of the chosen client API</para>
	/// </summary>
	ContextVersionMinor = 0x00022003,

	/// <summary>
	///     Specifies the robustness strategy to be used by the context.
	///     <para>Default Value: <see cref="Robustness.None" /></para>
	///     <para>Possible Values: Any of <see cref="Robustness" /> values</para>
	/// </summary>
	ContextRobustness = 0x00022005,

	/// <summary>
	///     Specifies whether the OpenGL context should be forward-compatible, i.e. one where all functionality deprecated in
	///     the requested version of OpenGL is removed.
	///     <para>This must only be used if the requested OpenGL version is 3.0 or above.</para>
	///     <para>If OpenGL ES is requested, this hint is ignored</para>
	///     <para>Forward-compatibility is described in detail in the OpenGL Reference Manual.</para>
	///     <para>Default Value: <see cref="Constants.False" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	OpenglForwardCompatible = 0x00022006,

	/// <summary>
	///     Specifies whether to create a debug OpenGL context, which may have additional error and performance issue reporting
	///     functionality.
	///     <para>If OpenGL ES is requested, this hint is ignored.</para>
	///     <para>Default Value: <see cref="Constants.False" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	OpenglDebugContext = 0x00022007,

	/// <summary>
	///     Specifies which OpenGL profile to create the context for.
	///     <para>If requesting an OpenGL version below <c>3.2</c>, <see cref="Profile.Any" />  must be used.</para>
	///     <para>If OpenGL ES is requested, this hint is ignored.</para>
	///     <para>OpenGL profiles are described in detail in the OpenGL Reference Manual.</para>
	///     <para>Default Value: <see cref="Profile.Any" /></para>
	///     <para>Possible Values: Any of <see cref="Profile" /> values</para>
	/// </summary>
	OpenglProfile = 0x00022008,

	/// <summary>
	///     Specifies the release behavior to be used by the context.
	///     <para>Default Value: <see cref="ReleaseBehavior.Any" /></para>
	///     <para>Possible Values: Any of <see cref="ReleaseBehavior" /> values</para>
	/// </summary>
	ContextReleaseBehavior = 0x00022009,

	/// <summary>
	///     Specifies whether errors should be generated by the context. If enabled, situations that would have generated
	///     errors instead cause undefined behavior.
	///     <para>Default Value: <see cref="Constants.False" /></para>
	///     <para>Possible Values: <see cref="Constants.True" /> or <see cref="Constants.False" />.</para>
	/// </summary>
	ContextNoError = 0x0002200a,

	/// <summary>
	///     Specifies whether to also expose joystick hats as buttons, for compatibility with earlier versions of
	///     GLFW (less than 3.3) that did not have <see cref="Glfw.GetJoystickHats(int)" />.
	/// </summary>
	JoystickHatButtons = 0x00050001,

	/// <summary>
	///     Specifies whether to set the current directory to the application to the Contents/Resources
	///     subdirectory of the application's bundle, if present.
	///     <para>macOS ONLY!</para>
	/// </summary>
	CocoaChDirResources = 0x00051001,

	/// <summary>
	///     Specifies whether to create a basic menu bar, either from a nib or manually, when the first window is
	///     created, which is when AppKit is initialized.
	///     <para>macOS ONLY!</para>
	/// </summary>
	CocoaMenuBar = 0x00051002,

	/// <summary>
	///     Specifies whether the cursor should be centered over newly created full screen windows.
	///     <para>Possible values are <c>true</c> and <c>false</c>.</para>
	///     <para>This hint is ignored for windowed mode windows.</para>
	/// </summary>
	CenterCursor = 0x00020009,

	/// <summary>
	///     Specifies whether the window framebuffer will be transparent.
	///     <para>
	///         If enabled and supported by the system, the window framebuffer alpha channel will be used to combine
	///         the framebuffer with the background. This does not affect window decorations.
	///     </para>
	///     <para>Possible values are <c>true</c> and <c>false</c>.</para>
	/// </summary>
	TransparentFramebuffer = 0x0002000A,

	/// <summary>
	///     Specifies whether the window will be given input focus when <see cref="Glfw.ShowWindow" /> is called.
	///     <para>Possible values are <c>true</c> and <c>false</c>.</para>
	/// </summary>
	FocusOnShow = 0x0002000C,

	/// <summary>
	///     Specifies whether the window content area should be resized based on the monitor content scale of any
	///     monitor it is placed on. This includes the initial placement when the window is created.
	///     <para>Possible values are <c>true</c> and <c>false</c>.</para>
	///     <para>
	///         This hint only has an effect on platforms where screen coordinates and pixels always map 1:1 such as
	///         Windows and X11. On platforms like macOS the resolution of the framebuffer is changed independently
	///         of the window size.
	///     </para>
	/// </summary>
	ScaleToMonitor = 0x0002200C,

	/// <summary>
	///     Specifies whether to use full resolution framebuffers on Retina displays.
	///     <para>Possible values are <c>true</c> and <c>false</c>.</para>
	///     <para>This is ignored on other platforms.</para>
	/// </summary>
	CocoaRetinaFrameBuffer = 0x00023001,

	/// <summary>
	///     Specifies the UTF-8 encoded name to use for auto-saving the window frame, or if empty disables frame
	///     auto-saving for the window.
	///     <para>macOs only, this is ignored on other platforms.</para>
	///     <para>This is set with <see cref="Glfw.WindowHintString" />.</para>
	/// </summary>
	CocoaFrameName = 0x00023002,

	/// <summary>
	///     Specifies whether to in Automatic Graphics Switching, i.e. to allow the system to choose the integrated
	///     GPU for the OpenGL context and move it between GPUs if necessary or whether to force it to always run on
	///     the discrete GPU.
	///     <para>This only affects systems with both integrated and discrete GPUs, ignored on other platforms.</para>
	///     <para>Possible values are <c>true</c> and <c>false</c>.</para>
	/// </summary>
	CocoaGraphicsSwitching = 0x00023003,

	/// <summary>
	///     Specifies the desired ASCII encoded class parts of the ICCCM <c>WM_CLASS</c> window property.
	///     <para>Set with <see cref="Glfw.WindowHintString" />.</para>
	/// </summary>
	X11ClassName = 0x00024001,

	/// <summary>
	///     Specifies the desired ASCII encoded instance parts of the ICCCM <c>WM_CLASS</c> window property.
	///     <para>Set with <see cref="Glfw.WindowHintString" />.</para>
	/// </summary>
	X11InstanceName = 0x00024002
}

/// <summary>
///     Strongly-typed values for getting/setting the input mode hints.
/// </summary>
public enum InputMode {
	/// <summary>
	///     If specified, enables setting the mouse behavior.
	///     <para>See <see cref="CursorMode" /> for possible values.</para>
	/// </summary>
	Cursor = 0x00033001,

	/// <summary>
	///     If specified, enables setting sticky keys, where <see cref="Glfw.GetKey" /> will return
	///     <see cref="InputState.Press" /> the first time you call it for a key that was pressed, even if that key has already
	///     been released.
	/// </summary>
	StickyKeys = 0x00033002,

	/// <summary>
	///     If specified, enables setting sticky mouse buttons, where <see cref="Glfw.GetMouseButton" /> will return
	///     <see cref="InputState.Press" /> the first time you call it for a mouse button that was pressed, even if that mouse
	///     button has already been released.
	/// </summary>
	StickyMouseButton = 0x00033003,

	/// <summary>
	///     When this input mode is enabled, any callback that receives modifier bits will have the
	///     <see cref="ModifierKeys.CapsLock" /> bit set if caps lock was on when the event occurred and the
	///     <see cref="ModifierKeys.NumLock" /> bit set if num lock was on.
	/// </summary>
	LockKeyMods = 0x00033004,

	/// <summary>
	///     When the cursor is disabled, raw (unscaled and unaccelerated) mouse motion can be enabled if available.
	///     <seealso cref="Glfw.RawMouseMotionSupported" />
	/// </summary>
	RawMouseMotion = 0x00033005
}

/// <summary>
///     Describes the state of a button/key.
/// </summary>
public enum InputState : byte {
	/// <summary>
	///     The key or mouse button was released.
	/// </summary>
	Release = 0,

	/// <summary>
	///     The key or mouse button was pressed.
	/// </summary>
	Press = 1,

	/// <summary>
	///     The key was held down until it repeated.
	/// </summary>
	Repeat = 2
}

/// <summary>
///     Strongly-typed values describing possible joysticks.
/// </summary>
public enum Joystick {
	/// <summary>
	///     Joystick 1
	/// </summary>
	Joystick1 = 0,

	/// <summary>
	///     Joystick 2
	/// </summary>
	Joystick2 = 1,

	/// <summary>
	///     Joystick 3
	/// </summary>
	Joystick3 = 2,

	/// <summary>
	///     Joystick 4
	/// </summary>
	Joystick4 = 3,

	/// <summary>
	///     Joystick 5
	/// </summary>
	Joystick5 = 4,

	/// <summary>
	///     Joystick 6
	/// </summary>
	Joystick6 = 5,

	/// <summary>
	///     Joystick 7
	/// </summary>
	Joystick7 = 6,

	/// <summary>
	///     Joystick 8
	/// </summary>
	Joystick8 = 7,

	/// <summary>
	///     Joystick 9
	/// </summary>
	Joystick9 = 8,

	/// <summary>
	///     Joystick 10
	/// </summary>
	Joystick10 = 9,

	/// <summary>
	///     Joystick 11
	/// </summary>
	Joystick11 = 10,

	/// <summary>
	///     Joystick 12
	/// </summary>
	Joystick12 = 11,

	/// <summary>
	///     Joystick 13
	/// </summary>
	Joystick13 = 12,

	/// <summary>
	///     Joystick 14
	/// </summary>
	Joystick14 = 13,

	/// <summary>
	///     Joystick 15
	/// </summary>
	Joystick15 = 14,

	/// <summary>
	///     Joystick 16
	/// </summary>
	Joystick16 = 15
}

/// <summary>
///     Strongly-typed enumeration for key code values.
/// </summary>
public enum Keys {
	Unknown = -1,
	Space = 32,
	Apostrophe = 39,
	Comma = 44,
	Minus = 45,
	Period = 46,
	Slash = 47,
	Alpha0 = 48,
	Alpha1 = 49,
	Alpha2 = 50,
	Alpha3 = 51,
	Alpha4 = 52,
	Alpha5 = 53,
	Alpha6 = 54,
	Alpha7 = 55,
	Alpha8 = 56,
	Alpha9 = 57,
	SemiColon = 59,
	Equal = 61,
	A = 65,
	B = 66,
	C = 67,
	D = 68,
	E = 69,
	F = 70,
	G = 71,
	H = 72,
	I = 73,
	J = 74,
	K = 75,
	L = 76,
	M = 77,
	N = 78,
	O = 79,
	P = 80,
	Q = 81,
	R = 82,
	S = 83,
	T = 84,
	U = 85,
	V = 86,
	W = 87,
	X = 88,
	Y = 89,
	Z = 90,
	LeftBracket = 91,
	Backslash = 92,
	RightBracket = 93,
	GraveAccent = 96,
	World1 = 161,
	World2 = 162,
	Escape = 256,
	Enter = 257,
	Tab = 258,
	Backspace = 259,
	Insert = 260,
	Delete = 261,
	Right = 262,
	Left = 263,
	Down = 264,
	Up = 265,
	PageUp = 266,
	PageDown = 267,
	Home = 268,
	End = 269,
	CapsLock = 280,
	ScrollLock = 281,
	NumLock = 282,
	PrintScreen = 283,
	Pause = 284,
	F1 = 290,
	F2 = 291,
	F3 = 292,
	F4 = 293,
	F5 = 294,
	F6 = 295,
	F7 = 296,
	F8 = 297,
	F9 = 298,
	F10 = 299,
	F11 = 300,
	F12 = 301,
	F13 = 302,
	F14 = 303,
	F15 = 304,
	F16 = 305,
	F17 = 306,
	F18 = 307,
	F19 = 308,
	F20 = 309,
	F21 = 310,
	F22 = 311,
	F23 = 312,
	F24 = 313,
	F25 = 314,
	Numpad0 = 320,
	Numpad1 = 321,
	Numpad2 = 322,
	Numpad3 = 323,
	Numpad4 = 324,
	Numpad5 = 325,
	Numpad6 = 326,
	Numpad7 = 327,
	Numpad8 = 328,
	Numpad9 = 329,
	NumpadDecimal = 330,
	NumpadDivide = 331,
	NumpadMultiply = 332,
	NumpadSubtract = 333,
	NumpadAdd = 334,
	NumpadEnter = 335,
	NumpadEqual = 336,
	LeftShift = 340,
	LeftControl = 341,
	LeftAlt = 342,
	LeftSuper = 343,
	RightShift = 344,
	RightControl = 345,
	RightAlt = 346,
	RightSuper = 347,
	Menu = 348
}

/// <summary>
///     Describes bitwise combination of modifier keys.
/// </summary>
[Flags]
public enum ModifierKeys {
	/// <summary>
	///     Either of the Shift keys.
	/// </summary>
	Shift = 0x0001,

	/// <summary>
	///     Either of the Ctrl keys.
	/// </summary>
	Control = 0x0002,

	/// <summary>
	///     Either of the Alt keys
	/// </summary>
	Alt = 0x0004,

	/// <summary>
	///     The super key ("Windows" key on Windows)
	/// </summary>
	Super = 0x0008,

	/// <summary>
	///     The caps-lock is enabled.
	/// </summary>
	CapsLock = 0x0010,

	/// <summary>
	///     The num-lock is enabled.
	/// </summary>
	NumLock = 0x0020
}

/// <summary>
///     Strongly-typed enumeration describing mouse buttons.
/// </summary>
public enum MouseButton {
	/// <summary>
	///     Mouse button 1.
	///     <para>Same as <see cref="Left" />.</para>
	/// </summary>
	Button1 = 0,

	/// <summary>
	///     Mouse button 2.
	///     <para>Same as <see cref="Right" />.</para>
	/// </summary>
	Button2 = 1,

	/// <summary>
	///     Mouse button 3.
	///     <para>Same as <see cref="Middle" />.</para>
	/// </summary>
	Button3 = 2,

	/// <summary>
	///     Mouse button 4.
	/// </summary>
	Button4 = 3,

	/// <summary>
	///     Mouse button 4.
	/// </summary>
	Button5 = 4,

	/// <summary>
	///     Mouse button 5.
	/// </summary>
	Button6 = 5,

	/// <summary>
	///     Mouse button 6.
	/// </summary>
	Button7 = 6,

	/// <summary>
	///     Mouse button 7.
	/// </summary>
	Button8 = 7,

	/// <summary>
	///     The left mouse button.
	///     <para>Same as <see cref="Button1" />.</para>
	/// </summary>
	Left = Button1,

	/// <summary>
	///     The right mouse button.
	///     <para>Same as <see cref="Button2" />.</para>
	/// </summary>
	Right = Button2,

	/// <summary>
	///     The middle mouse button.
	///     <para>Same as <see cref="Button3" />.</para>
	/// </summary>
	Middle = Button3
}

/// <summary>
///     Strongly-typed values used for getting/setting window hints.
///     <para>If OpenGL ES is requested, this hint is ignored.</para>
/// </summary>
public enum Profile {
	/// <summary>
	///     Indicates no preference on profile.
	///     <para>If requesting an OpenGL version below 3.2, this profile must be used.</para>
	/// </summary>
	Any = 0x00000000,

	/// <summary>
	///     Indicates OpenGL Core profile.
	///     <para>Only if requested OpenGL is greater than 3.2.</para>
	/// </summary>
	Core = 0x00032001,

	/// <summary>
	///     Indicates OpenGL Compatibility profile.
	///     <para>Only if requested OpenGL is greater than 3.2.</para>
	/// </summary>
	Compatibility = 0x00032002
}

/// <summary>
///     Describes the release behavior to be used by the context.
/// </summary>
public enum ReleaseBehavior {
	/// <summary>
	///     The default behavior of the context creation API will be used.
	/// </summary>
	Any = 0,

	/// <summary>
	///     The pipeline will be flushed whenever the context is released from being the current one.
	/// </summary>
	Flush = 0x00035001,

	/// <summary>
	///     The pipeline will not be flushed on release.
	/// </summary>
	None = 0x00035002
}

/// <summary>
///     Describes the robustness strategy to be used by the context.
/// </summary>
public enum Robustness {
	/// <summary>
	///     Disabled/no strategy (default)
	/// </summary>
	None = 0,

	/// <summary>
	///     No notification.
	/// </summary>
	NoResetNotification = 0x00031001,

	/// <summary>
	///     The context is lost on reset, use glGetGraphicsResetStatus for more information.
	/// </summary>
	LoseContextOnReset = 0x00031002
}

/// <summary>
///     Strongly-typed values used for getting/setting window hints.
/// </summary>
public enum WindowAttribute {
	/// <summary>
	///     Indicates whether the windowed mode window will be given input focus when created.
	///     <para>This hint is ignored for full screen and initially hidden windows.</para>
	/// </summary>
	Focused = 0x00020001,

	/// <summary>
	///     Indicates whether the full screen window will automatically iconify and restore the previous video mode on input
	///     focus loss.
	///     <para>This hint is ignored for windowed mode windows.</para>
	/// </summary>
	AutoIconify = 0x00020002,

	/// <summary>
	///     Indicates whether the windowed mode window will be maximized when created.
	///     <para>This hint is ignored for full screen windows.</para>
	/// </summary>
	Maximized = 0x00020008,

	/// <summary>
	///     Indicates whether the windowed mode window will be initially visible.
	///     <para>This hint is ignored for full screen windows.</para>
	/// </summary>
	Visible = 0x00020004,

	/// <summary>
	///     Indicates whether the windowed mode window will be resizable by the <i>user</i>.
	///     <para>The window will still be resizable using the <see cref="Glfw.SetWindowSize" /> function.</para>
	///     <para>This hint is ignored for full screen windows.</para>
	/// </summary>
	Resizable = 0x00020003,

	/// <summary>
	///     Indicates whether the windowed mode window will have window decorations such as a border, a close widget, etc.
	///     <para>An undecorated window may still allow the user to generate close events on some platforms.</para>
	///     <para>This hint is ignored for full screen windows.</para>
	/// </summary>
	Decorated = 0x00020005,

	/// <summary>
	///     Indicates whether the windowed mode window will be floating above other regular windows, also called topmost or
	///     always-on-top.
	///     <para>This is intended primarily for debugging purposes and cannot be used to implement proper full screen windows.</para>
	///     <para>This hint is ignored for full screen windows.</para>
	/// </summary>
	Floating = 0x00020007,

	/// <summary>
	///     Indicates whether the cursor is currently directly over the content area of the window, with no other
	///     windows between.
	/// </summary>
	MouseHover = 0x0002000B
}
