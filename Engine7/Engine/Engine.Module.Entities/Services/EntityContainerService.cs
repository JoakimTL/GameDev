using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Messages;

namespace Engine.Module.Entities.Services;
public sealed class EntityContainerService : DisposableIdentifiable, IUpdateable {

	private readonly List<EntityContainer> _containers;
	private readonly MessageBusStop _messageBusStop;

	public EntityContainerService() {
		_containers = [];
		_messageBusStop = MessageBus.CreateManager();
		_messageBusStop.OnMessageReceived += OnMessageReceived;
	}

	private void OnMessageReceived( Message message ) {
		if (message.Content is EntityContainerListRequest listRequest)
			foreach (EntityContainer container in _containers)
				message.ResponseFrom( _messageBusStop, new EntityContainerRequestResponse( container ) );
	}

	public EntityContainer CreateContainer() {
		EntityContainer container = new();
		_containers.Add( container );
		container.OnDisposed += OnContainerDisposed;
		_messageBusStop.Publish( new EntityContainerCreatedEvent( container ) );
		return container;
	}

	private void OnContainerDisposed() {
		List<EntityContainer> containersToRemove = _containers.Where( p => p.Disposed ).ToList();
		foreach (EntityContainer container in containersToRemove)
			_containers.Remove( container );
	}

	protected override bool InternalDispose() {
		foreach (EntityContainer container in _containers) {
			container.OnDisposed -= OnContainerDisposed;
			container.Dispose();
		}
		return true;
	}

	public void Update( double time, double deltaTime ) {
		_messageBusStop.ProcessQueue();
		foreach (EntityContainer container in _containers)
			container.SystemManager.Update( time, deltaTime );
	}
}
