using System.Numerics;

namespace Engine.Math.NewFolder.Operations.Interfaces;

public interface ILinearAlgebraOperations<T, TScalar> where T : unmanaged where TScalar : unmanaged, INumber<TScalar>
{
    static abstract T Negate(in T l);
    static abstract T Add(in T l, in T r);
    static abstract T Subtract(in T l, in T r);
    static abstract T ScalarMultiply(in T l, TScalar r);
    static abstract T ScalarDivide(in T l, TScalar r);
}
