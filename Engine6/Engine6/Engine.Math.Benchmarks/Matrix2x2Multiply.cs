using Engine.Math.Math;

namespace Engine.Math.Benchmarks;

public class Matrix4x4DeterminantBechmarks {

	private double aaa = 0;

	[GlobalSetup]
	public void Setup() {
		aaa = 0;
	}

	[Benchmark]
	public void Multi1() {
		var m1 = new Matrix2x2<double>( 2, 3, 4, 5 );
		var m2 = new Matrix2x2<double>( 6, 7, 8, 9 );
		var m = Matrix2x2Math<double>.Multiply( m1, m2 );
		aaa += aaa;
	}

}