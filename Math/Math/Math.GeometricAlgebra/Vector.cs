using System.Numerics;

namespace Math.GeometricAlgebra;

public static class Vector
{
	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static TVector Negate<TVector, TScalar>( this IVector<TVector, TScalar> vector )
	//	where TVector : unmanaged, IVector<TVector, TScalar>
	//	where TScalar : unmanaged, INumber<TScalar>
	//	=> TVector.Negate( (TVector) vector );

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static TVector Add<TVector, TScalar>( this TVector l, in TVector r )
	//	where TVector : unmanaged, IVector<TVector, TScalar>
	//	where TScalar : unmanaged, INumber<TScalar>
	//	=> TVector.Add( l, r );

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static TVector Subtract<TVector, TScalar>( this TVector l, in TVector r )
	//	where TVector : unmanaged, IVector<TVector, TScalar>
	//	where TScalar : unmanaged, INumber<TScalar>
	//	=> TVector.Subtract( l, r );

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static TVector ScalarMultiply<TVector, TScalar>( this TVector l, TScalar r )
	//	where TVector : unmanaged, IVector<TVector, TScalar>
	//	where TScalar : unmanaged, INumber<TScalar>
	//	=> TVector.ScalarMultiply( l, r );

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static TVector ScalarDivide<TVector, TScalar>( this TVector l, TScalar r )
	//	where TVector : unmanaged, IVector<TVector, TScalar>
	//	where TScalar : unmanaged, INumber<TScalar>
	//	=> TVector.ScalarDivide( l, r );

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static TVector DivideScalar<TVector, TScalar>( this TScalar l, in TVector r )
	//	where TVector : unmanaged, IVector<TVector, TScalar>
	//	where TScalar : unmanaged, INumber<TScalar>
	//	=> TVector.DivideScalar( l, r );

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static TVector MultiplyEntrywise<TVector, TScalar>( this TVector l, in TVector r )
	//	where TVector : unmanaged, IVector<TVector, TScalar>, IEntrywiseOperations<TVector>
	//	where TScalar : unmanaged, INumber<TScalar>
	//	=> TVector.MultiplyEntrywise( l, r );

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static TVector DivideEntrywise<TVector, TScalar>( this TVector l, in TVector r )
	//	where TVector : unmanaged, IVector<TVector, TScalar>, IEntrywiseOperations<TVector>
	//	where TScalar : unmanaged, INumber<TScalar>
	//	=> TVector.DivideEntrywise( l, r );

	public static TVector Lerp<TVector, TScalar>(this TVector start, in TVector end, TScalar interpolationFactor)
		where TVector :
			unmanaged, ILinearAlgebraScalarOperations<TVector, TScalar>, ILinearAlgebraVectorOperations<TVector>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> start.ScalarMultiply(TScalar.One - interpolationFactor).Add(end.ScalarMultiply(interpolationFactor));

	public static TVector Slerp<TVector, TScalar>(this TVector start, in TVector end, TScalar interpolationFactor)
		where TVector :
			unmanaged, ILinearAlgebraScalarOperations<TVector, TScalar>, ILinearAlgebraVectorOperations<TVector>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> start.Lerp(end, (TScalar.CosPi(interpolationFactor) + TScalar.One) / (TScalar.One + TScalar.One));

	public static TScalar Magnitude<TVector, TScalar>(this TVector vector)
		where TVector :
			unmanaged, IMagnitudeSquared<TScalar>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> TScalar.Sqrt(vector.MagnitudeSquared());

	public static TVector Normalize<TVector, TScalar>(this TVector vector)
		where TVector :
			unmanaged, IMagnitudeSquared<TScalar>, ILinearAlgebraScalarOperations<TVector, TScalar>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> vector.ScalarDivide(vector.Magnitude<TVector, TScalar>());

	public static bool TryNormalize<TVector, TScalar>(this TVector vector, out TVector normalizedVector, out TScalar originalMagnitude)
		where TVector :
			unmanaged, IMagnitudeSquared<TScalar>, ILinearAlgebraScalarOperations<TVector, TScalar>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
	{
		originalMagnitude = vector.Magnitude<TVector, TScalar>();
		if (originalMagnitude == TScalar.Zero)
		{
			normalizedVector = default;
			return false;
		}
		normalizedVector = vector.ScalarDivide(originalMagnitude);
		return true;
	}

