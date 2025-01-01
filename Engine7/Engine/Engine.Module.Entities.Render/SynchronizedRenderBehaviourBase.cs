using Engine.Module.Entities.Container;

namespace Engine.Module.Render.Entities;

/// <summary>
/// Allows for synchronization of archetype data when the consitiuent components are changed.
/// </summary>
/// <typeparam name="TArchetype"></typeparam>
public abstract class SynchronizedRenderBehaviourBase<TArchetype> : DependentRenderBehaviourBase<TArchetype> where TArchetype : ArchetypeBase {

	private bool _synchronized = false;

	public event Action<RenderBehaviourBase>? OnSynchronized;

	protected override void OnArchetypeSet() {
		base.OnArchetypeSet();
		this.Archetype.SubscribeToComponentChanges( InternalOnComponentChanged );
		this._synchronized = false;
	}

	private void InternalOnComponentChanged( ComponentBase component ) {
		if (PrepareSynchronization( component ))
			this._synchronized = false;
	}

	public override void Update( double time, double deltaTime ) {
		if (!this._synchronized) {
			this._synchronized = true;
			Synchronize();
			OnSynchronized?.Invoke( this );
		}
		OnUpdate( time, deltaTime );
	}

	protected void ForceSynchronization() {
		this._synchronized = false;
	}

	protected abstract void OnUpdate( double time, double deltaTime );

	protected override bool InternalDispose() {
		this.Archetype.UnsubscribeFromComponentChanges( InternalOnComponentChanged );
		return true;
	}

	/// <summary>
	/// Happens on the logic thread. No render logic should take place here, this step is for preparing the synchronization. An example would be copying over a transformation matrix to an intermediary buffer, so that the render thread can copy it over to be used. The reason for this complexity is because of the multithreaded nature of the engine. If you read data while it's being written to you can get corrupted data.
	/// </summary>
	/// <returns>True if synchronization is needed, false if not.</returns>
	protected abstract bool PrepareSynchronization( ComponentBase component );
	/// <summary>
	/// Happens on the render thread. This happens after the <see cref="PrepareSynchronization"/> step. This only runs when synchronization is needed.
	/// </summary>
	protected abstract void Synchronize();
}