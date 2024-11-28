
using System.Collections.Concurrent;

namespace Engine.Modularity;

public sealed class MessageBusStop : DisposableIdentifiable {
	private readonly ConcurrentQueue<Message> _messageQueue;
	private readonly MessageQueueInterface _interface;

	public event Action<Message>? OnMessageReceived;

	public MessageBusStop() {
		this._messageQueue = [];
		this._interface = new( this );
	}

	internal void ReceiveMessage( Message message ) => this._messageQueue.Enqueue( message );

	public void ProcessQueue() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		while (this._messageQueue.TryDequeue( out Message? message ))
			OnMessageReceived?.Invoke( message );
	}

	public void Publish( object content ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		MessageBus.Publish( new( this._interface, content ) );
	}

	public void SendMessageTo( MessageQueueInterface messageQueue, object content ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		messageQueue.LeaveMessage( new( this._interface, content ) );
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
		this._manager.ReceiveMessage( message );
	}
}

public static class MessageExtensions {
	public static void ResponseFrom( this Message message, MessageBusStop manager, object content ) {
		if (message.Sender is null)
			throw new InvalidOperationException( "Cannot respond to a message without a known sender!" );
		manager.SendMessageTo( message.Sender, content );
	}
}