	public static TVector ReflectMirror<TVector, TScalar>(this TVector v, in TVector mirrorNormal)
		where TVector :
			unmanaged, ILinearAlgebraVectorOperations<TVector>, IReflectable<TVector, TScalar>
		where TScalar :
			unmanaged, INumber<TScalar>
		=> v.ReflectNormal(mirrorNormal).Negate();

	public static TVector Floor<TVector, TScalar>(this TVector l)
		where TVector :
			unmanaged, IEntrywiseOperations<TVector, TScalar>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> l.EntrywiseOperation(TScalar.Floor);

	public static TVector Ceiling<TVector, TScalar>(this TVector l)
		where TVector :
			unmanaged, IEntrywiseOperations<TVector, TScalar>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> l.EntrywiseOperation(TScalar.Ceiling);

	public static TVector Round<TVector, TScalar>(this TVector l, int digits, MidpointRounding roundingMode)
		where TVector :
			unmanaged, IEntrywiseOperations<TVector, TScalar>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> l.EntrywiseOperation((s) => TScalar.Round(s, digits, roundingMode));

	/// <summary>
	/// Find the determinant between the two vectors <paramref name="l"/> and <paramref name="r"/> from a chosen <paramref name="origin"/>.
	/// </summary>
	/// <returns>The determinant of a transposed basis matrix with the bases composed of the difference between <paramref name="l"/>/<paramref name="r"/> and <paramref name="origin"/></returns>
	// The reason for the transposed basis if to stay in-line with the geometric algebra in the calculation of the determinant.
	public static TScalar DeterminantWithOrigin<TScalar>(this Vector2<TScalar> l, in Vector2<TScalar> origin, in Vector2<TScalar> r)
		where TScalar : unmanaged, INumber<TScalar>
		=> Matrix.Create2x2.TransposedBasis(l - origin, r - origin).GetDeterminant();

	public static TScalar Orient<TScalar>(this Vector2<TScalar> l, in Vector2<TScalar> origin, in Vector2<TScalar> r)
		where TScalar : unmanaged, INumber<TScalar>
		=> l.DeterminantWithOrigin(origin, r);

	public static TScalar InCircle<TScalar>(this Vector2<TScalar> a, in Vector2<TScalar> b, in Vector2<TScalar> c, in Vector2<TScalar> origin)
		where TScalar : unmanaged, INumber<TScalar>
		=> new Matrix3x3<TScalar>(
			a.X - origin.X, a.Y - origin.Y, (a - origin).MagnitudeSquared(),
			b.X - origin.X, b.Y - origin.Y, (b - origin).MagnitudeSquared(),
			c.X - origin.X, c.Y - origin.Y, (c - origin).MagnitudeSquared())
		.GetDeterminant();

	/// <summary>
	/// Find the determinant between the two vectors <paramref name="l"/> and <paramref name="r"/> from a chosen <paramref name="origin"/>.
	/// </summary>
	/// <returns>The determinant of a transposed basis matrix with the bases composed of the difference between <paramref name="l"/>/<paramref name="r"/> and <paramref name="origin"/></returns>
	// The reason for the transposed basis if to stay in-line with the geometric algebra in the calculation of the determinant.
	public static TScalar DeterminantWithOrigin<TScalar>(this Vector3<TScalar> a, in Vector3<TScalar> origin, in Vector3<TScalar> b, in Vector3<TScalar> c)
		where TScalar : unmanaged, INumber<TScalar>
		=> Matrix.Create3x3.TransposedBasis(a - origin, b - origin, c - origin).GetDeterminant();

	public static TScalar Orient<TScalar>(this Vector3<TScalar> a, in Vector3<TScalar> origin, in Vector3<TScalar> b, in Vector3<TScalar> c)
		where TScalar : unmanaged, INumber<TScalar>
		=> a.DeterminantWithOrigin(origin, b, c);

