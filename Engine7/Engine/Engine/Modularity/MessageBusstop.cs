
using System.Collections.Concurrent;

namespace Engine.Modularity;

public sealed class MessageBusStop : DisposableIdentifiable {
	private readonly ConcurrentQueue<Message> _messageQueue;
	private readonly MessageQueueInterface _interface;

	public event Action<Message>? OnMessageReceived;

	public MessageBusStop() {
		_messageQueue = [];
		_interface = new( this );
	}

	internal void ReceiveMessage( Message message ) => _messageQueue.Enqueue( message );

	public void ProcessQueue() {
		ObjectDisposedException.ThrowIf( Disposed, this );
		while (_messageQueue.TryDequeue( out Message? message ))
			OnMessageReceived?.Invoke( message );
	}

	public void Publish( object content ) {
		ObjectDisposedException.ThrowIf( Disposed, this );
		MessageBus.Publish( new( _interface, content ) );
	}

	public void SendMessageTo( MessageQueueInterface messageQueue, object content ) {
		ObjectDisposedException.ThrowIf( Disposed, this );
		messageQueue.LeaveMessage( new( _interface, content ) );
	}

	protected override bool InternalDispose() => true;
}

public sealed class Message( MessageQueueInterface? sender, object content ) {
	public readonly MessageQueueInterface? Sender = sender;
	public readonly object Content = content;
}

public sealed class MessageQueueInterface {
	private readonly MessageBusStop _manager;

	public MessageQueueInterface( MessageBusStop manager ) {
		this._manager = manager;
	}

	public void LeaveMessage( Message message ) {
		_manager.ReceiveMessage( message );
	}
}

public static class MessageExtensions {
	public static void ResponseFrom( this Message message, MessageBusStop manager, object content ) {
		if (message.Sender is null)
			throw new InvalidOperationException( "Cannot respond to a message without a known sender!" );
		manager.SendMessageTo( message.Sender, content );
	}
}