namespace Engine.Module.Entities;

public abstract class ComponentBase {
	//Components can change, there should be events for this.
	public event ComponentChangeHandler? ComponentChanged;

	private Entity? _entity;
	public Entity Entity => this._entity ?? throw new InvalidOperationException( "Component is not attached to an entity." );

	internal void SetEntity( Entity entity ) {
		if (this._entity is not null)
			throw new InvalidOperationException( "Component owner can't be changed." );
		this._entity = entity;
	}

	internal void AddChangedListener( ComponentChangeHandler handler ) => ComponentChanged += handler;
	internal void RemoveChangedListener( ComponentChangeHandler handler ) => ComponentChanged -= handler;

	protected void InvokeComponentChanged() => ComponentChanged?.Invoke( this );
}

public interface IMessageReadingComponent {
	void ReadMessage( object message );
}