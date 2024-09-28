using System.Numerics;

namespace Engine;

public interface IInnerProduct<TVector, TScalar>
	where TVector :
		unmanaged, IInnerProduct<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	TScalar Dot( in TVector r );
}

public interface IMagnitudeSquared<TScalar>
	 where TScalar :
		unmanaged, INumber<TScalar> {
	TScalar MagnitudeSquared();
}