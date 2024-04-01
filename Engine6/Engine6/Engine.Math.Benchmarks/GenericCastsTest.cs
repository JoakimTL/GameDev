using Engine.Math.NewVectors;

namespace Engine.Math.Benchmarks;

public class GenericCastsTest {

	public double a;

	[GlobalSetup]
	public void Setup() {
		a = 0;
	}

	//Casting is real slow here
	//[Benchmark]
	//public void IVector() {
	//	Vector2<double> vec = new( 1, a );
	//	a = vec.MagnitudeSquared();
	//}

	//[Benchmark]
	//public void IVectorOneCast() {
	//	Vector2<double> vec = new( 1, a );
	//	a = vec.MagnitudeSquaredOneCast();
	//}

	//[Benchmark]
	//public void Vector() {
	//	Vector2<double> vec = new( 1, a );
	//	a = vec.MagnitudeSquared<Vector2<double>, double>();
	//}
}


public class Vector3RoundingFuncVsRaw {

	public Vector3<double> a;

	[GlobalSetup]
	public void Setup() {
		a = new();
	}

	[Benchmark]
	public void Func() {
		Vector3<double> vec = new( a.Y + 1, a.Z + 1.5, a.X * 0.77);
		a = vec.Round<Vector3<double>, double>(2, MidpointRounding.ToEven);
	}

	//[Benchmark]
	//public void Raw() {
	//	Vector3<double> vec = new( a.Y + 1, a.Z + 1.5, a.X * 0.77 );
	//	a = vec.RoundRaw( 2, MidpointRounding.ToEven );
	//}
}