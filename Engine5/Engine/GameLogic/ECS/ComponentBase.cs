namespace Engine.GameLogic.ECS;
public abstract class ComponentBase : Identifiable {
	public Entity? Owner { get; internal set; }
	public event EntityComponentEvent? ComponentChanged;

	protected void AlertComponentChanged()
		=> ComponentChanged?.Invoke( this );

	internal void Dispose() {
		OnDispose();
	}

	protected virtual void OnDispose() { }
}
