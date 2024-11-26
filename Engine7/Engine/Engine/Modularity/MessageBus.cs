namespace Engine.Modularity;

public static class MessageBus {
	private static readonly List<MessageBusStop> _managers = [];
	private static readonly Lock _lock = new();

	public static MessageBusStop CreateManager() {
		MessageBusStop receiver = new();
		lock (_lock)
			_managers.Add( receiver );
		receiver.OnDisposed += ReceiverDisposed;
		return receiver;
	}

	private static void ReceiverDisposed() {
		lock (_lock)
			_managers.RemoveAll( p => p.Disposed );
	}

	public static void PublishAnonymously( object content ) => Publish( new( null, content ) );

	public static void Publish( Message message ) {
		lock (_lock)
			foreach (MessageBusStop receiver in _managers)
				receiver.ReceiveMessage( message );
	}
}