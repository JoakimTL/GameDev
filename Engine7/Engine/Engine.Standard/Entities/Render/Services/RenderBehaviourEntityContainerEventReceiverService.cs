using Engine.Modularity;
using Engine.Module.Entities.Messages;

namespace Engine.Standard.Entities.Render.Services;

public class RenderBehaviourEntityContainerEventReceiverService : DisposableIdentifiable, IUpdateable {

	private readonly MessageBusStop _messageBusStop;
	private readonly RenderEntityContainerService _renderEntityContainerService;

	public RenderBehaviourEntityContainerEventReceiverService( RenderEntityContainerService renderEntityContainerService ) {
		this._messageBusStop = MessageBus.CreateManager();
		this._messageBusStop.OnMessageReceived += OnMessageReceived;
		this._renderEntityContainerService = renderEntityContainerService;
		this._messageBusStop.Publish( new EntityContainerListRequest() );
	}

	private void OnMessageReceived( Message obj ) {
		if (obj.Content is EntityContainerCreatedEvent entityContainerCreatedEvent)
			this._renderEntityContainerService.RegisterEntityContainer( entityContainerCreatedEvent.EntityContainer );
		if (obj.Content is EntityContainerRequestResponse entityContainerRequestResponse)
			this._renderEntityContainerService.RegisterEntityContainer( entityContainerRequestResponse.EntityContainer );
	}

	public void Update( double time, double deltaTime ) {
		this._messageBusStop.ProcessQueue();
	}

	protected override bool InternalDispose() {
		this._messageBusStop.Dispose();
		return true;
	}
}

