namespace Engine.Modules.Entity;

public abstract class ComponentBase : Identifiable {

	public delegate void ComponentEventHandler( ComponentBase component );

	public Entity Entity { get; private set; } = null!;

	public event ComponentEventHandler? EntitySet;
	/// <summary>
	/// Trigger anytime the component deems itself changed.
	/// </summary>
	public event ComponentEventHandler? ComponentChanged;

	internal void SetParent( Entity e ) {
		this.Entity = e;
		EntitySet?.Invoke( this );
	}

	protected void TriggerChanged() {
		ComponentChanged?.Invoke( this );
	}

	//USing SpinWait to control timing on modules?

}
