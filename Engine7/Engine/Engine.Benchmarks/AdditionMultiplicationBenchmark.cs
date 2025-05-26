using BenchmarkDotNet.Attributes;

namespace Engine.Benchmarks;

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

public class PowExpLogBenchmark {

	public float _result;

	public float _base;
	public float _exponent;

	[GlobalSetup]
	public void Setup() {
		this._base = 0;
		this._exponent = 3f/5f;
		this._result = 0f;
	}

	[Benchmark]
	public float Pow() {
		_base++;
		return _result = MathF.Pow( _base, _exponent );
	}

	[Benchmark]
	public float Exp() {
		_base++;
		return _result = MathF.Exp( MathF.Log(_base) * _exponent );
	}

	[Benchmark]
	public float Polynomial() {
		_base++;
		float y = _base - 1;
		float y2 = y * y;
		float y3 = y2 * y;
		float y4 = y3 * y;
		float y5 = y4 * y;
		float y6 = y5 * y;

		return 1f
			+ 0.6f * y
			- 0.12f * y2
			+ 0.048f * y3
			- 0.027f * y4
			+ 0.018f * y5
			- 0.013f * y6;
	}
}