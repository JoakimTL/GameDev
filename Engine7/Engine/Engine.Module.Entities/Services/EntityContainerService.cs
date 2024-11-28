using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Messages;

namespace Engine.Module.Entities.Services;
public sealed class EntityContainerService : DisposableIdentifiable, IUpdateable {

	private readonly List<EntityContainer> _containers;
	private readonly MessageBusStop _messageBusStop;

	public EntityContainerService() {
		this._containers = [];
		this._messageBusStop = MessageBus.CreateManager();
		this._messageBusStop.OnMessageReceived += OnMessageReceived;
	}

	private void OnMessageReceived( Message message ) {
		if (message.Content is EntityContainerListRequest listRequest)
			foreach (EntityContainer container in this._containers)
				message.ResponseFrom( this._messageBusStop, new EntityContainerRequestResponse( container ) );
	}

	public EntityContainer CreateContainer() {
		EntityContainer container = new();
		this._containers.Add( container );
		container.OnDisposed += OnContainerDisposed;
		this._messageBusStop.Publish( new EntityContainerCreatedEvent( container ) );
		return container;
	}

	private void OnContainerDisposed() {
		List<EntityContainer> containersToRemove = this._containers.Where( p => p.Disposed ).ToList();
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
		foreach (EntityContainer container in this._containers)
			container.SystemManager.Update( time, deltaTime );
	}
}
