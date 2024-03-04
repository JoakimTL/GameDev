namespace Engine.Modules.ECS;

public abstract class EntityContainerBase : IDisposable {

	private readonly EntityManager _entityManager;

	protected EntityContainerBase( EntityManager entityManager ) {
		this._entityManager = entityManager;
		_entityManager.ComponentAdded += ComponentAdded;
		_entityManager.ComponentRemoved += ComponentRemoved;
	}

	public abstract void Dispose();
	protected abstract void ComponentAdded( Entity e, ComponentBase component );
	protected abstract void ComponentRemoved( Entity e, ComponentBase component );
	protected internal abstract void AddAll( IEnumerable<Entity> allEntities );


}

public abstract class EntityContainerBase<T> : EntityContainerBase {
	protected EntityContainerBase( EntityManager entityManager ) : base( entityManager ) { }

	public abstract IEnumerable<Entity> GetEntities( T t );
}