
namespace Engine.Modularity;

public abstract class MessageBusServiceBase : IUpdateable, IHostedService {
	protected readonly MessageBusNode MessageBusNode;

	protected MessageBusServiceBase( string? address ) {
		MessageBusNode = MessageBus.CreateNode( address );
		MessageBusNode.OnMessageProcessed += MessageProcessed;
	}

	public void Update( double time, double deltaTime ) => MessageBusNode.ProcessQueue();

	protected abstract void MessageProcessed( Message message );

}