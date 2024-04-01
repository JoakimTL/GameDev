using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface ILinearAlgebraOperators<TVector, TScalar>
    where TVector :
        unmanaged, ILinearAlgebraOperators<TVector, TScalar>, ILinearAlgebraOperations<TVector, TScalar>
    where TScalar :
        unmanaged, INumber<TScalar>
{
    static abstract TVector operator -(in TVector l);
    static abstract TVector operator +(in TVector l, in TVector r);
    static abstract TVector operator -(in TVector l, in TVector r);
    static abstract TVector operator *(in TVector l, TScalar r);
    static abstract TVector operator *(TScalar l, in TVector r);
    static abstract TVector operator /(in TVector l, TScalar r);
    static abstract TVector operator /(TScalar l, in TVector r);
}
