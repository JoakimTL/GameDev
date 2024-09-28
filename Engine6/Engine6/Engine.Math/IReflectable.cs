using System.Numerics;

namespace Engine;

public interface IReflectable<TVector, TScalar> :
        IVector<TVector, TScalar>
    where TVector :
        unmanaged, IVector<TVector, TScalar>
    where TScalar :
        unmanaged, INumber<TScalar>
{
    TVector ReflectNormal(in TVector normal);
}
