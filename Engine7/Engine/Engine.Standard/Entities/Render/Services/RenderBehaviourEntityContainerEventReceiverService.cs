using Engine.Modularity;
using Engine.Module.Entities.Messages;

namespace Engine.Standard.Entities.Render.Services;

public class RenderBehaviourEntityContainerEventReceiverService : DisposableIdentifiable, IUpdateable {

	private readonly MessageReceiver _messageReceiver;
	private readonly RenderEntityContainerService _renderEntityContainerService;

	public RenderBehaviourEntityContainerEventReceiverService( RenderEntityContainerService renderEntityContainerService ) {
		_messageReceiver = MessageBus.CreateMessageReceiver();
		_messageReceiver.OnMessageReceived += OnMessageReceived;
		this._renderEntityContainerService = renderEntityContainerService;
		MessageBus.SendMessage( new EntityContainerListRequest() );
	}

	private void OnMessageReceived( object obj ) {
		if (obj is EntityContainerCreatedEvent entityContainerCreatedEvent)
			_renderEntityContainerService.RegisterEntityContainer( entityContainerCreatedEvent.EntityContainer );
		if (obj is EntityContainerRequestResponse entityContainerRequestResponse)
			_renderEntityContainerService.RegisterEntityContainer( entityContainerRequestResponse.EntityContainer );
	}

	public void Update( double time, double deltaTime ) {
		_messageReceiver.ProcessQueue();
	}

	protected override bool InternalDispose() {
		_messageReceiver.Dispose();
		return true;
	}
}

