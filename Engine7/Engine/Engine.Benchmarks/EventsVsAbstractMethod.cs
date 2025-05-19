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

public class AdditionMultiplicationBenchmark {

	public long _intAddPartA;
	public long _intAddPartB;
	public long _intSum;
	public long _intMulPartA;
	public long _intMulPartB;
	public long _intMul;
	public double _floatAddPartA;
	public double _floatAddPartB;
	public double _floatSum;
	public double _floatMulPartA;
	public double _floatMulPartB;
	public double _floatMul;

	[GlobalSetup]
	public void Setup() {
		this._intSum = 0;
		this._intMul = 0;
		this._floatSum = 0;
		this._floatMul = 0;

		this._intAddPartA = Random.Shared.Next( 1, 4 );
		this._intAddPartB = Random.Shared.Next( 1, 4 );
		this._intMulPartA = Random.Shared.Next( 1, 4 );
		this._intMulPartB = Random.Shared.Next( 1, 4 );
		this._floatAddPartA = Random.Shared.NextDouble() + 0.5;
		this._floatAddPartB = Random.Shared.NextDouble() + 0.5;
		this._floatMulPartA = Random.Shared.NextDouble() + 0.5;
		this._floatMulPartB = Random.Shared.NextDouble() + 0.5;

	}

	[Benchmark]
	public long AddInt() {
		return _intSum += _intAddPartA + _intAddPartB;
	}

	[Benchmark]
	public long MulInt() {
		return _intMul += _intMulPartA * _intMulPartB;
	}

	[Benchmark]
	public double AddFloat() {
		return _floatSum += _floatAddPartA + _floatAddPartB;
	}

	[Benchmark]
	public double MulFloat() {
		return _floatMul += _floatMulPartA * _floatMulPartB;
	}


}