using Engine.Logging;
using Engine.Module.Entities.Container;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Module.Entities.Render;

public sealed class RenderEntityContainer : DisposableIdentifiable, IUpdateable {
	private readonly EntityContainer _container;
	private readonly EntityContainerListChangeEventHandler _handler;

	private readonly Dictionary<Guid, RenderEntity> _renderEntitiesByEntityId;

	private readonly ConcurrentQueue<Entity> _renderEntitiesToAddQueue;
	private readonly ConcurrentQueue<Entity> _renderEntitiesToRemoveQueue;

	private readonly RenderEntityContainerDependentBehaviourManager _renderEntityContainerDependentBehaviourManager;

	public event Action<RenderEntity>? OnRenderEntityAdded;
	public event Action<RenderEntity>? OnRenderEntityRemoved;

	public RenderEntityContainer( EntityContainer container ) {
		this._container = container;
		this._renderEntityContainerDependentBehaviourManager = new(this);
		this._renderEntitiesByEntityId = [];
		this._renderEntitiesToAddQueue = [];
		this._renderEntitiesToRemoveQueue = [];
		this._handler = container.CreateListChangeHandler( OnEntityAdded, OnEntityRemoved );
		this._container.ArchetypeManager.ArchetypeAdded += this._renderEntityContainerDependentBehaviourManager.OnArchetypeAdded;
		this._container.ArchetypeManager.ArchetypeRemoved += this._renderEntityContainerDependentBehaviourManager.OnArchetypeRemoved;
	}

	public int PendingEntitiesToAdd => this._renderEntitiesToAddQueue.Count;
	public int PendingEntitiesToRemove => this._renderEntitiesToRemoveQueue.Count;

	public IReadOnlyCollection<RenderEntity> RenderEntities => this._renderEntitiesByEntityId.Values;

	public bool TryGetRenderEntity( Guid entityId, [NotNullWhen( true )] out RenderEntity? renderEntity ) => this._renderEntitiesByEntityId.TryGetValue( entityId, out renderEntity );

	//Called on game logic thread
	private void OnEntityAdded( Entity entity ) {
		entity.ComponentAdded += OnComponentAdded;
		entity.ComponentRemoved += OnComponentRemoved;
		if (entity.HasComponent<RenderComponent>())
			this._renderEntitiesToAddQueue.Enqueue( entity );
	}

	//Called on game logic thread
	private void OnEntityRemoved( Entity entity ) {
		entity.ComponentAdded -= OnComponentAdded;
		entity.ComponentRemoved -= OnComponentRemoved;
		if (entity.HasComponent<RenderComponent>())
			this._renderEntitiesToRemoveQueue.Enqueue( entity );
	}

	//Called on game logic thread
	private void OnComponentAdded( ComponentBase component ) {
		if (component is not RenderComponent)
			return;
		this._renderEntitiesToAddQueue.Enqueue( component.Entity );
	}

	//Called on game logic thread
	private void OnComponentRemoved( ComponentBase component ) {
		if (component is not RenderComponent)
			return;
		this._renderEntitiesToRemoveQueue.Enqueue( component.Entity );
	}

	public void Update( double time, double deltaTime ) {
		//TODO test creating an entity here. If the entity exists paint a triangle based on it's attributes.
		while (this._renderEntitiesToAddQueue.TryDequeue( out Entity? entity ))
			AddRenderEntity( entity );
		while (this._renderEntitiesToRemoveQueue.TryDequeue( out Entity? entity ))
			RemoveRenderEntity( entity );
		this._renderEntityContainerDependentBehaviourManager.Update( time, deltaTime );
		foreach (RenderEntity renderEntity in this._renderEntitiesByEntityId.Values)
			renderEntity.Update( time, deltaTime );
	}

	private void AddRenderEntity( Entity entity ) {
		if (this._renderEntitiesByEntityId.ContainsKey( entity.EntityId ))
			return;
		RenderEntity renderEntity = new( entity );
		this._renderEntitiesByEntityId.Add( entity.EntityId, renderEntity );
		foreach (ArchetypeBase archetype in entity.CurrentArchetypes)
			renderEntity.AddDependenciesOnArchetype( archetype );
		OnRenderEntityAdded?.Invoke( renderEntity );
	}

	private void RemoveRenderEntity( Entity e ) {
		if (!this._renderEntitiesByEntityId.Remove( e.EntityId, out RenderEntity? renderEntity ))
			return;
		renderEntity.Dispose();
		OnRenderEntityRemoved?.Invoke( renderEntity );
	}

	protected override bool InternalDispose() {
		this._container.RemoveHandler( this._handler );
		this._container.ArchetypeManager.ArchetypeAdded -= this._renderEntityContainerDependentBehaviourManager.OnArchetypeAdded;
		this._container.ArchetypeManager.ArchetypeRemoved -= this._renderEntityContainerDependentBehaviourManager.OnArchetypeRemoved;
		foreach (RenderEntity renderEntity in this._renderEntitiesByEntityId.Values)
			renderEntity.Dispose();
		this._renderEntitiesByEntityId.Clear();
		return true;
	}
}
