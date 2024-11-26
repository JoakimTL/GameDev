using Engine.Module.Entities.Container;

namespace Engine.Module.Entities.Render;

/// <summary>
/// Allows for synchronization of archetype data when the consitiuent components are changed.
/// </summary>
/// <typeparam name="TArchetype"></typeparam>
public abstract class SynchronizedRenderBehaviourBase<TArchetype> : DependentRenderBehaviourBase<TArchetype> where TArchetype : ArchetypeBase {

	private bool _synchronized = false;

	protected override void OnArchetypeSet() {
		this.Archetype.SubscribeToComponentChanges( InternalOnComponentChanged );
		Initialize();
	}

	private void InternalOnComponentChanged( ComponentBase component ) {
		if (PrepareSynchronization( component ))
			this._synchronized = false;
	}

	public override void Update( double time, double deltaTime ) {
		if (this._synchronized)
			return;
		this._synchronized = true;
		Synchronize();
	}

	protected override bool InternalDispose() {
		this.Archetype.UnsubscribeFromComponentChanges( InternalOnComponentChanged );
		return true;
	}

	protected abstract void Initialize();
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