
using Engine.Logging;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Engine.Modularity;

public static class MessageBus {
	private static int _currentIndex;
	private static readonly ConcurrentDictionary<int, MessageBusNode> _nodes = [];

	public static MessageBusNode CreateNode( string? address ) {
		Log.Line( $"Created {nameof( MessageBusNode )} with address {address}!" );
		int index = Interlocked.Increment( ref _currentIndex );
		MessageBusNode receiver = new( index, address );
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
		Log.Line( $"Publishing {message.Content}{(!string.IsNullOrEmpty( message.Address ) ? $" to {message.Address}" : "")}..." );
		if (string.IsNullOrEmpty( message.Address )) {
			foreach (MessageBusNode receiver in _nodes.Values) {
				if (receiver.IsSenderOf( message ))
					continue;
				receiver.ReceiveMessage( message );
			}
			return;
		}

		Regex regex = new( message.Address, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant );

		foreach (MessageBusNode receiver in _nodes.Values) {
			if (receiver.IsSenderOf( message ))
				continue;

			if (string.IsNullOrEmpty( receiver.Address )) {
				receiver.ReceiveMessage( message );
				continue;
			}

			if (regex.IsMatch( receiver.Address ))
				receiver.ReceiveMessage( message );
		}
	}
}