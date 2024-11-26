using Engine.Modularity;
using Engine.Module.Entities.Messages;

namespace Engine.Standard.Entities.Render.Services;

public class RenderBehaviourEntityContainerEventReceiverService : DisposableIdentifiable, IUpdateable {

	private readonly MessageBusStop _messageBusStop;
	private readonly RenderEntityContainerService _renderEntityContainerService;

	public RenderBehaviourEntityContainerEventReceiverService( RenderEntityContainerService renderEntityContainerService ) {
		_messageBusStop = MessageBus.CreateManager();
		_messageBusStop.OnMessageReceived += OnMessageReceived;
		this._renderEntityContainerService = renderEntityContainerService;
		_messageBusStop.Publish( new EntityContainerListRequest() );
	}

	private void OnMessageReceived( object obj ) {
		if (obj is EntityContainerCreatedEvent entityContainerCreatedEvent)
			_renderEntityContainerService.RegisterEntityContainer( entityContainerCreatedEvent.EntityContainer );
		if (obj is EntityContainerRequestResponse entityContainerRequestResponse)
			_renderEntityContainerService.RegisterEntityContainer( entityContainerRequestResponse.EntityContainer );
	}

	public void Update( double time, double deltaTime ) {
		_messageBusStop.ProcessQueue();
	}

	protected override bool InternalDispose() {
		_messageBusStop.Dispose();
		return true;
	}
}

