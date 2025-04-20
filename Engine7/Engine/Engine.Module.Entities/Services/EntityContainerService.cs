using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Messages;

namespace Engine.Module.Entities.Services;
public sealed class EntityContainerService : DisposableIdentifiable, IUpdateable {

	private readonly List<EntityContainer> _containers;
	private readonly MessageBusNode _messageBusStop;

	public EntityContainerService() {
		this._containers = [];
		this._messageBusStop = MessageBus.CreateNode("ecs");
		this._messageBusStop.OnMessageReceived += OnMessageReceived;
	}

	private void OnMessageReceived( Message message ) {
		if (message.Content is EntityContainerListRequest listRequest)
			foreach (EntityContainer container in this._containers)
				message.SendResponseFrom( this._messageBusStop, new EntityContainerRequestResponse( container ) );
	}

	public EntityContainer CreateContainer() {
		EntityContainer container = new();
		this._containers.Add( container );
		container.OnDisposed += OnContainerDisposed;
		this._messageBusStop.Publish( new EntityContainerCreatedEvent( container ), null );
		return container;
	}

	private void OnContainerDisposed( IListenableDisposable disposable ) {
		List<EntityContainer> containersToRemove = [ .. this._containers.Where( p => p.Disposed ) ];
		foreach (EntityContainer container in containersToRemove)
			this._containers.Remove( container );
	}

	protected override bool InternalDispose() {
		foreach (EntityContainer container in this._containers) {
			container.OnDisposed -= OnContainerDisposed;
			container.Dispose();
		}
		return true;
	}

	public void Update( double time, double deltaTime ) {
		this._messageBusStop.ProcessQueue();
		foreach (EntityContainer container in this._containers) {
			container.ReadMessages();
			container.SystemManager.Update( time, deltaTime );
		}
	}
}
