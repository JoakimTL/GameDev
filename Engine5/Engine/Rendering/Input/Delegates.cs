using GLFW;

namespace Engine.Rendering.Input;

//MOUSE
public delegate void MouseButtonHandler( MouseButton button, ModifierKeys modifiers, MouseState state );
public delegate void MouseScrollHandler( double xAxis, double yAxis, MouseState state );
public delegate void MouseMoveHandler( MouseState state );
//KEYBOARD
public delegate void KeyboardHandler( Keys key, ModifierKeys mods, int scanCode );
public delegate void KeyboardCharHandler( uint charCode, string character, ModifierKeys mods );
//WINDOW
public delegate void FileDropHandler( string[] filePaths );
public delegate void WindowSizeHandler( int width, int height );
public delegate void WindowContentScaleHandler( float xScale, float yScale );
public delegate void WindowPositionHandler( double x, double y );
public delegate void WindowBooleanValueHandler( bool boolValue );
public delegate void WindowEventHandler();
