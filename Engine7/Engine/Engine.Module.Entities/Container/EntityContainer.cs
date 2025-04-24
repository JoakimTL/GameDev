using System.Collections.Concurrent;

namespace Engine.Module.Entities.Container;

public sealed class EntityContainer : DisposableIdentifiable {

	public const int MAX_GUID_ATTEMPTS = 16777216;
	private readonly Dictionary<Guid, Entity> _entitiesById;

	public event EntityListChangedHandler? OnEntityAdded;
	public event EntityListChangedHandler? OnEntityRemoved;

	public EntityContainer() {
		this._entitiesById = [];
		this.ArchetypeManager = new( this );
		this.SystemManager = new( this );
	}

	internal IEnumerable<Entity> Entities => this._entitiesById.Values;

	public EntityContainerArchetypeManager ArchetypeManager { get; }
	public EntityContainerSystemManager SystemManager { get; }

	public Entity CreateEntity() {
		ObjectDisposedException.ThrowIf( Disposed, nameof( EntityContainer ) );
		Guid guid = FindFreeGuid();
		Entity entity = new( guid, ParentIdChanged );
		this._entitiesById.Add( guid, entity );
		OnEntityAdded?.Invoke( entity );
		return entity;
	}

	public void RemoveEntity( Entity entity ) {
		ObjectDisposedException.ThrowIf( Disposed, nameof( EntityContainer ) );
		this._entitiesById.Remove( entity.EntityId );
		foreach (Entity e in this._entitiesById.Values) {
			if (e.Parent != entity)
				continue;
			if (e.HasComponent<InheritsParentsParentWhenParentIsRemovedComponent>())
				e.SetParent( entity.ParentId );
			else
				e.SetParent( null );
		}
		OnEntityRemoved?.Invoke( entity );
	}

	public void RemoveEntity( Guid entityId ) {
		ObjectDisposedException.ThrowIf( Disposed, nameof( EntityContainer ) );
		if (this._entitiesById.TryGetValue( entityId, out Entity? entity ))
			RemoveEntity( entity );
	}

	private Guid FindFreeGuid() {
		int attemptNumber = 0;
		do {
			Guid guid = Guid.NewGuid();
			if (guid == Guid.Empty)
				continue;
			if (!this._entitiesById.ContainsKey( guid ))
				return guid;
			++attemptNumber;
			if (attemptNumber > MAX_GUID_ATTEMPTS)
				throw new InvalidOperationException( $"Unable to find a free GUID after {MAX_GUID_ATTEMPTS} attempts." );
		} while (true);
	}

	private void ParentIdChanged( Entity entity ) => entity.SetParentInternal( entity.ParentId.HasValue && this._entitiesById.TryGetValue( entity.ParentId.Value, out Entity? parentEntity ) ? parentEntity : null );

	protected override bool InternalDispose() => true;
}
