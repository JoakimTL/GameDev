//using BenchmarkDotNet.Attributes;

//namespace Engine.Math.Benchmarks;

//public class NonSIMDDoubleBenchmarks {

//	//Copy NumericsBenchmarks.cs here, but use Engine.Math instead of System.Numerics

//	[Benchmark]
//	public Vector2<double> Vector2Add() {
//		var a = new Vector2<double>( 1f, 2f );
//		var b = new Vector2<double>( 3f, 4f );
//		return a + b;
//	}

//	[Benchmark]
//	public Vector3<double> Vector3Add() {
//		var a = new Vector3<double>( 1f, 2f, 3f );
//		var b = new Vector3<double>( 4f, 5f, 6f );
//		return a + b;
//	}

//	[Benchmark]
//	public Vector4<double> Vector4Add() {
//		var a = new Vector4<double>( 1f, 2f, 3f, 4f );
//		var b = new Vector4<double>( 5f, 6f, 7f, 8f );
//		return a + b;
//	}

//	[Benchmark]
//	public Vector2<double> Vector2Subtract() {
//		var a = new Vector2<double>( 1f, 2f );
//		var b = new Vector2<double>( 3f, 4f );
//		return a - b;
//	}

//	[Benchmark]
//	public Vector3<double> Vector3Subtract() {
//		var a = new Vector3<double>( 1f, 2f, 3f );
//		var b = new Vector3<double>( 4f, 5f, 6f );
//		return a - b;
//	}

//	[Benchmark]
//	public Vector4<double> Vector4Subtract() {
//		var a = new Vector4<double>( 1f, 2f, 3f, 4f );
//		var b = new Vector4<double>( 5f, 6f, 7f, 8f );
//		return a - b;
//	}

//	[Benchmark]
//	public Vector2<double> Vector2Multiply() {
//		var a = new Vector2<double>( 1f, 2f );
//		var b = new Vector2<double>( 3f, 4f );
//		return a * b;
//	}

//	[Benchmark]
//	public Vector3<double> Vector3Multiply() {
//		var a = new Vector3<double>( 1f, 2f, 3f );
//		var b = new Vector3<double>( 4f, 5f, 6f );
//		return a * b;
//	}

//	[Benchmark]
//	public Vector4<double> Vector4Multiply() {
//		var a = new Vector4<double>( 1f, 2f, 3f, 4f );
//		var b = new Vector4<double>( 5f, 6f, 7f, 8f );
//		return a * b;
//	}

//	[Benchmark]
//	public Vector2<double> Vector2Divide() {
//		var a = new Vector2<double>( 1f, 2f );
//		var b = new Vector2<double>( 3f, 4f );
//		return a / b;
//	}

//	[Benchmark]
//	public Vector3<double> Vector3Divide() {
//		var a = new Vector3<double>( 1f, 2f, 3f );
//		var b = new Vector3<double>( 4f, 5f, 6f );
//		return a / b;
//	}

//	[Benchmark]
//	public Vector4<double> Vector4Divide() {
//		var a = new Vector4<double>( 1f, 2f, 3f, 4f );
//		var b = new Vector4<double>( 5f, 6f, 7f, 8f );
//		return a / b;
//	}

//	[Benchmark]
//	public double Vector2Dot() {
//		var a = new Vector2<double>( 1f, 2f );
//		var b = new Vector2<double>( 3f, 4f );
//		return a.Dot( b );
//	}

//	[Benchmark]
//	public double Vector3Dot() {
//		var a = new Vector3<double>( 1f, 2f, 3f );
//		var b = new Vector3<double>( 4f, 5f, 6f );
//		return a.Dot( b );
//	}

//	[Benchmark]
//	public double Vector4Dot() {
//		var a = new Vector4<double>( 1f, 2f, 3f, 4f );
//		var b = new Vector4<double>( 5f, 6f, 7f, 8f );
//		return a.Dot( b );
//	}

//	[Benchmark]
//	public Vector2<double> Vector2Normalize() {
//		var a = new Vector2<double>( 1f, 2f );
//		return a.Normalize();
//	}

//	[Benchmark]
//	public Vector3<double> Vector3Normalize() {
//		var a = new Vector3<double>( 1f, 2f, 3f );
//		return a.Normalize();
//	}

//	[Benchmark]
//	public Vector4<double> Vector4Normalize() {
//		var a = new Vector4<double>( 1f, 2f, 3f, 4f );
//		return a.Normalize();
//	}

//	[Benchmark]
//	public Vector3<double> Vector3Cross() {
//		var a = new Vector3<double>( 1f, 2f, 3f );
//		var b = new Vector3<double>( 4f, 5f, 6f );
//		return a.Cross( b );
//	}
//}
