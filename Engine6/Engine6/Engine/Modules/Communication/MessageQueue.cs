using System.Collections.Concurrent;

namespace Engine.Modules.Communication;

public sealed class MessageQueue<T> : IDisposable {

	private readonly ConcurrentQueue<T> _queue;
	private readonly HashSet<IMessageEmitter> _emitters;

	public MessageQueue() {
		this._emitters = new();
		this._queue = new();
	}

	public void Listen( IMessageEmitter emitter ) {
		if ( this._emitters.Add( emitter ) )
			emitter.MessageEmitted += OnMessageEmitted;
	}

	public void StopListening( IMessageEmitter emitter ) {
		if ( this._emitters.Remove( emitter ) )
			emitter.MessageEmitted -= OnMessageEmitted;
	}

	private void OnMessageEmitted( in object message ) {
		if ( message is T t )
			this._queue.Enqueue( t );
	}

	public void Dispose() {
		foreach ( IMessageEmitter emitter in this._emitters )
			emitter.MessageEmitted -= OnMessageEmitted;
		this._emitters.Clear();
	}
}

public sealed class MessageEmitterService {



}