using System.Numerics;

namespace Engine.Math.NewFolder.Operations.Interfaces;

public interface IMatrixOperations<T, TScalar> where T : unmanaged where TScalar : unmanaged, INumber<TScalar>
{
    static abstract TScalar GetDeterminant(in T m);
    static abstract T GetTransposed(in T m);
    static abstract T GetInverse(in T m);
}
