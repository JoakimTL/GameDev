using Engine.Rendering.Contexts.Input.StateStructs;
using GlfwBinding.Enums;

namespace Engine.Rendering.Contexts.Input;

//MOUSE
public delegate void MouseButtonHandler(MouseButton button, ModifierKeys modifiers);
public delegate void MouseScrollHandler(double xAxis, double yAxis);
public delegate void MouseMoveHandler(double xAxis, double yAxis);
public delegate void MouseEnterHandler( bool entered );
//TIMED MOUSE
public delegate void TimedMouseButtonHandler(double time, MouseButtonEventState state);
public delegate void TimedMouseScrollHandler(double time, Point2d scroll);
public delegate void TimedMouseStateChangeHandler(double time, bool state);
public delegate void TimedMouseMoveHandler(double time, MousePointerState state, bool lockState);
//KEYBOARD
public delegate void KeyboardHandler(Keys key, ModifierKeys mods, int scanCode);
public delegate void KeyboardCharHandler(uint charCode, string character, ModifierKeys mods);
//WINDOW
public delegate void FileDropHandler(string[] filePaths);
public delegate void WindowSizeHandler(int width, int height);
public delegate void WindowContentScaleHandler(float xScale, float yScale);
public delegate void WindowPositionHandler(double x, double y);
public delegate void WindowBooleanValueHandler(bool boolValue);
public delegate void WindowEventHandler();
