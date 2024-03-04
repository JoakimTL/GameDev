using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Old.InternalMath;

namespace Engine.Math.Old;

public static class Matrix2x2Operations
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetDeterminant<T>(in this Matrix2x2<T> l) where T : INumberBase<T>
        => MathMatrix2x2.GetDeterminant(l);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2<T> Multiply<T>(in this Matrix2x2<T> l, in Matrix2x2<T> r) where T : INumberBase<T>
        => MathMatrix2x2.Multiply(l, r);

}
public static class Matrix3x3Operations
{

    //[MethodImpl( MethodImplOptions.AggressiveInlining )]
    //public static T GetDeterminant<T>( in this Matrix3x3<T> l ) where T : INumberBase<T>
    //	=> MathMatrix3x3.GetDeterminant( l );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetDeterminantByExpansionOfMinors<T>(in this Matrix3x3<T> l) where T : INumberBase<T>
        => MathMatrix3x3.GetDeterminantByExpansionOfMinors(l);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> Multiply<T>(in this Matrix3x3<T> l, in Matrix3x3<T> r) where T : INumberBase<T>
        => MathMatrix3x3.Multiply(l, r);

}

public static class Matrix4x4Operations
{

    //[MethodImpl( MethodImplOptions.AggressiveInlining )]
    //public static T GetDeterminant<T>( in this Matrix4x4<T> l ) where T : INumberBase<T>
    //	=> MathMatrix4x4.GetDeterminant( l );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetDeterminantByExpansionOfMinors<T>(in this Matrix4x4<T> l) where T : INumberBase<T>
        => MathMatrix4x4.GetDeterminantByExpansionOfMinors(l);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetDeterminantByGaussianElimination<T>(in this Matrix4x4<T> l, out T determinant) where T : INumberBase<T>
        => MathMatrix4x4.TryGetDeterminantByGaussianElimination(l, out determinant);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetUpperTriangular<T>(in this Matrix4x4<T> l, out Matrix4x4<T> upperTriangular, out bool negative) where T : INumberBase<T>
        => MathMatrix4x4.TryGetUpperTriangular(l, out upperTriangular, out negative);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4<T> Add<T>(in this Matrix4x4<T> l, in Matrix4x4<T> r) where T : INumberBase<T>
        => MathMatrix4x4.Add(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4<T> Subtract<T>(in this Matrix4x4<T> l, in Matrix4x4<T> r) where T : INumberBase<T>
        => MathMatrix4x4.Subtract(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4<T> Multiply<T>(in this Matrix4x4<T> l, in Matrix4x4<T> r) where T : INumberBase<T>
        => MathMatrix4x4.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4<T> Multiply<T>(in this Matrix4x4<T> l, in T r) where T : INumberBase<T>
        => MathMatrix4x4.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4<T> Divide<T>(in this Matrix4x4<T> l, in T r) where T : INumberBase<T>
        => MathMatrix4x4.Divide(l, r);

}
