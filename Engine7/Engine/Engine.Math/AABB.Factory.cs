namespace Engine;

public static partial class AABB {
	public static AABB<TVector> Create<TVector>( Span<TVector> vectors )
		where TVector :
			unmanaged, IInEqualityOperators<TVector, TVector, bool>, IEntrywiseMinMaxOperations<TVector> {
		TVector min = vectors[ 0 ];
		TVector max = vectors[ 0 ];
		for (int i = 1; i < vectors.Length; i++) {
			min = min.Min( vectors[ i ] );
			max = max.Max( vectors[ i ] );
		}
		return new AABB<TVector>( min, max );
	}

	public static AABB<TVector> Create<TVector>( Span<AABB<TVector>> bounds )
		where TVector :
			unmanaged, IInEqualityOperators<TVector, TVector, bool>, IEntrywiseMinMaxOperations<TVector> {
		TVector min = bounds[ 0 ].Minima;
		TVector max = bounds[ 0 ].Maxima;
		for (int i = 1; i < bounds.Length; i++) {
			min = min.Min( bounds[ i ].Minima );
			max = max.Max( bounds[ i ].Maxima );
		}
		return new AABB<TVector>( min, max );
	}
}