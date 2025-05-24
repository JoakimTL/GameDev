using Engine.Module.Render.Input;

namespace Engine.Standard.Render.UserInterface;
public abstract class UserInterfaceElementBase( uint baseLayer = 0 ) : DisposableIdentifiable {

	private readonly List<UserInterfaceComponentBase> _components = [];

	public UserInterfaceServiceAccess UserInterfaceServiceAccess { get; private set; } = null!;
	public GameStateProvider GameStateProvider { get; private set; } = null!;

	public bool IsDisplayed { get; private set; }

	public uint BaseLayer { get; } = baseLayer;

	internal protected abstract void Initialize();
	protected abstract void OnUpdate( double time, double deltaTime );

	/// <summary>
	/// Determines if the element should be displayed based on the current game state.
	/// </summary>
	protected abstract bool ShouldDisplay();

	public bool UpdateDisplayState() {
		bool oldValue = this.IsDisplayed;
		this.IsDisplayed = this.ShouldDisplay();
		return oldValue != this.IsDisplayed;
	}

	internal void AddComponent( UserInterfaceComponentBase component ) => this._components.Add( component );

	internal void RemoveComponent( UserInterfaceComponentBase component ) => this._components.Remove( component );

	internal void Hide() {
		for (int i = 0; i < this._components.Count; i++) {
			UserInterfaceComponentBase component = this._components[ i ];
			if (component.Parent is null)
				component.Hide();
		}
	}

	internal void Show() {
		for (int i = 0; i < this._components.Count; i++) {
			UserInterfaceComponentBase component = this._components[ i ];
			if (component.Parent is null)
				component.Show();
		}
	}

	internal void UiSpaceChanged( Vector2<double> newAspectVector ) {
		for (int i = 0; i < this._components.Count; i++) {
			UserInterfaceComponentBase component = this._components[ i ];
			if (component.Parent is null)
				component.UiSpaceChanged( newAspectVector );
		}
	}

	internal bool OnMouseButton( MouseButtonEvent @event ) {
		for (int i = 0; i < this._components.Count; i++) {
			UserInterfaceComponentBase component = this._components[ i ];
			if (component.Parent is null && component.Visible)
				if (component.OnMouseButton( @event ))
					return true;
		}
		return false;
	}

	internal bool OnMouseWheelScrolled( MouseWheelEvent @event ) {
		for (int i = 0; i < this._components.Count; i++) {
			UserInterfaceComponentBase component = this._components[ i ];
			if (component.Parent is null && component.Visible)
				if (component.OnMouseWheelScrolled( @event ))
					return true;
		}
		return false;
	}

	internal bool OnMouseMoved( MouseMoveEvent @event ) {
		for (int i = 0; i < this._components.Count; i++) {
			UserInterfaceComponentBase component = this._components[ i ];
			if (component.Parent is null && component.Visible)
				if (component.OnMouseMoved( @event ))
					return true;
		}
		return false;
	}

	internal bool OnMouseEnter( MouseEnterEvent @event ) {
		for (int i = 0; i < this._components.Count; i++) {
			UserInterfaceComponentBase component = this._components[ i ];
			if (component.Parent is null && component.Visible)
				if (component.OnMouseEnter( @event ))
					return true;
		}
		return false;
	}

	internal bool OnKey( KeyboardEvent @event ) {
		for (int i = 0; i < this._components.Count; i++) {
			UserInterfaceComponentBase component = this._components[ i ];
			if (component.Parent is null && component.Visible)
				if (component.OnKey( @event ))
					return true;
		}
		return false;
	}

	internal bool OnCharacter( KeyboardCharacterEvent @event ) {
		for (int i = 0; i < this._components.Count; i++) {
			UserInterfaceComponentBase component = this._components[ i ];
			if (component.Parent is null && component.Visible)
				if (component.OnCharacter( @event ))
					return true;
		}
		return false;
	}

	internal void Update( double time, double deltaTime ) {
		OnUpdate( time, deltaTime );
		for(int i  = 0; i < this._components.Count; i++) {
			UserInterfaceComponentBase component = this._components[ i ];
			if (component.Parent is null)
				component.Update( time, deltaTime );
		}
	}

	internal void SetServices( UserInterfaceServiceAccess userInterfaceServiceAccess, GameStateProvider gameStateProvider ) {
		this.UserInterfaceServiceAccess = userInterfaceServiceAccess;
		this.GameStateProvider = gameStateProvider;
	}

	protected override bool InternalDispose() {
		UserInterfaceComponentBase[] components = [ .. this._components ];
		foreach (UserInterfaceComponentBase component in components)
			if (component.Parent is null)
				component.Remove();
		return true;
	}
}
