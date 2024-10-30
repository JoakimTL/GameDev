namespace Engine.Module.Entities;

public abstract class SystemBase<TArchetype> : IUpdateable where TArchetype : ArchetypeBase {
	protected readonly EntityContainer _container = null!;
	protected readonly Type _archetype = typeof( TArchetype );

	public void Update( double time, double deltaTime ) {
		foreach (TArchetype archetype in this._container.ArchetypeManager.GetArchetypes<TArchetype>())
			ProcessEntity( archetype );
	}

	protected abstract void ProcessEntity( TArchetype archetype );
}
