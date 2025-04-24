namespace Engine.Module.Entities.Container;

public sealed class EntityContainerListChangeEventHandler : DisposableIdentifiable {

	private readonly EntityContainer _container;
	private readonly EntityListChangedHandler _entityAddedHandler;
	private readonly EntityListChangedHandler _entityRemovedHandler;
	private readonly HashSet<Entity> _hookedEntities;

	/// <param name="entityAddedHandler">Called when an entity is added to the container. Use this method to listen for entity events, or other things related to the entity which will persist until the entity is removed and needs to be cleaned up in <see cref="entityRemovedHandler"/>.</param>
	/// <param name="entityRemovedHandler">Called when an entity is removed from the container. Use this method to clean up any resources or references to the entity that were created in <see cref="entityAddedHandler"/>."/></param>
	public EntityContainerListChangeEventHandler( EntityContainer container, EntityListChangedHandler entityAddedHandler, EntityListChangedHandler entityRemovedHandler ) {
		this._container = container;
		this._entityAddedHandler = entityAddedHandler;
		this._entityRemovedHandler = entityRemovedHandler;
		this._hookedEntities = [];
		this._container.OnEntityAdded += OnEntityAdded;
		this._container.OnEntityRemoved += OnEntityRemoved;
		Entity[] currentEntities = this._container.Entities.ToArray();
		foreach (Entity entity in currentEntities)
			OnEntityAdded( entity );
	}

	private void OnEntityAdded( Entity entity ) {
		if (!this._hookedEntities.Add( entity ))
			return;
		this._entityAddedHandler( entity );
	}

	private void OnEntityRemoved( Entity entity ) {
		if (!this._hookedEntities.Remove( entity ))
			return;
		this._entityRemovedHandler( entity );
	}

	protected override bool InternalDispose() {
		this._container.OnEntityAdded -= OnEntityAdded;
		this._container.OnEntityRemoved -= OnEntityRemoved;
		foreach (Entity entity in this._hookedEntities)
			this._entityRemovedHandler( entity );
		this._hookedEntities.Clear();
		return true;
	}
}
