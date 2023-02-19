namespace Engine.GameLogic.ECS;
public abstract class ComponentBase : Identifiable {
	public Entity? Owner { get; private set; }
	public event EntityComponentEvent? ComponentChanged;

	internal void SetOwner( Entity? e ) {
		Owner = e;
		OnOwnerChanged();
	}

	protected void AlertComponentChanged()
		=> ComponentChanged?.Invoke( this );

	internal void Dispose()
		=> OnDispose();

	protected virtual void OnDispose() { }
	protected virtual void OnOwnerChanged() { }
}
