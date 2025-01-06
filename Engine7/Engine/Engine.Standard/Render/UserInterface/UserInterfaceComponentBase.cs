using Engine.Module.Render.Input;
using Engine.Transforms.Models;

namespace Engine.Standard.Render.UserInterface;

public abstract class UserInterfaceComponentBase : DisposableIdentifiable {
	protected UserInterfaceElementBase Element { get; }

	protected readonly UserInterfaceComponentPlacement _userInterfaceComponentPlacement;
	protected Transform2<double> Transform { get; }
	public TransformInterface<double, Vector2<double>, double, Vector2<double>> TransformInterface { get; }

	public AABB<Vector2<double>> PlacementBounds { get; private set; }
	public event Action? PlacementBoundsChanged;

	public UserInterfaceComponentBase( UserInterfaceElementBase element ) {
		this.Element = element;
		Transform = new();
		TransformInterface = Transform.Interface;
		_userInterfaceComponentPlacement = new( this );
	}

	internal void UiSpaceChanged( Vector2<double> newAspectVector ) {
		PlacementBounds = new AABB<Vector2<double>>( -newAspectVector, newAspectVector );
		PlacementBoundsChanged?.Invoke();
	}

	internal protected virtual bool OnCharacter( KeyboardCharacterEvent @event ) => false;
	internal protected virtual bool OnKey( KeyboardEvent @event ) => false;
	internal protected virtual bool OnMouseButton( MouseButtonEvent @event ) => false;
	internal protected virtual bool OnMouseEnter( MouseEnterEvent @event ) => false;
	internal protected virtual bool OnMouseMoved( MouseMoveEvent @event ) => false;
	internal protected virtual bool OnMouseWheelScrolled( MouseWheelEvent @event ) => false;
	internal void Update( double time, double deltaTime ) {
		if (_userInterfaceComponentPlacement.Update())
			OnPlacementChanged();
		OnUpdate( time, deltaTime );
	}

	protected abstract void OnPlacementChanged();

	protected abstract void OnUpdate( double time, double deltaTime );
}