	public static TScalar InSphere<TScalar>(this Vector3<TScalar> a, in Vector3<TScalar> b, in Vector3<TScalar> c, in Vector3<TScalar> d, in Vector3<TScalar> origin)
		where TScalar : unmanaged, INumber<TScalar>
		=> new Matrix4x4<TScalar>(
			a.X - origin.X, a.Y - origin.Y, a.Z - origin.Z, (a - origin).MagnitudeSquared(),
			b.X - origin.X, b.Y - origin.Y, b.Z - origin.Z, (b - origin).MagnitudeSquared(),
			c.X - origin.X, c.Y - origin.Y, c.Z - origin.Z, (c - origin).MagnitudeSquared(),
			d.X - origin.X, d.Y - origin.Y, d.Z - origin.Z, (d - origin).MagnitudeSquared())
		.GetDeterminant();

	/// <summary>
	/// Returns the normalized dot product between two vectors <paramref name="l"/> and <paramref name="r"/> with a defined <paramref name="origin"/>.
	/// </summary>
	/// <param name="product">The resulting dot product.</param>
	/// <returns>True if the vectors subtracted by the defined origin could be normalized.</returns>
	public static TScalar DotWithOrigin<TVector, TScalar>(this TVector l, in TVector origin, in TVector r)
		where TVector : unmanaged, ILinearAlgebraVectorOperations<TVector>, IInnerProduct<TVector, TScalar>
		where TScalar : unmanaged, INumber<TScalar>
		=> l.Subtract(origin).Dot(r.Subtract(origin));
	/// <summary>
	/// Returns the normalized dot product between two vectors <paramref name="l"/> and <paramref name="r"/> with a defined <paramref name="origin"/>.
	/// </summary>
	/// <param name="product">The resulting dot product.</param>
	/// <returns>True if the vectors subtracted by the defined origin could be normalized.</returns>
	public static bool TryNormalizedDotWithOrigin<TVector, TScalar>(this TVector l, in TVector origin, in TVector r, out TScalar product)
		where TVector : unmanaged, ILinearAlgebraVectorOperations<TVector>, IMagnitudeSquared<TScalar>, ILinearAlgebraScalarOperations<TVector, TScalar>, IInnerProduct<TVector, TScalar>
		where TScalar : unmanaged, IFloatingPointIeee754<TScalar>
	{
		product = default;
		if (!l.Subtract(origin).TryNormalize<TVector, TScalar>(out TVector a, out _) || !r.Subtract(origin).TryNormalize<TVector, TScalar>(out TVector b, out _))
			return false;
		product = a.Dot(b);
		return true;
	}

	public static TVector Average<TVector, TScalar>(this in ReadOnlySpan<TVector> vectors)
		where TVector :
			unmanaged, IVectorIdentities<TVector>, ILinearAlgebraVectorOperations<TVector>, ILinearAlgebraScalarOperations<TVector, TScalar>
		where TScalar :
			unmanaged, INumber<TScalar>
	{
		TVector sum = TVector.Zero;
		foreach (TVector vector in vectors)
			sum = sum.Add(vector);
		return sum.ScalarDivide(TScalar.CreateSaturating(vectors.Length));
	}

	public static TVector Average<TVector, TScalar>(this in Span<TVector> vectors)
		where TVector :
			unmanaged, IVectorIdentities<TVector>, ILinearAlgebraVectorOperations<TVector>, ILinearAlgebraScalarOperations<TVector, TScalar>
		where TScalar :
			unmanaged, INumber<TScalar>
		=> ((ReadOnlySpan<TVector>)vectors).Average<TVector, TScalar>();

	public static Vector3<TScalar> Cross<TScalar>(this Vector3<TScalar> l, in Vector3<TScalar> r)
		where TScalar :
			unmanaged, INumber<TScalar>
		=> l.Wedge(r) * -Trivector3<TScalar>.One;

	public static bool IsNegativeOrZero<TVector>(this TVector vector)
		where TVector :
			unmanaged, IEntrywiseComparisonOperators<TVector>, IVectorIdentities<TVector>
		=> vector <= TVector.Zero;

	public static bool IsPositiveOrZero<TVector>(this TVector vector)
		where TVector :
			unmanaged, IEntrywiseComparisonOperators<TVector>, IVectorIdentities<TVector>
		=> vector >= TVector.Zero;

	public static bool IsLowerThanZero<TVector>(this TVector vector)
		where TVector :
			unmanaged, IEntrywiseComparisonOperators<TVector>, IVectorIdentities<TVector>
		=> vector < TVector.Zero;

	public static bool IsHigherThanZero<TVector>(this TVector vector)
		where TVector :
			unmanaged, IEntrywiseComparisonOperators<TVector>, IVectorIdentities<TVector>
		=> vector > TVector.Zero;

