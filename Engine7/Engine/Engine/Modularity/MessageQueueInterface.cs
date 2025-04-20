using Engine.Logging;

namespace Engine.Modularity;

public sealed class MessageQueueInterface( MessageBusNode node ) {
	private readonly MessageBusNode _node = node;
	public void LeaveMessageAtNode( Message message ) {
		Log.Line( $"{_node} received {message.Content}..." );
		this._node.ReceiveMessage( message );
	}
}
