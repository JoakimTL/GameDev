using Engine.Module.Render.Input;

namespace Engine.Standard.Render.UserInterface;

public abstract class UserInterfaceComponentBase( UserInterfaceElementBase element ) : DisposableIdentifiable {
	protected UserInterfaceElementBase Element { get; } = element;

	internal protected virtual bool OnCharacter( KeyboardCharacterEvent @event ) => false;
	internal protected virtual bool OnKey( KeyboardEvent @event ) => false;
	internal protected virtual bool OnMouseButton( MouseButtonEvent @event ) => false;
	internal protected virtual bool OnMouseEnter( MouseEnterEvent @event ) => false;
	internal protected virtual bool OnMouseMoved( MouseMoveEvent @event ) => false;
	internal protected virtual bool OnMouseWheelScrolled( MouseWheelEvent @event ) => false;
	internal protected abstract void Update( double time, double deltaTime );
}