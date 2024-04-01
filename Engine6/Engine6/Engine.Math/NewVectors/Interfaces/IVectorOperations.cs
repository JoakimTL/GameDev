using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface IInnerProduct<TVector, TScalar>
	where TVector :
		unmanaged, IInnerProduct<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	TScalar Dot( in TVector r );
}

public interface IOuterProduct<TVector, TResult>
	where TVector :
		unmanaged, IOuterProduct<TVector, TResult>
	where TResult :
		unmanaged {
	TResult Wedge( in TVector r );
}