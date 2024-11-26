using Engine.Module.Entities.Container;

namespace Engine.Module.Entities.Render;

public abstract class DependentRenderBehaviourBase<TArchetype> : RenderBehaviourBase where TArchetype : ArchetypeBase {

	private TArchetype? _archetype;
	protected TArchetype Archetype => this._archetype ?? throw new InvalidOperationException( "Archetype is not set." );

	internal void SetArchetype( TArchetype archetype ) {
		if (this._archetype is not null)
			throw new InvalidOperationException( "Archetype can't be changed." );
		this._archetype = archetype;
		OnArchetypeSet();
	}

	protected abstract void OnArchetypeSet();
}
