
using System.Collections.Concurrent;

namespace Engine.Modularity;

public sealed class MessageReceiver : DisposableIdentifiable {
	private readonly ConcurrentQueue<object> _messageQueue = [];
	public event Action<object>? OnMessageReceived;

	public void ProcessQueue() {
		ObjectDisposedException.ThrowIf( Disposed, this );
		while (_messageQueue.TryDequeue( out object? message ))
			OnMessageReceived?.Invoke( message );
	}

	internal void ReceiveMessage( object message ) => _messageQueue.Enqueue( message );

	protected override bool InternalDispose() => true;
}
