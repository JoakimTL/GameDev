using BenchmarkDotNet.Attributes;

namespace Engine.Benchmarks;

[MemoryDiagnoser]
public class EventsVsAbstractMethod {

	private abstract class AbstractTest {
		public abstract void Test( int i );
	}

	private sealed class Implementation : AbstractTest {
		public int _latestValue = 0;
		public override void Test( int i ) { this._latestValue = i; }
	}

	private sealed class EventTest {
		public int _latestValue = 0;
		public event Action<int>? Event;

		public EventTest( int num ) {
			for (int i = 0; i < num; i++)
				Event += Test;
		}

		private void Test( int i ) => this._latestValue = i;
		public void CallEvent( int i ) => Event?.Invoke( i );
	}

	private readonly AbstractTest _testAbstract = new Implementation();
	private readonly EventTest _testEvent1 = new( 1 );
	private readonly EventTest _testEvent1000 = new( 1000 );

	[Benchmark]
	public void AbstractMethod_1() {
		this._testAbstract.Test( 4 );
	}

	[Benchmark]
	public void AbstractMethod_1000() {
		for (int i = 0; i < 1000; i++) {
			this._testAbstract.Test( 4 );
		}
	}

	[Benchmark]
	public void Event_1() {
		this._testEvent1.CallEvent( 4 );
	}

	[Benchmark]
	public void Event_1000() {
		this._testEvent1000.CallEvent( 4 );
	}


}