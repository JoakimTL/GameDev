using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface IVectorOperations<TVector, TScalar> :
        ILinearAlgebraOperations<TVector, TScalar>
    where TVector :
        unmanaged, IVectorOperations<TVector, TScalar>
    where TScalar :
        unmanaged, INumber<TScalar>
{
    static abstract TScalar Dot(in TVector l, in TVector r);
}
