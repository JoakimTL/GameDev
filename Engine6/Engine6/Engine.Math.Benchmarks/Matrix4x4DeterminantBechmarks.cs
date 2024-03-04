//namespace Engine.Math.Benchmarks;

//public class Matrix4x4DeterminantBechmarks {

//	private double aaa = 0;

//	[GlobalSetup]
//	public void Setup() {
//		aaa = 0;
//	}

//	[Benchmark]
//	public void Matrix4x4Determinant() {
//		var m = new Matrix4x4<double>(
//			1, 2, 3, 4,
//			5, 6, 7, 8,
//			9, 10, 11, 12,
//			13, 14, 15, 16
//		);
//		aaa += m.Determinant;
//	}

//	[Benchmark]
//	public void Matrix4x4DeterminantExpansionOfMinors() {
//		var m = new Matrix4x4<double>(
//			1, 2, 3, 4,
//			5, 6, 7, 8,
//			9, 10, 11, 12,
//			13, 14, 15, 16
//		);
//		aaa += m.GetDeterminantByExpansionOfMinors();
//	}

//	[Benchmark]
//	public void Matrix4x4DeterminantGaussianElimination() {
//		var m = new Matrix4x4<double>(
//			1, 2, 3, 4,
//			5, 6, 7, 8,
//			9, 10, 11, 12,
//			13, 14, 15, 16
//		);
//		if (m.TryGetDeterminantByGaussianElimination( out double det ))
//			aaa += det;
//	}

//}