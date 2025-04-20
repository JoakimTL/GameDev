
using System.Collections.Concurrent;

namespace Engine.Modularity;

public sealed class MessageBusNode : DisposableIdentifiable {
	private readonly ConcurrentQueue<Message> _messageQueue;
	private readonly MessageQueueInterface _interface;

	public int Index { get; }
	public string? Address { get; }

	public event Action<Message>? OnMessageReceived;

	public MessageBusNode( int index, string? address ) {
		this._messageQueue = [];
		this._interface = new( this );
		this.Index = index;
		this.Address = address;
		this.Nickname = $"{address}#{index}";
	}

	internal void ReceiveMessage( Message message ) => this._messageQueue.Enqueue( message );

	public void ProcessQueue() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		while (this._messageQueue.TryDequeue( out Message? message ))
			OnMessageReceived?.Invoke( message );
	}

	public void Publish( object content, string? address ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		MessageBus.Publish( new( this._interface, content, address ) );
	}

	public void SendMessageTo( MessageQueueInterface messageQueue, object content ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		messageQueue.LeaveMessageAtNode( new( this._interface, content, null ) );
	}

	protected override bool InternalDispose() => true;

	public bool IsSenderOf( Message message ) => message.Sender == this._interface;
}
