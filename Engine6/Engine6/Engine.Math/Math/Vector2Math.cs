using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Math;

/// <summary>
/// All methods that return <see cref="Vector2{T}"/> are implemented here."/>
/// </summary>
public sealed class Vector2Math<T> :
        ILinearMath<Vector2<T>, T>,
        IEntrywiseProduct<Vector2<T>>,
        IGeometricProduct<Vector2<T>, Bivector2<T>, Vector2<T>>,
        IGeometricProduct<Bivector2<T>, Vector2<T>, Vector2<T>>,
        IMatrixMultiplicationProduct<Vector2<T>, Matrix2x2<T>, Vector2<T>>
    where T :
        unmanaged, INumberBase<T> {

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Vector2<T> Negate( in Vector2<T> l ) => new( -l.X, -l.Y );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Vector2<T> Add( in Vector2<T> l, in Vector2<T> r ) => new( l.X + r.X, l.Y + r.Y );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Vector2<T> Subtract( in Vector2<T> l, in Vector2<T> r ) => new( l.X - r.X, l.Y - r.Y );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Vector2<T> Multiply( in Vector2<T> l, T r ) => new( l.X * r, l.Y * r );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Vector2<T> Divide( in Vector2<T> l, T r ) => new( l.X / r, l.Y / r );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Vector2<T> MultiplyEntrywise( in Vector2<T> l, in Vector2<T> r ) => new( l.X * r.X, l.Y * r.Y );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Vector2<T> DivideEntrywise( in Vector2<T> l, in Vector2<T> r ) => new( l.X / r.X, l.Y / r.Y );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Vector2<T> Multiply( in Vector2<T> l, in Bivector2<T> r ) => new( -l.Y * r.XY, l.X * r.XY );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Vector2<T> Multiply( in Bivector2<T> l, in Vector2<T> r ) => new( r.Y * l.XY, -r.X * l.XY );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Vector2<T> Multiply( in Vector2<T> l, in Matrix2x2<T> r ) => new( l.Dot( r.Col0 ), l.Dot( r.Col1 ) );
}

