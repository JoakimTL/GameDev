using Engine.Module.Render.Glfw.Enums;

namespace Engine.Module.Render.Input;

//MOUSE
public delegate void MouseButtonHandler( MouseButton button, ModifierKeys modifiers );
public delegate void MouseScrollHandler( double xAxis, double yAxis );
public delegate void MouseMoveHandler( double xAxis, double yAxis );
public delegate void MouseEnterHandler( bool entered );
//KEYBOARD
public delegate void KeyboardHandler( Keys key, ModifierKeys mods, int scanCode );
public delegate void KeyboardCharHandler( uint charCode, ModifierKeys mods );
//WINDOW
public delegate void FileDropHandler( string[] filePaths );
public delegate void WindowSizeHandler( int width, int height );
public delegate void WindowContentScaleHandler( float xScale, float yScale );
public delegate void WindowPositionHandler( double x, double y );
public delegate void WindowBooleanValueHandler( bool boolValue );
public delegate void WindowEventHandler();

//MOUSE
public delegate void TimedMouseButtonHandler( MouseButtonEvent @event );
public delegate void TimedMouseScrollHandler( MouseWheelEvent @event );
public delegate void TimedMouseMoveHandler( MouseMoveEvent @event );
public delegate void TimedMouseEnterHandler( MouseEnterEvent @event );
//KEYBOARD
public delegate void TimedKeyboardHandler( KeyboardEvent @event );
public delegate void TimedKeyboardCharHandler( KeyboardCharacterEvent @event );