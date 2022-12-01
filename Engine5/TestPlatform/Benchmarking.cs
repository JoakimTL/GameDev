//using BenchmarkDotNet.Attributes;

//namespace TestPlatform;

//public class Benchmarking {

//	[Params( 10000 )]
//	public int Size { get; set; }

//	private float[] values32f;
//	private double[] values64f;
//	private int[] values32;
//	private long[] values64;
//	private Int128[] values128;
//	private uint[] values32u;
//	private ulong[] values64u;
//	private UInt128[] values128u;

//	private float ff = 2;
//	private double dd = 2;

//	[GlobalSetup]
//	public void Setup() {
//		values32f = new float[ Size ];
//		values64f = new double[ Size ];
//		values32 = new int[ Size ];
//		values64 = new long[ Size ];
//		values128 = new Int128[ Size ];
//		values32u = new uint[ Size ];
//		values64u = new ulong[ Size ];
//		values128u = new UInt128[ Size ];

//		Random random = new();

//		for ( int i = 0; i < Size; i++ ) {
//			values32f[ i ] = random.NextSingle();
//			values64f[ i ] = random.NextDouble();
//			values32[ i ] = random.Next();
//			values64[ i ] = random.NextInt64();
//			values128[ i ] = unchecked(new Int128( (ulong) random.NextInt64(), (ulong) random.NextInt64() ));
//			values32u[ i ] = unchecked((uint) random.Next());
//			values64u[ i ] = unchecked((ulong) random.NextInt64());
//			values128u[ i ] = unchecked(new UInt128( (ulong) random.NextInt64(), (ulong) random.NextInt64() ));
//		}
//	}

//	//[Benchmark]
//	//public void FloatToDouble() {
//	//	dd = (double) 4.3151341f;
//	//	dd = (double) 4.4123523f;
//	//	dd = (double) 4.3166442f;
//	//	dd = (double) 4.3156421f;
//	//	dd = (double) 4.3123221f;
//	//	dd = (double) 5.3151451f;
//	//}

//	//[Benchmark]
//	//public void DoubleToFloat() {
//	//	ff = (float) 4.315124345621;
//	//	ff = (float) 4.412351332523;
//	//	ff = (float) 4.316642353442;
//	//	ff = (float) 4.315647688621;
//	//	ff = (float) 4.312324345621;
//	//	ff = (float) 5.315124345621;
//	//}

//	[Benchmark]
//	public void FloatMathSin() {
//		unchecked {
//			float sum = 0;
//			for ( float i = 0; i < 10; i += 0.01f )
//				sum += MathF.Sin( i );
//		}
//	}

//	[Benchmark]
//	public void FloatMathSqrt() {
//		unchecked {
//			float sum = 0;
//			for ( float i = 1; i < Size; i++ )
//				sum += MathF.Sqrt( i );
//		}
//	}

//	[Benchmark]
//	public void DoubleMathSin() {
//		unchecked {
//			double sum = 0;
//			for ( float i = 0; i < 10; i += 0.01f )
//				sum += Math.Sin( i );
//		}
//	}

//	[Benchmark]
//	public void DoubleMathSqrt() {
//		unchecked {
//			double sum = 0;
//			for ( float i = 1; i < Size; i++ )
//				sum += Math.Sqrt( i );
//		}
//	}

//	[Benchmark]
//	public void DoubleMathSinF() {
//		unchecked {
//			double sum = 0;
//			for ( float i = 0; i < 10; i += 0.01f )
//				sum += MathF.Sin( i );
//		}
//	}

//	[Benchmark]
//	public void DoubleMathSqrtF() {
//		unchecked {
//			double sum = 0;
//			for ( float i = 1; i < Size; i++ )
//				sum += MathF.Sqrt( i );
//		}
//	}

//	[Benchmark]
//	public void DoubleMathSinFD() {
//		unchecked {
//			double sum = 0;
//			for ( double i = 0; i < 10; i += 0.01f )
//				sum += MathF.Sin( (float) i );
//		}
//	}

//	[Benchmark]
//	public void DoubleMathSqrtFD() {
//		unchecked {
//			double sum = 0;
//			for ( double i = 1; i < Size; i++ )
//				sum += MathF.Sqrt( (float) i );
//		}
//	}

//	//[Benchmark]
//	//public void Uint32Add() {
//	//	unchecked {
//	//		uint sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += values32u[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Uint64Add() {
//	//	unchecked {
//	//		ulong sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += values64u[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Uint128Add() {
//	//	unchecked {
//	//		UInt128 sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += values128u[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Int32Add() {
//	//	unchecked {
//	//		int sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += values32[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Int64Add() {
//	//	unchecked {
//	//		long sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += values64[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Int128Add() {
//	//	unchecked {
//	//		Int128 sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += values128[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Uint32Mul() {
//	//	unchecked {
//	//		uint sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum *= values32u[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Uint64Mul() {
//	//	unchecked {
//	//		ulong sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum *= values64u[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Uint128Mul() {
//	//	unchecked {
//	//		UInt128 sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum *= values128u[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Int32Mul() {
//	//	unchecked {
//	//		int sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum *= values32[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Int64Mul() {
//	//	unchecked {
//	//		long sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum *= values64[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void Int128Mul() {
//	//	unchecked {
//	//		Int128 sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum *= values128[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void F32Add() {
//	//	unchecked {
//	//		float sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += values32f[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void F64Add() {
//	//	unchecked {
//	//		double sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += values64f[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void F32Mul() {
//	//	unchecked {
//	//		float sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum *= values32f[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void F64Mul() {
//	//	unchecked {
//	//		double sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum *= values64f[ i ];
//	//	}
//	//}

//	////F32 and F64 is equally fast

//	//[Benchmark]
//	//public void F32CastAdd64() {
//	//	unchecked {
//	//		float sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += (float) values64[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void F64CastAdd64() {
//	//	unchecked {
//	//		double sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += (double) values64[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void F32CastAdd128() {
//	//	unchecked {
//	//		float sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += (float) values128[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void F64CastAdd128() {
//	//	unchecked {
//	//		double sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += (double) values128[ i ];
//	//	}
//	//}

//	//[Benchmark]
//	//public void F32CastAdd128p() {
//	//	unchecked {
//	//		float sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += (float) ( (long) values128[ i ] );
//	//	}
//	//}

//	//[Benchmark]
//	//public void F64CastAdd128p() {
//	//	unchecked {
//	//		double sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum += (double) ( (long) values128[ i ] );
//	//	}
//	//}

//	//[Benchmark]
//	//public void F32AddD() {
//	//	unchecked {
//	//		float sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum = Add( values32f[ i ], sum );
//	//	}
//	//}

//	//[Benchmark]
//	//public void F64AddD() {
//	//	unchecked {
//	//		double sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum = Add( values64f[ i ], sum );
//	//	}
//	//}

//	//[Benchmark]
//	//public void F32MulD() {
//	//	unchecked {
//	//		float sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum = Mul( values32f[ i ], sum );
//	//	}
//	//}

//	//[Benchmark]
//	//public void F64MulD() {
//	//	unchecked {
//	//		double sum = 0;
//	//		for ( int i = 0; i < Size; i++ )
//	//			sum = Mul( values64f[ i ], sum );
//	//	}
//	//}

//	//private float Add( float a, float b ) => a + b;
//	//private double Add( double a, double b ) => a + b;
//	//private float Mul( float a, float b ) => a * b;
//	//private double Mul( double a, double b ) => a * b;
//}
