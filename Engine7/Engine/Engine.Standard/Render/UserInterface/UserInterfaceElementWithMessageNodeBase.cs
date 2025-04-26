using Engine.Modularity;

namespace Engine.Standard.Render.UserInterface;

public abstract class UserInterfaceElementWithMessageNodeBase : UserInterfaceElementBase {

	protected readonly MessageBusNode MessageBusNode;

	protected UserInterfaceElementWithMessageNodeBase( string address ) {
		MessageBusNode = MessageBus.CreateNode( address );
		MessageBusNode.OnMessageProcessed += OnMessageReceived;
	}

	protected abstract void OnMessageReceived( Message message );

	protected void Publish( object content, string? address )
		=> MessageBusNode.Publish( content, address );

	protected override void OnUpdate( double time, double deltaTime )
		=> MessageBusNode.ProcessQueue();
}