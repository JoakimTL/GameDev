using Engine.Logging;
using Engine.Module.Render.Input;

namespace Engine.Standard.Render.UserInterface;
public sealed class UserInterfaceStateManager( UserInterfaceServiceAccess userInterfaceServiceAccess, GameStateProvider gameStateProvider ) : DisposableIdentifiable, ICapturingUserInputListener, IUpdateable {

	private readonly List<UserInterfaceElementBase> _baseElements = [];
	private readonly Queue<UserInterfaceElementBase> _elementsToInitialize = [];
	private readonly UserInterfaceServiceAccess _userInterfaceServiceAccess = userInterfaceServiceAccess;
	private readonly GameStateProvider _gameStateProvider = gameStateProvider;

	private Vector2<float> _aspectRatioVector = 0;

	public void AddAllElements() {
		IEnumerable<Type> types = TypeManager.Registry.ImplementationTypes.Where( type => type.IsAssignableTo( typeof( UserInterfaceElementBase ) ) );
		foreach (Type type in types) {
			object? instance = type.Resolve().CreateInstance( null ) ?? throw new InvalidOperationException( $"Type {type.Name} does not have a parameterless constructor." );
			if (instance is not UserInterfaceElementBase element)
				throw new InvalidOperationException( $"Type {type.Name} does not inherit from {nameof( UserInterfaceElementBase )}." );
			this.LogLine( $"Adding UI element {type.Name}.", Log.Level.NORMAL );
			AddElement( element );
		}
	}

	public void AddElement<T>() where T : UserInterfaceElementBase, new()
		=> this.AddElement( new T() );

	private void AddElement( UserInterfaceElementBase element ) {
		this._baseElements.Add( element );
		this._elementsToInitialize.Enqueue( element );
		element.SetServices( _userInterfaceServiceAccess, _gameStateProvider );
	}

	public bool OnCharacter( KeyboardCharacterEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnCharacter( @event ))
				return true;
		}
		return false;
	}

	public bool OnKey( KeyboardEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnKey( @event ))
				return true;
		}
		return false;
	}

	public bool OnMouseButton( MouseButtonEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnMouseButton( @event ))
				return true;
		}
		return false;
	}

	public bool OnMouseEnter( MouseEnterEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnMouseEnter( @event ))
				return true;
		}
		return false;
	}

	public bool OnMouseMoved( MouseMoveEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnMouseMoved( @event ))
				return true;
		}
		return false;
	}

	public bool OnMouseWheelScrolled( MouseWheelEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnMouseWheelScrolled( @event ))
				return true;
		}
		return false;
	}

	public void Update( double time, double deltaTime ) {
		while (this._elementsToInitialize.Count > 0) {
			UserInterfaceElementBase element = this._elementsToInitialize.Dequeue();
			element.Initialize();
		}
		if (_userInterfaceServiceAccess.WindowProvider.Window.AspectRatioVector != _aspectRatioVector) {
			_aspectRatioVector = _userInterfaceServiceAccess.WindowProvider.Window.AspectRatioVector;
			foreach (UserInterfaceElementBase element in this._baseElements)
				element.UiSpaceChanged( _aspectRatioVector.CastSaturating<float, double>() );
		}
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (element.UpdateDisplayState()) {
				if (element.IsDisplayed) {
					element.Show();
				} else {
					element.Hide();
				}
			}
			if (element.IsDisplayed)
				element.Update( time, deltaTime );
		}
	}

	protected override bool InternalDispose() {
		foreach (UserInterfaceElementBase element in this._baseElements)
			element.Dispose();
		return true;
	}
}
