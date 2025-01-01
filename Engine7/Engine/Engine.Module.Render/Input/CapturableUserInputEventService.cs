namespace Engine.Module.Render.Input;

public sealed class CapturableUserInputEventService : DisposableIdentifiable {
	private readonly UserInputEventService _userInputEventService;
	private readonly Structures.TypeDigraph<ICapturingUserInputListener> _sortedListenerTypes;
	private readonly Dictionary<Type, List<ICapturingUserInputListener>> _listeners;

	public CapturableUserInputEventService( UserInputEventService userInputEventService ) {
		this._userInputEventService = userInputEventService;
		this._sortedListenerTypes = new();
		this._listeners = [];
		this._userInputEventService.OnMouseButton += OnMouseButton;
		this._userInputEventService.OnMouseWheelScrolled += OnMouseWheelScrolled;
		this._userInputEventService.OnMouseMoved += OnMouseMoved;
		this._userInputEventService.OnMouseEnter += OnMouseEnter;
		this._userInputEventService.OnKey += OnKey;
		this._userInputEventService.OnCharacter += OnCharacter;
	}

	private void OnMouseButton( MouseButtonEvent @event ) {
		foreach (Type type in _sortedListenerTypes.GetTypes()) {
			if (!_listeners.TryGetValue( type, out List<ICapturingUserInputListener>? list ))
				continue;
			foreach (ICapturingUserInputListener listener in list) {
				if (listener.OnMouseButton( @event ))
					return;
			}
		}
	}

	private void OnMouseWheelScrolled( MouseWheelEvent @event ) {
		foreach (Type type in _sortedListenerTypes.GetTypes()) {
			if (!_listeners.TryGetValue( type, out List<ICapturingUserInputListener>? list ))
				continue;
			foreach (ICapturingUserInputListener listener in list) {
				if (listener.OnMouseWheelScrolled( @event ))
					return;
			}
		}
	}

	private void OnMouseMoved( MouseMoveEvent @event ) {
		foreach (Type type in _sortedListenerTypes.GetTypes()) {
			if (!_listeners.TryGetValue( type, out List<ICapturingUserInputListener>? list ))
				continue;
			foreach (ICapturingUserInputListener listener in list) {
				if (listener.OnMouseMoved( @event ))
					return;
			}
		}
	}

	private void OnMouseEnter( MouseEnterEvent @event ) {
		foreach (Type type in _sortedListenerTypes.GetTypes()) {
			if (!_listeners.TryGetValue( type, out List<ICapturingUserInputListener>? list ))
				continue;
			foreach (ICapturingUserInputListener listener in list) {
				if (listener.OnMouseEnter( @event ))
					return;
			}
		}
	}

	private void OnKey( KeyboardEvent @event ) {
		foreach (Type type in _sortedListenerTypes.GetTypes()) {
			if (!_listeners.TryGetValue( type, out List<ICapturingUserInputListener>? list ))
				continue;
			foreach (ICapturingUserInputListener listener in list) {
				if (listener.OnKey( @event ))
					return;
			}
		}
	}

	private void OnCharacter( KeyboardCharacterEvent @event ) {
		foreach (Type type in _sortedListenerTypes.GetTypes()) {
			if (!_listeners.TryGetValue( type, out List<ICapturingUserInputListener>? list ))
				continue;
			foreach (ICapturingUserInputListener listener in list) {
				if (listener.OnCharacter( @event ))
					return;
			}
		}
	}

	public void AddListener( ICapturingUserInputListener listener ) {
		if (!_listeners.TryGetValue( listener.GetType(), out List<ICapturingUserInputListener>? list ))
			_listeners.Add( listener.GetType(), list = [] );
		list.Add( listener );
		this._sortedListenerTypes.Add( listener.GetType() );
	}

	public void RemoveListener( ICapturingUserInputListener listener ) {
		if (!_listeners.TryGetValue( listener.GetType(), out List<ICapturingUserInputListener>? list ))
			return;
		list.Remove( listener );
		if (list.Count == 0) {
			_sortedListenerTypes.Remove( listener.GetType() );
			_listeners.Remove( listener.GetType() );
		}
	}

	protected override bool InternalDispose() {
		this._userInputEventService.OnMouseButton -= OnMouseButton;
		this._userInputEventService.OnMouseWheelScrolled -= OnMouseWheelScrolled;
		this._userInputEventService.OnMouseMoved -= OnMouseMoved;
		this._userInputEventService.OnMouseEnter -= OnMouseEnter;
		this._userInputEventService.OnKey -= OnKey;
		this._userInputEventService.OnCharacter -= OnCharacter;
		return true;
	}
}
