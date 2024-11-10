namespace Engine.Module.Entities.Render;
public sealed class RenderEntity : DisposableIdentifiable {
	private readonly Entity _entity;

	internal RenderEntity( Entity entity ) {
		this._entity = entity;
	}

	protected override bool InternalDispose() {
		throw new NotImplementedException();
	}
}

public abstract class RenderBehaviourBase : DisposableIdentifiable, IUpdateable {
	public abstract void Update( double time, double deltaTime );
}

/// <summary>
/// Allows for synchronization of component data when the base component is changed.
/// </summary>
/// <typeparam name="TComponent"></typeparam>
public abstract class SynchronizedRenderBehaviourBase<TComponent> : RenderBehaviourBase where TComponent : ComponentBase {
	protected readonly TComponent Component;
	private bool _synchronized;

	protected SynchronizedRenderBehaviourBase( TComponent component ) {
		this.Component = component;
		this.Component.ComponentChanged += this.InternalOnComponentChanged;
	}

	private void InternalOnComponentChanged( ComponentBase component ) {
		if (PrepareSynchronization())
			_synchronized = false;
	}

	public override void Update( double time, double deltaTime ) {
		if (_synchronized)
			return;
		Synchronize();
		_synchronized = true;
	}
	protected override bool InternalDispose() {
		this.Component.ComponentChanged -= this.InternalOnComponentChanged;
		return true;
	}

	/// <summary>
	/// Happens on the logic thread. No render logic should take place here, this step is for preparing the synchronization. An example would be copying over a transformation matrix to an intermediary buffer, so that the render thread can copy it over to be used. The reason for this complexity is because of the multithreaded nature of the engine. If you read data while it's being written to you can get corrupted data.
	/// </summary>
	/// <returns>True if synchronization is needed, false if not.</returns>
	protected abstract bool PrepareSynchronization();
	/// <summary>
	/// Happens on the render thread. This happens after the <see cref="PrepareSynchronization"/> step. This only runs when synchronization is needed.
	/// </summary>
	protected abstract void Synchronize();

}