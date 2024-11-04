namespace Engine;

public interface IEntrywiseComparisonOperators<TVector>
	where TVector :
		IEntrywiseComparisonOperators<TVector> {
	/// <returns>True if all entries in <c>left</c> are less than or equal to the corresponding entries in <c>right</c>.</returns>
	static abstract bool operator <( in TVector left, in TVector right );
	/// <returns>True if all entries in <c>left</c> are less than or equal to the corresponding entries in <c>right</c>.</returns>
	static abstract bool operator >( in TVector left, in TVector right );
	/// <returns>True if all entries in <c>left</c> are less than or equal to the corresponding entries in <c>right</c>.</returns>
	static abstract bool operator <=( in TVector left, in TVector right );
	/// <returns>True if all entries in <c>left</c> are less than or equal to the corresponding entries in <c>right</c>.</returns>
	static abstract bool operator >=( in TVector left, in TVector right );
}