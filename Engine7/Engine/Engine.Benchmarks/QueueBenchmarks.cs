using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;

namespace Engine.Benchmarks;

[MemoryDiagnoser]
public class QueueBenchmarks {

	private int _lastMessage;
	private ConcurrentQueue<int> _queue = [];
	private BlockingCollection<int> _blockingCollection = [];

	[Benchmark]
	public void Queue_AddIntsAsString_ReadAfter() {
		for (int i = 0; i < 10; i++) {
			this._queue.Enqueue( i );
		}
		for (int i = 0; i < 10; i++) {
			this._queue.TryDequeue( out this._lastMessage );
		}
	}

	[Benchmark]
	public void BlockingCollection_AddIntsAsString_ReadAfter() {
		for (int i = 0; i < 10; i++) {
			this._blockingCollection.Add( i );
		}
		for (int i = 0; i < 10; i++) {
			this._blockingCollection.TryTake( out this._lastMessage );
		}
	}

}