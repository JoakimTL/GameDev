using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

public static class VectorExtensions {
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

	public static TScalar MagnitudeSquared<TVector, TScalar>( this TVector vector )
		where TVector : unmanaged, IVector<TVector, TScalar>
		where TScalar : unmanaged, INumber<TScalar>
		=> vector.Dot( vector );

	public static TScalar Magnitude<TVector, TScalar>( this TVector vector )
		where TVector : unmanaged, IVector<TVector, TScalar>
		where TScalar : unmanaged, IFloatingPointIeee754<TScalar>
		=> TScalar.Sqrt( vector.MagnitudeSquared<TVector, TScalar>() );

	public static TVector Normalize<TVector, TScalar>( this TVector vector )
		where TVector : unmanaged, IVector<TVector, TScalar>
		where TScalar : unmanaged, IFloatingPointIeee754<TScalar>
		=> vector.ScalarDivide( vector.Magnitude<TVector, TScalar>() );

	public static bool TryNormalize<TVector, TScalar>( this TVector vector, out TVector normalizedVector, out TScalar originalMagnitude )
		where TVector : unmanaged, IVector<TVector, TScalar>
		where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		originalMagnitude = vector.Magnitude<TVector, TScalar>();
		if (originalMagnitude == TScalar.Zero) {
			normalizedVector = default;
			return false;
		}
		normalizedVector = vector.ScalarDivide( originalMagnitude );
		return true;
	}

	public static TVector ReflectMirror<TVector, TScalar>( this TVector v, in TVector mirrorNormal )
		where TVector :
			unmanaged, IVector<TVector, TScalar>, IReflectable<TVector, TScalar>
		where TScalar :
			unmanaged, INumber<TScalar>
		=> v.ReflectNormal( mirrorNormal ).Negate();

	public static TVector Floor<TVector, TScalar>( this TVector l )
		where TVector :
			unmanaged, IEntrywiseOperations<TVector, TScalar>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> l.EntrywiseOperation( TScalar.Floor );

	public static TVector Ceiling<TVector, TScalar>( this TVector l )
		where TVector :
			unmanaged, IEntrywiseOperations<TVector, TScalar>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> l.EntrywiseOperation( TScalar.Ceiling );

	public static TVector Round<TVector, TScalar>( this TVector l, int digits, MidpointRounding roundingMode )
		where TVector :
			unmanaged, IEntrywiseOperations<TVector, TScalar>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> l.EntrywiseOperation( ( TScalar s ) => TScalar.Round( s, digits, roundingMode ) );
}