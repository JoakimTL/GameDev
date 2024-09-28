namespace Engine.Math.NewFolder.Calculation.Interfaces;

public interface ILinearMath<T, TScalar> where T : unmanaged where TScalar : unmanaged
{
    static abstract T Negate(in T l);
    static abstract T Add(in T l, in T r);
    static abstract T Subtract(in T l, in T r);
    static abstract T Multiply(in T l, TScalar r);
    static abstract T Divide(in T l, TScalar r);
}
