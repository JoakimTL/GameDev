namespace Engine.Module.Entities;

public abstract class SystemBase : IUpdateable {

	protected readonly EntityContainer _container;
	protected readonly Type _archetype;

	internal SystemBase( EntityContainer container, Type archetype ) {
		if (!archetype.IsClass || archetype.IsAbstract)
			throw new ArgumentException( "Archetype must be a non-abstract class." );
		this._container = container;
		this._archetype = archetype;
	}

	public abstract void Update( double time, double deltaTime );
}

public abstract class SystemBase<TArchetype>( EntityContainer container ) : SystemBase( container, typeof( TArchetype ) ) where TArchetype : ArchetypeBase {
	public override void Update( double time, double deltaTime ) {
		foreach (TArchetype archetype in _container.ArchetypeManager.GetArchetypes<TArchetype>())
			ProcessEntity( archetype );
	}

	protected abstract void ProcessEntity( TArchetype archetype );
}
