namespace Engine.Math.Benchmarks;

//public class Matrix4x4DeterminantBechmarks {

//	private double aaa = 0;

//	[GlobalSetup]
//	public void Setup() {
//		aaa = 0;
//	}

//	[Benchmark]
//	public void Multi1() {
//		var m1 = new Matrix2x2<double>( 2, 3, 4, 5 );
//		var m2 = new Matrix2x2<double>( 6, 7, 8, 9 );
//		var m = Matrix2x2Math<double>.Multiply( m1, m2 );
//		aaa += aaa;
//	}

//}
//public class Rotor3FactoryBenchmarks {

//	public Rotor3<double> a;

//	[Benchmark]
//	public void FromVector1() {
//		Vector3<double> from = new( 1, 0, 0 );
//		Vector3<double> to = new( 0, 1, 0 );
//		a = Rotor3Factory.FromVectors(from, to);
//	}

//	[Benchmark]
//	public void FromVector2() {
//		Vector3<double> from = new( 1, 0, 0 );
//		Vector3<double> to = new( 0, 1, 0 );
//		a = Rotor3Factory.FromVectors(from, to);
//	}

//}