	public static Vector2<TScalarNew> CastChecked<TOriginalScalar, TScalarNew>(in this Vector2<TOriginalScalar> vector)
		where TOriginalScalar :
			unmanaged, INumber<TOriginalScalar>
		where TScalarNew :
			unmanaged, INumber<TScalarNew>
		=> new(TScalarNew.CreateChecked(vector.X), TScalarNew.CreateChecked(vector.Y));

	public static Vector2<TScalarNew> CastSaturating<TOriginalScalar, TScalarNew>(in this Vector2<TOriginalScalar> vector)
		where TOriginalScalar :
			unmanaged, INumber<TOriginalScalar>
		where TScalarNew :
			unmanaged, INumber<TScalarNew>
		=> new(TScalarNew.CreateSaturating(vector.X), TScalarNew.CreateSaturating(vector.Y));

	public static Vector2<TScalarNew> CastTruncating<TOriginalScalar, TScalarNew>(in this Vector2<TOriginalScalar> vector)
		where TOriginalScalar :
			unmanaged, INumber<TOriginalScalar>
		where TScalarNew :
			unmanaged, INumber<TScalarNew>
		=> new(TScalarNew.CreateTruncating(vector.X), TScalarNew.CreateTruncating(vector.Y));

	public static Vector3<TScalarNew> CastChecked<TOriginalScalar, TScalarNew>(in this Vector3<TOriginalScalar> vector)
		where TOriginalScalar :
			unmanaged, INumber<TOriginalScalar>
		where TScalarNew :
			unmanaged, INumber<TScalarNew>
		=> new(TScalarNew.CreateChecked(vector.X), TScalarNew.CreateChecked(vector.Y), TScalarNew.CreateChecked(vector.Z));

	public static Vector3<TScalarNew> CastSaturating<TOriginalScalar, TScalarNew>(in this Vector3<TOriginalScalar> vector)
		where TOriginalScalar :
			unmanaged, INumber<TOriginalScalar>
		where TScalarNew :
			unmanaged, INumber<TScalarNew>
		=> new(TScalarNew.CreateSaturating(vector.X), TScalarNew.CreateSaturating(vector.Y), TScalarNew.CreateSaturating(vector.Z));

	public static Vector3<TScalarNew> CastTruncating<TOriginalScalar, TScalarNew>(in this Vector3<TOriginalScalar> vector)
		where TOriginalScalar :
			unmanaged, INumber<TOriginalScalar>
		where TScalarNew :
			unmanaged, INumber<TScalarNew>
		=> new(TScalarNew.CreateTruncating(vector.X), TScalarNew.CreateTruncating(vector.Y), TScalarNew.CreateTruncating(vector.Z));

	public static Vector4<TScalarNew> CastChecked<TOriginalScalar, TScalarNew>(in this Vector4<TOriginalScalar> vector)
		where TOriginalScalar :
			unmanaged, INumber<TOriginalScalar>
		where TScalarNew :
			unmanaged, INumber<TScalarNew>
		=> new(TScalarNew.CreateChecked(vector.X), TScalarNew.CreateChecked(vector.Y), TScalarNew.CreateChecked(vector.Z), TScalarNew.CreateChecked(vector.W));

	public static Vector4<TScalarNew> CastSaturating<TOriginalScalar, TScalarNew>(in this Vector4<TOriginalScalar> vector)
		where TOriginalScalar :
			unmanaged, INumber<TOriginalScalar>
		where TScalarNew :
			unmanaged, INumber<TScalarNew>
		=> new(TScalarNew.CreateSaturating(vector.X), TScalarNew.CreateSaturating(vector.Y), TScalarNew.CreateSaturating(vector.Z), TScalarNew.CreateSaturating(vector.W));

	public static Vector4<TScalarNew> CastTruncating<TOriginalScalar, TScalarNew>(in this Vector4<TOriginalScalar> vector)
		where TOriginalScalar :
			unmanaged, INumber<TOriginalScalar>
		where TScalarNew :
			unmanaged, INumber<TScalarNew>
		=> new(TScalarNew.CreateTruncating(vector.X), TScalarNew.CreateTruncating(vector.Y), TScalarNew.CreateTruncating(vector.Z), TScalarNew.CreateTruncating(vector.W));

}