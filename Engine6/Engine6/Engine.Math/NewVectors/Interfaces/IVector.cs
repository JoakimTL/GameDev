using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface IVector<TVector, TScalar> :
        IVectorOperations<TVector, TScalar>,
        IAdditiveIdentity<TVector, TVector>,
        IMultiplicativeIdentity<TVector, TVector>,
        IInEqualityOperators<TVector, TVector, bool>
    where TVector :
        unmanaged, IVector<TVector, TScalar>
    where TScalar :
        unmanaged, INumber<TScalar>
{
    static abstract TVector One { get; }
    static abstract TVector Zero { get; }
}

public interface IReflectable<TVector, TScalar> :
		IVector<TVector, TScalar>
	where TVector :
		unmanaged, IVector<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	static abstract TVector ReflectNormal( in TVector v, in TVector normal );
	static abstract TVector ReflectMirror( in TVector v, in TVector mirrorNormal );
}

public interface IRotor<TRotor, TVector, TScalar> :
		IVector<TRotor, TScalar>
	where TRotor :
		unmanaged, IRotor<TRotor, TVector, TScalar>
	where TVector :
		unmanaged, IVector<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
    static abstract TVector Rotate( in TRotor r, in TVector v);
}