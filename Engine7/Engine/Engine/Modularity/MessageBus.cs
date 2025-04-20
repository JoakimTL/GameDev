
using System.Collections.Concurrent;

namespace Engine.Modularity;

public static class MessageBus {
	private static int _currentIndex;
	private static readonly ConcurrentDictionary<int, MessageBusNode> _nodes = [];

	public static MessageBusNode CreateNode(string? address) {
		int index = Interlocked.Increment( ref _currentIndex );
		MessageBusNode receiver = new( index , address);
		_nodes.TryAdd( index, receiver );
		receiver.OnDisposed += ReceiverDisposed;
		return receiver;
	}

	private static void ReceiverDisposed( IListenableDisposable disposable ) {
		if (disposable is not MessageBusNode receiver)
			return;
		_nodes.TryRemove( receiver.Index, out _ );
		receiver.OnDisposed -= ReceiverDisposed;
	}

	public static void PublishAnonymously( object content, string? address = default ) => Publish( new( null, content, address ) );

	//TODO: finish stuff. (addresses being simple regex, etc...) Make naming scheme understandable...
	public static void Publish( Message message ) {
		if (message == null)
			throw new ArgumentNullException( nameof( message ) );
		if (message.Address == null) {
			foreach (MessageBusNode receiver in _nodes.Values)
				receiver.ReceiveMessage( message );
		} else {
			foreach (MessageBusNode receiver in _nodes.Values)
				if (receiver.Address == message.Address)
					receiver.ReceiveMessage( message );
		}
	}
}