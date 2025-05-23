﻿using Engine.Serialization;
using System.Collections.Concurrent;

namespace Engine.Module.Entities.Container;

/// <summary>
/// Should be disposed by the owner of the copy.
/// </summary>
public sealed class SynchronizedEntityContainer : DisposableIdentifiable {
	private readonly EntityContainer _originalContainer;
	private readonly SerializerProvider _originalSerializerProvider;

	private readonly Dictionary<Guid, SynchronizedEntity> _synchronizedEntities;

	private readonly ConcurrentQueue<SynchronizedEntity> _incomingEntities;
	private readonly ConcurrentQueue<Guid> _outgoingEntities;

	public event EntityListChangedHandler? EntityAdded;
	public event EntityListChangedHandler? EntityRemoved;
	public event EntityArchetypeChangeHandler? EntityArchetypeAdded;
	public event EntityArchetypeChangeHandler? EntityArchetypeRemoved;

	public SynchronizedEntityContainer( EntityContainer originalContainer, SerializerProvider originalSerializerProvider ) {
		_originalContainer = originalContainer;
		_originalContainer.OnEntityAdded += OnEntityAdded;
		_originalContainer.OnEntityRemoved += OnEntityRemoved;
		_originalSerializerProvider = originalSerializerProvider;
		_synchronizedEntities = [];
		_incomingEntities = [];
		_outgoingEntities = [];

		foreach (Entity entity in originalContainer.Entities)
			OnEntityAdded( entity );
	}

	public Guid ContainerId => _originalContainer.Id;

	public IReadOnlyCollection<SynchronizedEntity> SynchronizedEntities => _synchronizedEntities.Values;

	public int IncomingEntities => _incomingEntities.Count;
	public int OutgoingEntities => _outgoingEntities.Count;

	//Called from other thread
	public void Update( SerializerProvider serializerProvider ) {
		if (Disposed)
			return;
		while (_incomingEntities.TryDequeue( out SynchronizedEntity? synchronizedEntity )) {
			synchronizedEntity.Initialize( serializerProvider, ParentIdChanged );
			ParentIdChanged( synchronizedEntity.EntityCopy! );
			synchronizedEntity.EntityCopy!.ArchetypeAdded += OnArchetypeAdded;
			synchronizedEntity.EntityCopy!.ArchetypeRemoved += OnArchetypeRemoved;
			_synchronizedEntities.Add( synchronizedEntity.EntityId, synchronizedEntity );
			EntityAdded?.Invoke( synchronizedEntity.EntityCopy! );
		}

		while (_outgoingEntities.TryDequeue( out Guid entityId )) {
			_synchronizedEntities.Remove( entityId, out SynchronizedEntity? removed );
			removed?.Dispose();
			EntityRemoved?.Invoke( removed?.EntityCopy! );
		}

		foreach (SynchronizedEntity synchronizedEntity in _synchronizedEntities.Values) {
			synchronizedEntity.Update();
		}
	}

	private void ParentIdChanged( Entity entity ) => entity.SetParentInternal( entity.ParentId.HasValue && this._synchronizedEntities.TryGetValue( entity.ParentId.Value, out SynchronizedEntity? parentSynchronizedEntity ) ? parentSynchronizedEntity.EntityCopy : null );
	private void OnArchetypeAdded( ArchetypeBase archetype ) => EntityArchetypeAdded?.Invoke( archetype );
	private void OnArchetypeRemoved( ArchetypeBase archetype ) => EntityArchetypeRemoved?.Invoke( archetype );

	//Called from original thread
	private void OnEntityAdded( Entity entity ) {
		if (Disposed)
			return;
		SynchronizedEntity synchronizedEntity = new( new EntitySerializerPair( entity, _originalSerializerProvider ) );
		synchronizedEntity.Synchronize();
		_incomingEntities.Enqueue( synchronizedEntity );
	}

	//Called from original thread
	private void OnEntityRemoved( Entity entity ) {
		if (Disposed)
			return;
		_outgoingEntities.Enqueue( entity.EntityId );
	}

	protected override bool InternalDispose() {
		_originalContainer.OnEntityAdded -= OnEntityAdded;
		_originalContainer.OnEntityRemoved -= OnEntityRemoved;
		foreach (SynchronizedEntity synchronizedEntity in _synchronizedEntities.Values)
			synchronizedEntity.Dispose();
		return true;
	}
}