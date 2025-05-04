using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Messages;
using Engine.Serialization;
using System.Collections.Concurrent;

namespace Engine.Module.Entities.Services;

public sealed class SynchronizedEntityContainerService : DisposableIdentifiable, IUpdateable {

	private readonly SerializerProvider _serializerProvider;
	private readonly MessageBusNode _messageBusNode;
	private readonly List<SynchronizedEntityContainer> _synchronizedContainers;
	private readonly ConcurrentQueue<SynchronizedEntityContainer> _incomingContainers;
	private readonly ConcurrentQueue<SynchronizedEntityContainer> _outgoingContainers;

	/// <summary>
	/// Happens on local thread
	/// </summary>
	public event Action<SynchronizedEntityContainer>? SynchronizedEntityContainerAdded;
	/// <summary>
	/// Happens on local thread
	/// </summary>
	public event Action<SynchronizedEntityContainer>? SynchronizedEntityContainerRemoved;

	public SynchronizedEntityContainerService( SerializerProvider serializerProvider ) {
		this._serializerProvider = serializerProvider;
		this._messageBusNode = MessageBus.CreateNode( "ecs-sync" );
		this._messageBusNode.OnMessageProcessed += OnMessageReceived;
		this._synchronizedContainers = [];
		this._incomingContainers = [];
		this._outgoingContainers = [];
		this._messageBusNode.Publish( new SynchronizedEntityContainerListRequestMessage(), null, true );
	}

	public IReadOnlyList<SynchronizedEntityContainer> SynchronizedContainers => this._synchronizedContainers.AsReadOnly();

	private void OnMessageReceived( Message message ) {
		if (message.Content is SynchronizedEntityContainerRequestMessageResponse synchronizedEntityContainerRequestResponse)
			this._incomingContainers.Enqueue( synchronizedEntityContainerRequestResponse.SynchronizedEntityContainer );
		if (message.Sender is not null && message.Content is EntityContainerCreatedEventMessage entityContainerCreatedEvent)
			_messageBusNode.SendMessageTo( message.Sender, new SynchronizedEntityContainerRequestMessage( entityContainerCreatedEvent.ContainerId ) );
	}

	public void Update( double time, double deltaTime ) {
		this._messageBusNode.ProcessQueue();
		while (this._outgoingContainers.TryDequeue( out SynchronizedEntityContainer? container )) {
			this._synchronizedContainers.Remove( container );
			container.OnDisposed -= OnContainerDisposed;
			this.SynchronizedEntityContainerRemoved?.Invoke( container );
		}
		while (this._incomingContainers.TryDequeue( out SynchronizedEntityContainer? container )) {
			this._synchronizedContainers.Add( container );
			container.OnDisposed += OnContainerDisposed;
			this.SynchronizedEntityContainerAdded?.Invoke( container );
		}
		foreach (SynchronizedEntityContainer container in this._synchronizedContainers)
			container.Update( _serializerProvider );
	}

	private void OnContainerDisposed( IListenableDisposable disposable ) {
		if (disposable is not SynchronizedEntityContainer container)
			return;
		this._synchronizedContainers.Remove( container );
		this._outgoingContainers.Enqueue( container );
	}

	protected override bool InternalDispose() {
		foreach (SynchronizedEntityContainer container in this._synchronizedContainers) {
			container.OnDisposed -= OnContainerDisposed;
			container.Dispose();
		}
		return true;
	}
}