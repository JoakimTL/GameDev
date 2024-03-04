using Engine.Modules.Rendering.Glfw.Enums;
using System.Runtime.InteropServices;

namespace Engine.Modules.Rendering.Ogl.Glfw;

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void ErrorCallback( ErrorCode code, nint message );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void SizeCallback( nint windowPtr, int width, int height );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void PositionCallback( nint windowPtr, double x, double y );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void FocusCallback( nint windowPtr, bool focusing );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void WindowCallback( nint windowPtr );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void FileDropCallback( nint windowPtr, int count, nint arrayPtr );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void MouseCallback( nint windowPtr, double x, double y );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void MouseEnterCallback( nint windowPtr, bool entering );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void MouseButtonCallback( nint windowPtr, MouseButton button, InputState state, ModifierKeys modifiers );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void CharCallback( nint windowPtr, uint codePoint );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void CharModsCallback( nint windowPtr, uint codePoint, ModifierKeys mods );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void KeyCallback( nint windowPtr, Keys key, int scanCode, InputState state, ModifierKeys mods );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void JoystickCallback( Joystick joystick, ConnectionStatus status );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void MonitorCallback( nint monitorPtr, ConnectionStatus status );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void IconifyCallback( nint windowPtr, bool focusing );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void WindowContentsScaleCallback( nint windowPtr, float xScale, float yScale );

[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
public delegate void WindowMaximizedCallback( nint windowPtr, bool maximized );
