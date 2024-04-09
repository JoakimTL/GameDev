using Engine.Math.NewFolder.Calculation.Interfaces;
using Engine.Math.NewFolder.Old.InternalMath;
using Engine.Math.NewFolder.Operations;
using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface IVector<TVector, TScalar> :
		ILinearAlgebraOperations<TVector, TScalar>,
		IInnerProduct<TVector, TScalar>,
		IAdditiveIdentity<TVector, TVector>,
		IMultiplicativeIdentity<TVector, TVector>,
		IInEqualityOperators<TVector, TVector, bool>
	where TVector :
		unmanaged, IVector<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	static abstract TVector One { get; }
	static abstract TVector Zero { get; }
}

public interface IMatrix<TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	TScalar GetDeterminant();
	uint Rows { get; }
	uint Columns { get; }
	TScalar this[ uint row, uint column ] { get; }
}

public interface ISquareMatrix<TMatrix>
	where TMatrix :
		unmanaged, ISquareMatrix<TMatrix> {
	TMatrix GetTransposed();
	/// <summary>
	/// Does not work that well for integers compared to floating point numbers
	/// </summary>
	/// <returns>False if the matrix can't be inverted</returns>
	bool TryGetInverse( out TMatrix matrix );
	/// <returns>False if triangulation failed</returns>
	bool TryGetUpperTriangular( out TMatrix upperTriangular, out bool negative );
	/// <returns>False if triangulation failed</returns>
	bool TryGetLowerTriangular( out TMatrix lowerTriangular, out bool negative );
}
