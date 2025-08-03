using Engine.Buffers;
using Engine.Serialization;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Engine.Module.Entities.Container;

//Listens to changes in the entity and reflects them onto itself. It is essentially a copy of the entity, but with no logic. This is to allow the render entity and its behaviours to determine the behaviour, but with the newest data from the entity at this frame.
public sealed class SynchronizedEntity : DisposableIdentifiable {

	private bool _rewritingParentId = false;
	private Guid? _newParent;
	private readonly EntitySerializerPair _originalPair;
	private EntitySerializerPair? _copyPair;

	private PooledBufferData? _synchronizationData;
	private readonly ConcurrentQueue<PooledBufferData> _componentSerializationResults;
	private readonly ConcurrentQueue<Type> _removedComponents;
	private readonly Dictionary<Type, PooledBufferData> _failedDeserializations;

	public SynchronizedEntity( EntitySerializerPair original ) {
		_originalPair = original;
		_synchronizationData = null;
		_removedComponents = [];
		_componentSerializationResults = [];
		_failedDeserializations = [];
	}

	public Guid EntityId => _originalPair.Entity.EntityId;
	public Entity? EntityCopy => _copyPair?.Entity;

	//Called from other thread (render thread)
	public void Initialize( SerializerProvider serializerProvider, ParentIdChanged parentIdChangedDelegate ) {
		_copyPair = new( new( _originalPair.Entity.EntityId, parentIdChangedDelegate ), serializerProvider );
		_originalPair.Entity.ParentChanged += OnParentChanged;
		_newParent = _originalPair.Entity.ParentId;
	}

	//Called from other thread (render thread)
	public void Update() {
		if (_copyPair is null || Disposed)
			return;
		if (_synchronizationData is not null) {
			ISerializer? serializer = _copyPair.SerializerProvider.GetSerializerFor<Entity>() ?? throw new InvalidOperationException( "Serializer for Entity not found." );
			if (serializer.CanDeserialize( _synchronizationData.Payload.Span )) {
				serializer.DeserializeInto( _synchronizationData.Payload.Span, _copyPair.Entity );
				_synchronizationData.Dispose();
				_synchronizationData = null;
			}
		}
		//Process failed deserializations, this happens before processing the newest changes in case any of the failed deserializations are the same type as the newest changes.
		while (_failedDeserializations.Count > 0) {
			var failedComponentSerializationData = _failedDeserializations.First();
			_failedDeserializations.Remove( failedComponentSerializationData.Key );
			DeserializeComponentData( failedComponentSerializationData.Value );
		}
		while (_componentSerializationResults.TryDequeue( out PooledBufferData? componentSerializationData )) {
			DeserializeComponentData( componentSerializationData );
		}
		while (_removedComponents.TryDequeue( out Type? removedComponentType )) {
			_copyPair.Entity.RemoveComponent( removedComponentType );
			//If this component had a failed deserialization, let's remove it too to avoid a memory leak.
			if (_failedDeserializations.Remove( removedComponentType, out PooledBufferData? data ))
				data.Dispose();
		}
		if (!_rewritingParentId && _newParent != _copyPair.Entity.ParentId)
			_copyPair.Entity.SetParent( _newParent );
	}

	//Called from other thread (render thread)
	private void DeserializeComponentData( PooledBufferData data ) {
		ISerializer? serializer = _copyPair!.SerializerProvider.GetSerializerFor( MemoryMarshal.Read<Guid>( data.Payload.Span[ ^16.. ] ) );
		if (serializer is null) {
			data.Dispose();//There is no serializer for these changes, discard!
			return;
		}
		if (!serializer.CanDeserialize( data.Payload.Span )) {
			//We can't deserialize the component, let's overwrite the existing failed deserialization if it exists. We assume this is the newest change.
			_failedDeserializations[ serializer.Target ] = data;
			return;
		}
		//At this point we know the data is deserializable, so let's deserialize into the existing component, or create a new component instance if it doesn't already exist.
		if (!_copyPair.Entity.TryGetComponent( serializer.Target, out ComponentBase? component )) {
			component = _copyPair.Entity.CreateComponent( serializer.Target );
			serializer.DeserializeInto( data.Payload.Span, component );
			_copyPair.Entity.AddComponent( serializer.Target, component );
		} else {
			serializer.DeserializeInto( data.Payload.Span, component );
		}
		data.Dispose();
	}

	//Called from Entity
	internal void Synchronize() {
		ISerializer serializer = _originalPair.SerializerProvider.GetSerializerFor<Entity>() ?? throw new InvalidOperationException( "Serializer for Entity not found." );
		_originalPair.Entity.ComponentAdded += OnComponentAdded;
		_originalPair.Entity.ComponentChanged += OnComponentChanged;
		_originalPair.Entity.ComponentRemoved += OnComponentRemoved;
		ThreadedByteBuffer buffer = ThreadedByteBuffer.GetBuffer( "ecs-sync" );
		serializer.SerializeInto( buffer, _originalPair.Entity );
		_synchronizationData = buffer.GetData( tag: $"sync-data-{_originalPair.Entity}" );
	}

	//Called from Entity
	private void OnComponentAdded( Entity entity, ComponentBase component ) => OnComponentChanged( component );

	//Called from Entity
	private void OnComponentChanged( ComponentBase component ) {
		ISerializer? serializer = _originalPair.SerializerProvider.GetSerializerFor( component.GetType() );
		if (serializer is null)
			return;
		ThreadedByteBuffer buffer = ThreadedByteBuffer.GetBuffer( "ecs-sync" );
		serializer.SerializeInto( buffer, component );
		_componentSerializationResults.Enqueue( buffer.GetData( tag: _originalPair.Entity ) );
	}

	//Called from Entity
	private void OnComponentRemoved( Entity entity, ComponentBase component ) => _removedComponents.Enqueue( component.GetType() );

	//Called from Entity
	private void OnParentChanged( Entity entity, Entity? oldParent, Entity? newParent ) {
		_rewritingParentId = true;
		_newParent = newParent?.EntityId;
		_rewritingParentId = false;
	}

	protected override bool InternalDispose() {
		_originalPair.Entity.ComponentAdded -= OnComponentAdded;
		_originalPair.Entity.ComponentChanged -= OnComponentChanged;
		_originalPair.Entity.ComponentRemoved -= OnComponentRemoved;
		_synchronizationData?.Dispose();
		_synchronizationData = null;
		while (_componentSerializationResults.TryDequeue( out PooledBufferData? componentSerializationData ))
			componentSerializationData.Dispose();
		return true;
	}
}
