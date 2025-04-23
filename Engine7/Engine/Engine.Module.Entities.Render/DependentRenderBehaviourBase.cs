using Engine.Module.Entities.Container;

namespace Engine.Module.Render.Entities;

public abstract class DependentRenderBehaviourBase<TArchetype> : RenderBehaviourBase where TArchetype : ArchetypeBase {

	private RenderArchetype<TArchetype>? _renderArchetype;
	protected RenderArchetype<TArchetype> RenderArchetype => this._renderArchetype ?? throw new InvalidOperationException( "RenderArchetype is not set." );
	private TArchetype? _archetype;
	protected TArchetype Archetype => this._archetype ?? throw new InvalidOperationException( "Archetype is not set." );

	internal void SetArchetype( TArchetype archetype ) {
		if (this._renderArchetype is not null)
			throw new InvalidOperationException( "Archetype can't be changed." );
		this._renderArchetype = new(archetype);

		OnArchetypeSet();
	}

	/// <summary>
	/// RenderEntity is not set yet during this call.
	/// </summary>
	protected virtual void OnArchetypeSet() { }
}

public sealed class RenderArchetype<TArchetype> where TArchetype : ArchetypeBase {
	public RenderArchetype( TArchetype archetype ) {
		
	}
}

public sealed class RenderComponent<T> where T : ComponentBase, ISerializableComponent {

}