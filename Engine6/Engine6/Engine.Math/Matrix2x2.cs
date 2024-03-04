using System.Numerics;

namespace Engine.Math;

public readonly struct Matrix2x2<T>(T m00, T m01, T m10, T m11) where T : unmanaged, INumberBase<T> {
	public readonly T M00 = m00;
	public readonly T M01 = m01;
	public readonly T M10 = m10;
	public readonly T M11 = m11;

	public Vector2<T> Row0 => new(M00, M01);
	public Vector2<T> Row1 => new(M10, M11);
	public Vector2<T> Col0 => new(M00, M10);
	public Vector2<T> Col1 => new(M01, M11);

	public static Matrix2x2<T> Identity => new(
		T.One, T.Zero, 
		T.Zero, T.One
	);

	public override string ToString() => $"[{Row0}]/[{Row1}]";
}
