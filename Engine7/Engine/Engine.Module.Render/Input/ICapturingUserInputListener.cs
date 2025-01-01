namespace Engine.Module.Render.Input;

/// <summary>
/// Used when a hierarchy of objects needs to listen to user input.<br />
/// Example: User interface elements takes priority over the game world.<br />
/// User <see cref="Processing.Do{TProcessType}.BeforeAttribute{TBeforeType}"/> and <see cref="Processing.Do{TProcessType}.AfterAttribute{TAfterType}"/> where TProcessType is <see cref="ICapturingUserInputListener"/> to process user input.
/// </summary>
public interface ICapturingUserInputListener {
	/// <returns>True if this listener captured the event.</returns>
	bool OnMouseButton( MouseButtonEvent @event );
	/// <returns>True if this listener captured the event.</returns>
	bool OnMouseWheelScrolled( MouseWheelEvent @event );
	/// <returns>True if this listener captured the event.</returns>
	bool OnMouseMoved( MouseMoveEvent @event );
	/// <returns>True if this listener captured the event.</returns>
	bool OnMouseEnter( MouseEnterEvent @event );
	/// <returns>True if this listener captured the event.</returns>
	bool OnKey( KeyboardEvent @event );
	/// <returns>True if this listener captured the event.</returns>
	bool OnCharacter( KeyboardCharacterEvent @event );
}