
using System.Collections.Concurrent;

namespace Engine.Modularity;

public sealed class MessageBusNode : DisposableIdentifiable {
	private readonly ConcurrentQueue<Message> _messageQueue;
	private readonly MessageQueueInterface _interface;

	public int Index { get; }
	public string? Address { get; }

	public event Action<Message>? OnMessageReceived;

	public MessageBusNode(int index, string? address) {
		this._messageQueue = [];
		this._interface = new( this );
		this.Index = index;
		this.Address = address;
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
		messageQueue.LeaveMessageAtNode( new( this._interface, content ) );
	}

	protected override bool InternalDispose() => true;
}

public sealed class Message( MessageQueueInterface? sender, object content, string? address = default ) {
	public readonly MessageQueueInterface? Sender = sender;
	public readonly object Content = content;
	public readonly string? Address = address;
}

public sealed class MessageQueueInterface {
	private readonly MessageBusNode _manager;

	public MessageQueueInterface( MessageBusNode manager ) {
		this._manager = manager;
	}

	public void LeaveMessageAtNode( Message message ) {
		this._manager.ReceiveMessage( message );
	}
}

public static class MessageExtensions {
	public static void SendResponseFrom( this Message message, MessageBusNode node, object content ) {
		if (message.Sender is null)
			throw new InvalidOperationException( "Cannot respond to a message without a known sender!" );
		node.SendMessageTo( message.Sender, content );
	}
}