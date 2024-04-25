using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface IMatrix<TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	TScalar GetDeterminant();
	uint Rows { get; }
	uint Columns { get; }
	TScalar this[ uint row, uint column ] { get; }
}
