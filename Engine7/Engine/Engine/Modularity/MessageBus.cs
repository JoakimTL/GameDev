namespace Engine.Modularity;

public static class MessageBus {
	private static readonly List<MessageReceiver> _receivers = [];
	private static readonly Lock _lock = new();

	public static MessageReceiver CreateMessageReceiver() {
		MessageReceiver receiver = new();
		lock (_lock)
			_receivers.Add( receiver );
		receiver.OnDisposed += ReceiverDisposed;
		return receiver;
	}

	private static void ReceiverDisposed() {
		lock (_lock)
			_receivers.RemoveAll( p => p.Disposed );
	}

	public static void SendMessage( object message ) {
		lock (_lock)
			foreach (MessageReceiver receiver in _receivers)
				receiver.ReceiveMessage( message );
	}
}