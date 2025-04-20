using Engine.Modularity;
using Engine.Module.Entities.Messages;

namespace Engine.Standard.Render.Entities.Services;

public class RenderBehaviourEntityContainerEventReceiverService : DisposableIdentifiable, IUpdateable {

	private readonly MessageBusNode _messageBusNode;
	private readonly RenderEntityContainerService _renderEntityContainerService;

	public RenderBehaviourEntityContainerEventReceiverService( RenderEntityContainerService renderEntityContainerService ) {
		this._messageBusNode = MessageBus.CreateNode("render_ecs");
		this._messageBusNode.OnMessageReceived += OnMessageReceived;
		this._renderEntityContainerService = renderEntityContainerService;
		this._messageBusNode.Publish( new EntityContainerListRequest(), null );
	}

	private void OnMessageReceived( Message obj ) {
		if (obj.Content is EntityContainerCreatedEvent entityContainerCreatedEvent)
			this._renderEntityContainerService.RegisterEntityContainer( entityContainerCreatedEvent.EntityContainer );
		if (obj.Content is EntityContainerRequestResponse entityContainerRequestResponse)
			this._renderEntityContainerService.RegisterEntityContainer( entityContainerRequestResponse.EntityContainer );
	}

	public void Update( double time, double deltaTime ) {
		this._messageBusNode.ProcessQueue();
	}

	protected override bool InternalDispose() {
		this._messageBusNode.Dispose();
		return true;
	}
}

