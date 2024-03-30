using System.Numerics;

namespace Engine.Math.Interfaces;

public interface ILinearOperators<TSelf, T> where TSelf : unmanaged, ILinearOperators<TSelf, T> where T : unmanaged, INumber<T>
{
    static abstract TSelf operator -(in TSelf l);
    static abstract TSelf operator -(in TSelf l, in TSelf r);
    static abstract TSelf operator +(in TSelf l, in TSelf r);
    static abstract TSelf operator *(in TSelf l, T r);
    static abstract TSelf operator *(T l, in TSelf r);
    static abstract TSelf operator /(in TSelf l, T r);
}
