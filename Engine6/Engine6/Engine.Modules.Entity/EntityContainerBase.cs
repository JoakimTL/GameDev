namespace Engine.Modules.Entity;

public abstract class EntityContainerBase {

	private readonly EntityManager _entityManager;

	protected EntityContainerBase( EntityManager entityManager ) {
		this._entityManager = entityManager;
		this._entityManager.EntityAdded += AddEntity;
		this._entityManager.EntityRemoved += RemoveEntity;
	}

	protected abstract void AddEntity( Entity entity );
	protected abstract void RemoveEntity( Entity entity );
	protected internal abstract void AddAll( IEnumerable<Entity> allEntities );

}

public abstract class EntityContainerBase<T> : EntityContainerBase {
	protected EntityContainerBase( EntityManager entityManager ) : base( entityManager ) {	}

	public abstract IEnumerable<Entity> GetEntities( T t );
}