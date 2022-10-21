using GLFW;

namespace Engine.Rendering.Input;

public interface IKeyChangeEventListener {
	void OnKeyReleased( Keys key, ModifierKeys mods, int scanCode );
	void OnKeyPressed( Keys key, ModifierKeys mods, int scanCode );
	void OnKeyRepeated( Keys key, ModifierKeys mods, int scanCode );
}

public interface IWritingEventListener {
	void OnCharacterWritten( uint charCode, string character, ModifierKeys mods );
}

public interface IButtonChangeEventListener {
	void OnButtonReleased( MouseButton btn, ModifierKeys modifier, MouseState state );
	void OnButtonPressed( MouseButton btn, ModifierKeys modifier, MouseState state );
	void OnButtonRepeat( MouseButton btn, ModifierKeys modifier, MouseState state );
}

public interface IMouseChangeEventListener {
	void OnMouseCursorMove( MouseState state );
	void OnMouseLockedMove( MouseState state );
}

public interface IWheelScrollChangeEventListener {
	void OnMouseScroll( double xAxis, double yAxis, MouseState state );
}

public interface IKeyboardEventListener : IKeyChangeEventListener, IWritingEventListener { }
public interface IMouseEventListener : IButtonChangeEventListener, IMouseChangeEventListener, IWheelScrollChangeEventListener { }
public interface IEventListener : IKeyboardEventListener, IMouseEventListener { }
