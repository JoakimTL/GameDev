﻿using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render.Components;
using Engine.Module.Entities.Services;
using Engine.Processing;
using Engine.Serialization;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Module.Render.Entities;

[Do<IUpdateable>.After<SynchronizedEntityContainerService>]
public sealed class RenderEntityContainer : DisposableIdentifiable, IUpdateable {
	private readonly RenderEntityServiceAccess _serviceAccess;

	private readonly Dictionary<Guid, RenderEntity> _renderEntitiesByEntityId;

	private readonly ConcurrentQueue<Entity> _renderEntitiesToAddQueue;
	private readonly ConcurrentQueue<Entity> _renderEntitiesToRemoveQueue;

	private readonly RenderEntityContainerDependentBehaviourManager _renderEntityContainerDependentBehaviourManager;

	public event Action<RenderEntity>? OnRenderEntityAdded;
	public event Action<RenderEntity>? OnRenderEntityRemoved;

	public RenderEntityContainer( SynchronizedEntityContainer synchronizedContainer, RenderEntityServiceAccess serviceAccess ) {
		this.SynchronizedEntityContainer = synchronizedContainer;
		this._serviceAccess = serviceAccess;
		this._renderEntityContainerDependentBehaviourManager = new( this );
		this._renderEntitiesByEntityId = [];
		this._renderEntitiesToAddQueue = [];
		this._renderEntitiesToRemoveQueue = [];
		this.SynchronizedEntityContainer.EntityAdded += OnEntityAdded;
		this.SynchronizedEntityContainer.EntityRemoved += OnEntityRemoved;
	}

	public SynchronizedEntityContainer SynchronizedEntityContainer { get; }

	public int PendingEntitiesToAdd => this._renderEntitiesToAddQueue.Count;
	public int PendingEntitiesToRemove => this._renderEntitiesToRemoveQueue.Count;

	public IReadOnlyCollection<RenderEntity> RenderEntities => this._renderEntitiesByEntityId.Values;

	public bool TryGetRenderEntity( Guid entityId, [NotNullWhen( true )] out RenderEntity? renderEntity ) => this._renderEntitiesByEntityId.TryGetValue( entityId, out renderEntity );

	private void OnEntityAdded( Entity entity ) {
		if (entity.HasComponent<RenderComponent>())
			this._renderEntitiesToAddQueue.Enqueue( entity );
		entity.ComponentAdded += OnComponentAdded;
		entity.ComponentRemoved += OnComponentRemoved;
	}

	private void OnEntityRemoved( Entity entity ) {
		if (entity.HasComponent<RenderComponent>())
			this._renderEntitiesToRemoveQueue.Enqueue( entity );
		entity.ComponentAdded -= OnComponentAdded;
		entity.ComponentRemoved -= OnComponentRemoved;
	}

	private void OnComponentAdded( Entity entity, ComponentBase component ) {
		if (component is not RenderComponent)
			return;
		this._renderEntitiesToAddQueue.Enqueue( entity );
	}

	private void OnComponentRemoved( Entity entity, ComponentBase component ) {
		if (component is not RenderComponent)
			return;
		this._renderEntitiesToRemoveQueue.Enqueue( entity );
	}

	public void Update( double time, double deltaTime ) {
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
		RenderEntity renderEntity = new( entity, this._serviceAccess );
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
		_renderEntityContainerDependentBehaviourManager.Dispose();
		foreach (RenderEntity renderEntity in this._renderEntitiesByEntityId.Values)
			renderEntity.Dispose();
		return true;
	}
}
