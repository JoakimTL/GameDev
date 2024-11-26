using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Messages;

namespace Engine.Module.Entities.Services;
public sealed class EntityContainerService : DisposableIdentifiable, IUpdateable {

	private readonly List<EntityContainer> _containers;
	private readonly MessageReceiver _messageReceiver;

	public EntityContainerService() {
		_containers = [];
		_messageReceiver = MessageBus.CreateMessageReceiver();
		_messageReceiver.OnMessageReceived += OnMessageReceived;
	}

	private void OnMessageReceived( object obj ) {
		if (obj is EntityContainerListRequest listRequest)
			foreach (EntityContainer container in _containers)
				MessageBus.SendMessage( new EntityContainerRequestResponse( container ) );
	}

	public EntityContainer CreateContainer() {
		EntityContainer container = new();
		_containers.Add( container );
		container.OnDisposed += OnContainerDisposed;
		MessageBus.SendMessage( new EntityContainerCreatedEvent( container ) );
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
		_messageReceiver.ProcessQueue();
		foreach (EntityContainer container in _containers)
			container.SystemManager.Update( time, deltaTime );
	}
}
