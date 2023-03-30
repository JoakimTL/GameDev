namespace Engine.GameLogic.ECPS;
public abstract class ComponentBase : Identifiable {
	public Entity? Owner { get; private set; }
	public event EntityComponentEvent? ComponentChanged;

	internal void SetOwner( Entity? e )
	{
		if (Owner is not null)
			Owner.ParentChanged -= OnOwnerParentChanged;
		Owner = e;
		OnOwnerChanged();
		if (Owner is not null)
			Owner.ParentChanged += OnOwnerParentChanged;
	}

	protected void AlertComponentChanged()
		=> ComponentChanged?.Invoke( this );

	internal void Dispose()
		=> OnDispose();

	protected virtual void OnDispose() { }
	protected virtual void OnOwnerChanged() { }
	protected virtual void OnOwnerParentChanged(Entity owner, Entity? newParent) { }
}
