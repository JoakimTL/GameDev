using Engine.Module.Render.Input;

namespace Engine.Standard.Render.UserInterface;

public abstract class UserInterfaceElementBase : DisposableIdentifiable {

	private readonly List<UserInterfaceComponentBase> _components = [];

	public UserInterfaceServiceAccess UserInterfaceServiceAccess { get; private set; } = null!;

	public bool IsDisplayed { get; private set; }

	public void UpdateDisplayState( GameStateProvider gameStateProvider ) 
		=> this.IsDisplayed = this.ShouldDisplay( gameStateProvider );

	/// <summary>
	/// Determines if the element should be displayed based on the current game state.
	/// </summary>
	protected abstract bool ShouldDisplay( GameStateProvider gameStateProvider );

	protected void AddComponent( UserInterfaceComponentBase component ) 
		=> this._components.Add( component );

	internal protected abstract void Initialize( GameStateProvider gameStateProvider );

	internal bool OnMouseButton( MouseButtonEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnMouseButton( @event ))
				return true;
		}
		return false;
	}

	internal bool OnMouseWheelScrolled( MouseWheelEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnMouseWheelScrolled( @event ))
				return true;
		}
		return false;
	}

	internal bool OnMouseMoved( MouseMoveEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnMouseMoved( @event ))
				return true;
		}
		return false;
	}

	internal bool OnMouseEnter( MouseEnterEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnMouseEnter( @event ))
				return true;
		}
		return false;
	}

	internal bool OnKey( KeyboardEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnKey( @event ))
				return true;
		}
		return false;
	}

	internal bool OnCharacter( KeyboardCharacterEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnCharacter( @event ))
				return true;
		}
		return false;
	}

	internal void Update( double time, double deltaTime ) {
		foreach (UserInterfaceComponentBase component in this._components)
			component.Update( time, deltaTime );
	}

	internal void SetServiceAccess( UserInterfaceServiceAccess userInterfaceServiceAccess ) 
		=> this.UserInterfaceServiceAccess = userInterfaceServiceAccess;

	protected override bool InternalDispose() {
		foreach (UserInterfaceComponentBase component in this._components)
			component.Dispose();
		return true;
	}
}
