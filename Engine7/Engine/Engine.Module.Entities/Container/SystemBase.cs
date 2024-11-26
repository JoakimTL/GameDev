namespace Engine.Module.Entities.Container;

public abstract class SystemBase<TArchetype> : IUpdateable where TArchetype : ArchetypeBase {
#pragma warning disable IDE0044 // Add readonly modifier This is set by the SystemTypeManager system factory
	private EntityContainer _container = null!;
#pragma warning restore IDE0044 // Add readonly modifier
	protected EntityContainer Container => this._container;
	protected readonly Type _archetype = typeof( TArchetype );

	public void Update( double time, double deltaTime ) {
		foreach (TArchetype archetype in this._container.ArchetypeManager.GetArchetypes<TArchetype>())
			ProcessEntity( archetype, time, deltaTime );
	}

	protected abstract void ProcessEntity( TArchetype archetype, double time, double deltaTime );
}
