using GLFW;

namespace Engine.Rendering.Input;

public abstract class MouseEventListener : IMouseEventListener {

	/// <summary>
	/// Whenever the cursor is moved in a free state.<br/>
	/// This means the user is actively moving their mouse cursor around on the screen, and not controlling something directly with their mouse.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="state">The current state of the mouse pointer.</param>
	public virtual void OnMouseCursorMove( MouseState state ) {

	}

	/// <summary>
	/// Whenever the cursor is moved in a locked state.<br/>
	/// This means the user is usually controlling something directly with their mouse, and not moving their mouse cursor around on the screen.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="state">The current state of the mouse pointer.</param>
	public virtual void OnMouseLockedMove( MouseState state ) {

	}

	/// <summary>
	/// Whenever a mouse button is pressed
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="button">The mouse button in question.</param>
	/// <param name="modifiers">The modifiers active as this event takes place.</param>
	public virtual void OnButtonPressed( MouseButton button, ModifierKeys modifiers, MouseState state ) {

	}

	/// <summary>
	/// Whenever a mouse button is released
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="button">The mouse button in question.</param>
	/// <param name="modifiers">The modifiers active as this event takes place.</param>
	public virtual void OnButtonReleased( MouseButton button, ModifierKeys modifiers, MouseState state ) {

	}

	/// <summary>
	/// Whenever a mouse button is held down and the OS repeats the button event.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="button">The mouse button in question.</param>
	/// <param name="modifiers">The modifiers active as this event takes place.</param>
	public virtual void OnButtonRepeat( MouseButton button, ModifierKeys modifiers, MouseState state ) {

	}

	/// <summary>
	/// When the mouse scroll wheel is moved.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <param name="xAxis">The sideways movement on the mouse wheel.</param>
	/// <param name="yAxis">The scrolling movement on the mouse wheel.</param>
	public virtual void OnMouseScroll( double xAxis, double yAxis, MouseState state ) {

	}

}
