namespace Engine.Modularity.ECS;

public abstract class Component : Identifiable {
	private Entity? _parent;
	public Entity Parent => this._parent ?? throw new NullReferenceException( "Component does not belong to any entity." );
	public event Action<Component>? Changed;

	internal void SetEntity( Entity e ) {
		if ( this._parent is not null )
			this.LogWarning( "Can't set entity manually." );
		this._parent = e;
		ParentSet();
	}

	internal void Removed() => RemovedFromParent();
	protected void TriggerChanged() => Changed?.Invoke( this );
	protected virtual void ParentSet() { }
	protected virtual void RemovedFromParent() { }
}
