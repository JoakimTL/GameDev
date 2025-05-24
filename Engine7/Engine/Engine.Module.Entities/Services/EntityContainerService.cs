using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Messages;
using Engine.Serialization;
using System.Linq;

namespace Engine.Module.Entities.Services;
public sealed class EntityContainerService : DisposableIdentifiable, IUpdateable {

	private readonly List<EntityContainer> _containers;
	private readonly MessageBusNode _messageBusNode;
	private readonly SerializerProvider _serializerProvider;

	public EntityContainerService( SerializerProvider serializerProvider ) {
		this._containers = [];
		this._messageBusNode = MessageBus.CreateNode( "ecs" );
		this._messageBusNode.OnMessageProcessed += OnMessageReceived;
		this._serializerProvider = serializerProvider;
	}

	private void OnMessageReceived( Message message ) {
		if (message.Content is SynchronizedEntityContainerListRequestMessage containerListRequest)
			foreach (EntityContainer container in this._containers)
				message.SendResponseFrom( this._messageBusNode, new SynchronizedEntityContainerRequestMessageResponse( new( container, _serializerProvider ) ) );
		if (message.Content is SynchronizedEntityContainerRequestMessage containerRequest) {
			EntityContainer? container = this._containers.FirstOrDefault( p => p.Id == containerRequest.ContainerId );
			if (container is not null)
				message.SendResponseFrom( this._messageBusNode, new SynchronizedEntityContainerRequestMessageResponse( new( container, _serializerProvider ) ) );
		}
	}

	public EntityContainer CreateContainer() {
		EntityContainer container = new();
		this._containers.Add( container );
		container.OnDisposed += OnContainerDisposed;
		this._messageBusNode.Publish( new EntityContainerCreatedEventMessage( container.Id ), null, true );
		return container;
	}

	private void OnContainerDisposed( IListenableDisposable disposable ) {
		if (disposable is not EntityContainer container)
			return;
		this._containers.Remove( container );
		this._messageBusNode.Publish( new EntityContainerRemovedEventMessage( container.Id ), null, true );
	}

	protected override bool InternalDispose() {
		foreach (EntityContainer container in this._containers) {
			container.OnDisposed -= OnContainerDisposed;
			container.Dispose();
		}
		return true;
	}

	public void Update( double time, double deltaTime ) {
		this._messageBusNode.ProcessQueue();
		foreach (EntityContainer container in this._containers)
			container.SystemManager.Update( time, deltaTime );
	}
}
