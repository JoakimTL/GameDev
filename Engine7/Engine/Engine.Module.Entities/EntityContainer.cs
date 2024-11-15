namespace Engine.Module.Entities;

public sealed class EntityContainer : DisposableIdentifiable {

	public const int MAX_GUID_ATTEMPTS = 16777216;
	private readonly Dictionary<Guid, Entity> _entitiesById;
	private readonly HashSet<EntityContainerListChangeEventHandler> _handlers;

	public event EntityListChangedHandler? OnEntityAdded;
	public event EntityListChangedHandler? OnEntityRemoved;

	public EntityContainer() {
		this._entitiesById = [];
		this._handlers = [];
		this.ArchetypeManager = new( this );
		this.SystemManager = new( this );
	}

	internal IEnumerable<Entity> Entities => this._entitiesById.Values;

	public EntityContainerArchetypeManager ArchetypeManager { get; }
	public EntityContainerSystemManager SystemManager { get; }

	/// <summary>
	/// Adds a handler to listen for entity list changes. When created the <see cref="EntityContainerListChangeEventHandler"/> also runs the <see cref="entityAddedHandler"/> for all existing entities.
	/// </summary>
	/// <param name="entityAddedHandler">Called when an entity is added to the container. Use this method to listen for entity events, or other things related to the entity which will persist until the entity is removed and needs to be cleaned up in <see cref="entityRemovedHandler"/>.</param>
	/// <param name="entityRemovedHandler">Called when an entity is removed from the container (and for each entity when the handler is disposed). Use this method to clean up any resources or references to the entity that were created in <see cref="entityAddedHandler"/>."/></param>
	/// <returns>The event handler object. If you have no use for it anymore make sure to use <see cref="RemoveHandler(EntityContainerListChangeEventHandler)"/>.</returns>
	public EntityContainerListChangeEventHandler CreateListChangeHandler( EntityListChangedHandler entityAddedHandler, EntityListChangedHandler entityRemovedHandler ) {
		EntityContainerListChangeEventHandler handler = new( this, entityAddedHandler, entityRemovedHandler );
		this._handlers.Add( handler );
		return handler;
	}

	public void RemoveHandler( EntityContainerListChangeEventHandler handler ) {
		if (!this._handlers.Remove( handler ))
			return;
		handler.Dispose();
	}

	public Entity CreateEntity() {
		Guid guid = FindFreeGuid();
		Entity entity = new( guid, ParentIdChanged );
		this._entitiesById.Add( guid, entity );
		OnEntityAdded?.Invoke( entity );
		return entity;
	}

	public void RemoveEntity( Entity entity ) {
		this._entitiesById.Remove( entity.EntityId );
		foreach (Entity e in this._entitiesById.Values) {
			if (e.Parent != entity)
				continue;
			if (e.HasComponent<InheritsParentsParentWhenParentIsRemovedComponent>()) {
				e.SetParent( entity.ParentId );
			} else {
				e.SetParent( null );
			}
		}
		OnEntityRemoved?.Invoke( entity );
	}

	public void RemoveEntity( Guid entityId ) {
		if (this._entitiesById.TryGetValue( entityId, out Entity? entity ))
			RemoveEntity( entity );
	}

	private void OnEntityShouldBeRemoved( Entity entity ) => RemoveEntity( entity );

	private Guid FindFreeGuid() {
		int attemptNumber = 0;
		do {
			Guid guid = Guid.NewGuid();
			if (!this._entitiesById.ContainsKey( guid ))
				return guid;
			++attemptNumber;
			if (attemptNumber > MAX_GUID_ATTEMPTS)
				throw new InvalidOperationException( $"Unable to find a free GUID after {MAX_GUID_ATTEMPTS} attempts." );
		} while (true);
	}

	private void ParentIdChanged( Entity entity ) => entity.SetParentInternal( entity.ParentId.HasValue && this._entitiesById.TryGetValue( entity.ParentId.Value, out Entity? parentEntity ) ? parentEntity : null );

	protected override bool InternalDispose() {
		foreach (EntityContainerListChangeEventHandler handler in this._handlers)
			handler.Dispose();
		this._handlers.Clear();
		return true;
	}
}
