using Engine.Module.Entities.Container;

namespace Engine.Module.Render.Entities;

public abstract class DependentRenderBehaviourBase<TArchetype> : RenderBehaviourBase where TArchetype : ArchetypeBase {

	private TArchetype? _archetype;
	protected TArchetype Archetype => this._archetype ?? throw new InvalidOperationException( "Archetype is not set." );

	internal void SetArchetype( TArchetype archetype ) {
		if (this._archetype is not null)
			throw new InvalidOperationException( "Archetype can't be changed." );
		this._archetype = archetype;

		OnArchetypeSet();
	}

	/// <summary>
	/// RenderEntity is not set yet during this call.
	/// </summary>
	protected virtual void OnArchetypeSet() { }
}