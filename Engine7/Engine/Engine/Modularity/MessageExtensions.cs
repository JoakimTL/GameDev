namespace Engine.Modularity;

public static class MessageExtensions {
	public static void SendResponseFrom( this Message message, MessageBusNode node, object content ) {
		if (message.Sender is null)
			throw new InvalidOperationException( "Cannot respond to a message without a known sender!" );
		node.SendMessageTo( message.Sender, content );
	}
}