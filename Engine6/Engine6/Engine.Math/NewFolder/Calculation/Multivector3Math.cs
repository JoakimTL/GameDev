using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Calculation.Interfaces;

namespace Engine.Math.NewFolder.Calculation;

/// <summary>
/// All methods that return <see cref="Multivector3{T}"/> are implemented here."/>
/// </summary>
public sealed class Multivector3Math<T> :
        ILinearMath<Multivector3<T>, T>,
        IEntrywiseProduct<Multivector3<T>>,
        IGeometricProduct<Vector3<T>, Bivector3<T>, Multivector3<T>>,           // 1*2
        IGeometricProduct<Vector3<T>, Rotor3<T>, Multivector3<T>>,              // 1*4
        IGeometricProduct<Vector3<T>, Multivector3<T>, Multivector3<T>>,        // 1*5
        IGeometricProduct<Bivector3<T>, Vector3<T>, Multivector3<T>>,           // 2*1
        IGeometricProduct<Bivector3<T>, Multivector3<T>, Multivector3<T>>,      // 2*5
        IGeometricProduct<Trivector3<T>, Rotor3<T>, Multivector3<T>>,           // 3*4
        IGeometricProduct<Trivector3<T>, Multivector3<T>, Multivector3<T>>,     // 3*5
        IGeometricProduct<Rotor3<T>, Vector3<T>, Multivector3<T>>,              // 4*1
        IGeometricProduct<Rotor3<T>, Trivector3<T>, Multivector3<T>>,           // 4*3
        IGeometricProduct<Rotor3<T>, Multivector3<T>, Multivector3<T>>,         // 4*5
        IGeometricProduct<Multivector3<T>, Vector3<T>, Multivector3<T>>,        // 5*1
        IGeometricProduct<Multivector3<T>, Bivector3<T>, Multivector3<T>>,      // 5*2
        IGeometricProduct<Multivector3<T>, Trivector3<T>, Multivector3<T>>,     // 5*3
        IGeometricProduct<Multivector3<T>, Rotor3<T>, Multivector3<T>>,         // 5*4
        IGeometricProduct<Multivector3<T>, Multivector3<T>, Multivector3<T>>    // 5*5
    where T :
        unmanaged, INumber<T>
{


    #region Linear Algebra

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Negate(in Multivector3<T> l)
        => new(
            -l.Scalar,
            -l.Vector,
            -l.Bivector,
            -l.Trivector
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Add(in Multivector3<T> l, in Multivector3<T> r)
        => new(
            l.Scalar + r.Scalar,
            l.Vector + r.Vector,
            l.Bivector + r.Bivector,
            l.Trivector + r.Trivector
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Subtract(in Multivector3<T> l, in Multivector3<T> r)
        => new(
            l.Scalar - r.Scalar,
            l.Vector - r.Vector,
            l.Bivector - r.Bivector,
            l.Trivector - r.Trivector
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Multivector3<T> l, T r)
        => new(
            l.Scalar * r,
            l.Vector * r,
            l.Bivector * r,
            l.Trivector * r
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Divide(in Multivector3<T> l, T r)
        => new(
            l.Scalar / r,
            l.Vector / r,
            l.Bivector / r,
            l.Trivector / r
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> MultiplyEntrywise(in Multivector3<T> l, in Multivector3<T> r)
        => new(
            l.Scalar * r.Scalar,
            l.Vector * r.Vector,
            l.Bivector * r.Bivector,
            l.Trivector * r.Trivector
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> DivideEntrywise(in Multivector3<T> l, in Multivector3<T> r)
        => new(
            l.Scalar / r.Scalar,
            l.Vector / r.Vector,
            l.Bivector / r.Bivector,
            l.Trivector / r.Trivector
        );

    #endregion

    #region Geometric Products

    /*
	 * 0 - scalar
	 * 1 - vec3
	 * 2 - bivec3
	 * 3 - trivec3
	 * 4 - rotor
	 * 5 - multivec3
	 * 
	 ---------------------* 0 * 1, // returns 1
	 ---------------------* 0 * 2, // returns 2
	 ---------------------* 0 * 3, // returns 3
	 ---------------------* 0 * 4, // returns 4
	 * 0 * 5, // returns 5
	 ---------------------* 1 * 1, // returns 4
	 * 1 * 2, // returns 5
	 ---------------------* 1 * 3, // returns 2
	 * 1 * 4, // returns 5
	 * 1 * 5, // returns 5
	 * 2 * 1, // returns 5
	 ---------------------* 2 * 2, // returns 4
	 ---------------------* 2 * 3, // returns 1
	 ---------------------* 2 * 4, // returns 4
	 * 2 * 5, // returns 5
	 ---------------------* 3 * 1, // returns 2
	 ---------------------* 3 * 2, // returns 1
	 ---------------------* 3 * 3, // returns 0
	 * 3 * 4, // returns 5
	 * 3 * 5, // returns 5
	 * 4 * 1, // returns 5
	 ---------------------* 4 * 2, // returns 4
	 * 4 * 3, // returns 5
	 ---------------------* 4 * 4, // returns 4
	 * 4 * 5, // returns 5
	 * 5 * 1, // returns 5
	 * 5 * 2, // returns 5
	 * 5 * 3, // returns 5
	 * 5 * 4, // returns 5
	 * 5 * 5, // returns 5
	 */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Vector3<T> l, in Bivector3<T> r)
        => new(
            T.AdditiveIdentity,
            (l.Z * r.ZX) - (l.Y * r.XY),
            (l.X * r.XY) - (l.Z * r.YZ),
            (l.Y * r.YZ) - (l.X * r.ZX),
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            (l.X * r.YZ) + (l.Y * r.ZX) + (l.Z * r.XY)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Vector3<T> l, in Rotor3<T> r)
        => new(
            T.AdditiveIdentity,
            (l.Z * r.Bivector.ZX) + (l.X * r.Scalar) - (l.Y * r.Bivector.XY),
            (l.X * r.Bivector.XY) + (l.Y * r.Scalar) - (l.Z * r.Bivector.YZ),
            (l.Y * r.Bivector.YZ) + (l.Z * r.Scalar) - (l.X * r.Bivector.ZX),
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            (l.X * r.Bivector.YZ) + (l.Y * r.Bivector.ZX) + (l.Z * r.Bivector.XY)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Vector3<T> l, in Multivector3<T> r)
        => new(
            (l.X * r.Vector.X) + (l.Y * r.Vector.Y) + (l.Z * r.Vector.Z),
            (l.Z * r.Bivector.ZX) + (l.X * r.Scalar) - (l.Y * r.Bivector.XY),
            (l.X * r.Bivector.XY) + (l.Y * r.Scalar) - (l.Z * r.Bivector.YZ),
            (l.Y * r.Bivector.YZ) + (l.Z * r.Scalar) - (l.X * r.Bivector.ZX),
            (l.Y * r.Vector.Z) + (l.X * r.Trivector.XYZ) - (l.Z * r.Vector.Y),
            (l.Z * r.Vector.X) + (l.Y * r.Trivector.XYZ) - (l.X * r.Vector.Z),
            (l.X * r.Vector.Y) + (l.Z * r.Trivector.XYZ) - (l.Y * r.Vector.X),
            (l.X * r.Bivector.YZ) + (l.Y * r.Bivector.ZX) + (l.Z * r.Bivector.XY)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Bivector3<T> l, in Vector3<T> r)
        => new(
            T.AdditiveIdentity,
            (l.XY * r.Y) - (l.ZX * r.Z),
            (l.YZ * r.Z) - (l.XY * r.X),
            (l.ZX * r.X) - (l.YZ * r.Y),
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            (l.XY * r.Z) + (l.YZ * r.X) + (l.ZX * r.Y)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Bivector3<T> l, in Multivector3<T> r)
        => new(
            (-l.XY * r.Bivector.XY) - (l.YZ * r.Bivector.YZ) - (l.ZX * r.Bivector.ZX),
            (l.XY * r.Vector.Y) - (l.YZ * r.Trivector.XYZ) - (l.ZX * r.Vector.Z),
            (l.YZ * r.Vector.Z) - (l.ZX * r.Trivector.XYZ) - (l.XY * r.Vector.X),
            (l.ZX * r.Vector.X) - (l.XY * r.Trivector.XYZ) - (l.YZ * r.Vector.Y),
            (l.XY * r.Bivector.ZX) + (l.YZ * r.Scalar) - (l.ZX * r.Bivector.XY),
            (l.YZ * r.Bivector.XY) + (l.ZX * r.Scalar) - (l.XY * r.Bivector.YZ),
            (l.ZX * r.Bivector.YZ) + (l.XY * r.Scalar) - (l.YZ * r.Bivector.ZX),
            (l.XY * r.Vector.Z) + (l.YZ * r.Vector.X) + (l.ZX * r.Vector.Y)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Trivector3<T> l, in Rotor3<T> r)
        => new(
            T.AdditiveIdentity,
            -(l.XYZ * r.Bivector.YZ),
            -(l.XYZ * r.Bivector.ZX),
            -(l.XYZ * r.Bivector.XY),
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            l.XYZ * r.Scalar
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Trivector3<T> l, in Multivector3<T> r)
        => new(
            -l.XYZ * r.Trivector.XYZ,
            -l.XYZ * r.Bivector.YZ,
            -l.XYZ * r.Bivector.ZX,
            -l.XYZ * r.Bivector.XY,
            l.XYZ * r.Vector.X,
            l.XYZ * r.Vector.Y,
            l.XYZ * r.Vector.Z,
            l.XYZ * r.Scalar
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Rotor3<T> l, in Vector3<T> r)
        => new(
            T.AdditiveIdentity,
            (l.Scalar * r.X) + (l.Bivector.XY * r.Y) - (l.Bivector.ZX * r.Z),
            (l.Scalar * r.Y) + (l.Bivector.YZ * r.Z) - (l.Bivector.XY * r.X),
            (l.Scalar * r.Z) + (l.Bivector.ZX * r.X) - (l.Bivector.YZ * r.Y),
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            (l.Bivector.XY * r.Z) + (l.Bivector.YZ * r.X) + (l.Bivector.ZX * r.Y)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Rotor3<T> l, in Trivector3<T> r)
        => new(
            T.AdditiveIdentity,
            -(l.Bivector.YZ * r.XYZ),
            -(l.Bivector.ZX * r.XYZ),
            -(l.Bivector.XY * r.XYZ),
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            T.AdditiveIdentity,
            l.Scalar * r.XYZ
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Rotor3<T> l, in Multivector3<T> r)
        => new(
            (l.Scalar * r.Scalar) - (l.Bivector.XY * r.Bivector.XY) - (l.Bivector.YZ * r.Bivector.YZ) - (l.Bivector.ZX * r.Bivector.ZX),
            (l.Scalar * r.Vector.X) + (l.Bivector.XY * r.Vector.Y) - (l.Bivector.YZ * r.Trivector.XYZ) - (l.Bivector.ZX * r.Vector.Z),
            (l.Scalar * r.Vector.Y) + (l.Bivector.YZ * r.Vector.Z) - (l.Bivector.ZX * r.Trivector.XYZ) - (l.Bivector.XY * r.Vector.X),
            (l.Scalar * r.Vector.Z) + (l.Bivector.ZX * r.Vector.X) - (l.Bivector.XY * r.Trivector.XYZ) - (l.Bivector.YZ * r.Vector.Y),
            (l.Scalar * r.Bivector.YZ) + (l.Bivector.XY * r.Bivector.ZX) + (l.Bivector.YZ * r.Scalar) - (l.Bivector.ZX * r.Bivector.XY),
            (l.Scalar * r.Bivector.ZX) + (l.Bivector.YZ * r.Bivector.XY) + (l.Bivector.ZX * r.Scalar) - (l.Bivector.XY * r.Bivector.YZ),
            (l.Scalar * r.Bivector.XY) + (l.Bivector.ZX * r.Bivector.YZ) + (l.Bivector.XY * r.Scalar) - (l.Bivector.YZ * r.Bivector.ZX),
            (l.Scalar * r.Trivector.XYZ) + (l.Bivector.XY * r.Vector.Z) + (l.Bivector.YZ * r.Vector.X) + (l.Bivector.ZX * r.Vector.Y)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Multivector3<T> l, in Vector3<T> r)
        => new(
            (l.Vector.X * r.X) + (l.Vector.Y * r.Y) + (l.Vector.Z * r.Z),
            (l.Scalar * r.X) + (l.Bivector.XY * r.Y) - (l.Bivector.ZX * r.Z),
            (l.Scalar * r.Y) + (l.Bivector.YZ * r.Z) - (l.Bivector.XY * r.X),
            (l.Scalar * r.Z) + (l.Bivector.ZX * r.X) - (l.Bivector.YZ * r.Y),
            (l.Vector.Y * r.Z) + (l.Trivector.XYZ * r.X) - (l.Vector.Z * r.Y),
            (l.Vector.Z * r.X) + (l.Trivector.XYZ * r.Y) - (l.Vector.X * r.Z),
            (l.Vector.X * r.Y) + (l.Trivector.XYZ * r.Z) - (l.Vector.Y * r.X),
            (l.Bivector.XY * r.Z) + (l.Bivector.YZ * r.X) + (l.Bivector.ZX * r.Y)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Multivector3<T> l, in Bivector3<T> r)
        => new(
            (-l.Bivector.XY * r.XY) - (l.Bivector.YZ * r.YZ) - (l.Bivector.ZX * r.ZX),
            (l.Vector.Z * r.ZX) - (l.Vector.Y * r.XY) - (l.Trivector.XYZ * r.YZ),
            (l.Vector.X * r.XY) - (l.Vector.Z * r.YZ) - (l.Trivector.XYZ * r.ZX),
            (l.Vector.Y * r.YZ) - (l.Vector.X * r.ZX) - (l.Trivector.XYZ * r.XY),
            (l.Scalar * r.YZ) + (l.Bivector.XY * r.ZX) - (l.Bivector.ZX * r.XY),
            (l.Scalar * r.ZX) + (l.Bivector.YZ * r.XY) - (l.Bivector.XY * r.YZ),
            (l.Scalar * r.XY) + (l.Bivector.ZX * r.YZ) - (l.Bivector.YZ * r.ZX),
            (l.Vector.X * r.YZ) + (l.Vector.Y * r.ZX) + (l.Vector.Z * r.XY) + l.Bivector.XY
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Multivector3<T> l, in Trivector3<T> r)
        => new(
            -l.Trivector.XYZ * r.XYZ,
            -l.Bivector.YZ * r.XYZ,
            -l.Bivector.ZX * r.XYZ,
            -l.Bivector.XY * r.XYZ,
            l.Vector.X * r.XYZ,
            l.Vector.Y * r.XYZ,
            l.Vector.Z * r.XYZ,
            l.Scalar * r.XYZ
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Multivector3<T> l, in Rotor3<T> r)
        => new(
            (l.Scalar * r.Scalar) - (l.Bivector.XY * r.Bivector.XY) - (l.Bivector.YZ * r.Bivector.YZ) - (l.Bivector.ZX * r.Bivector.ZX),
            (l.Vector.Z * r.Bivector.ZX) + (l.Vector.X * r.Scalar) - (l.Vector.Y * r.Bivector.XY) - (l.Trivector.XYZ * r.Bivector.YZ),
            (l.Vector.X * r.Bivector.XY) + (l.Vector.Y * r.Scalar) - (l.Vector.Z * r.Bivector.YZ) - (l.Trivector.XYZ * r.Bivector.ZX),
            (l.Vector.Y * r.Bivector.YZ) + (l.Vector.Z * r.Scalar) - (l.Vector.X * r.Bivector.ZX) - (l.Trivector.XYZ * r.Bivector.XY),
            (l.Scalar * r.Bivector.YZ) + (l.Bivector.XY * r.Bivector.ZX) + (l.Bivector.YZ * r.Scalar) - (l.Bivector.ZX * r.Bivector.XY),
            (l.Scalar * r.Bivector.ZX) + (l.Bivector.YZ * r.Bivector.XY) + (l.Bivector.ZX * r.Scalar) - (l.Bivector.XY * r.Bivector.YZ),
            (l.Scalar * r.Bivector.XY) + (l.Bivector.ZX * r.Bivector.YZ) + (l.Bivector.XY * r.Scalar) - (l.Bivector.YZ * r.Bivector.ZX),
            (l.Vector.X * r.Bivector.YZ) + (l.Vector.Y * r.Bivector.ZX) + (l.Vector.Z * r.Bivector.XY) + (l.Trivector.XYZ * r.Scalar)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply(in Multivector3<T> l, in Multivector3<T> r)
        => new(
            (l.Scalar * r.Scalar) + (l.Vector.X * r.Vector.X) + (l.Vector.Y * r.Vector.Y) + (l.Vector.Z * r.Vector.Z) - (l.Bivector.XY * r.Bivector.XY) - (l.Bivector.YZ * r.Bivector.YZ) - (l.Bivector.ZX * r.Bivector.ZX) - (l.Trivector.XYZ * r.Trivector.XYZ),
            (l.Scalar * r.Vector.X) + (l.Vector.Z * r.Bivector.ZX) + (l.Vector.X * r.Scalar) + (l.Bivector.XY * r.Vector.Y) - (l.Vector.Y * r.Bivector.XY) - (l.Bivector.YZ * r.Trivector.XYZ) - (l.Bivector.ZX * r.Vector.Z) - (l.Trivector.XYZ * r.Bivector.YZ),
            (l.Scalar * r.Vector.Y) + (l.Vector.X * r.Bivector.XY) + (l.Vector.Y * r.Scalar) + (l.Bivector.YZ * r.Vector.Z) - (l.Vector.Z * r.Bivector.YZ) - (l.Bivector.ZX * r.Trivector.XYZ) - (l.Bivector.XY * r.Vector.X) - (l.Trivector.XYZ * r.Bivector.ZX),
            (l.Scalar * r.Vector.Z) + (l.Vector.Y * r.Bivector.YZ) + (l.Vector.Z * r.Scalar) + (l.Bivector.ZX * r.Vector.X) - (l.Vector.X * r.Bivector.ZX) - (l.Bivector.XY * r.Trivector.XYZ) - (l.Bivector.YZ * r.Vector.Y) - (l.Trivector.XYZ * r.Bivector.XY),
            (l.Scalar * r.Bivector.YZ) + (l.Vector.Y * r.Vector.Z) + (l.Vector.X * r.Trivector.XYZ) + (l.Bivector.XY * r.Bivector.ZX) + (l.Bivector.YZ * r.Scalar) + (l.Trivector.XYZ * r.Vector.X) - (l.Vector.Z * r.Vector.Y) - (l.Bivector.ZX * r.Bivector.XY),
            (l.Scalar * r.Bivector.ZX) + (l.Vector.Z * r.Vector.X) + (l.Vector.Y * r.Trivector.XYZ) + (l.Bivector.YZ * r.Bivector.XY) + (l.Bivector.ZX * r.Scalar) + (l.Trivector.XYZ * r.Vector.Y) - (l.Vector.X * r.Vector.Z) - (l.Bivector.XY * r.Bivector.YZ),
            (l.Scalar * r.Bivector.XY) + (l.Vector.X * r.Vector.Y) + (l.Vector.Z * r.Trivector.XYZ) + (l.Bivector.ZX * r.Bivector.YZ) + (l.Bivector.XY * r.Scalar) + (l.Trivector.XYZ * r.Vector.Z) - (l.Vector.Y * r.Vector.X) - (l.Bivector.YZ * r.Bivector.ZX),
            (l.Scalar * r.Trivector.XYZ) + (l.Vector.X * r.Bivector.YZ) + (l.Vector.Y * r.Bivector.ZX) + (l.Vector.Z * r.Bivector.XY) + (l.Bivector.XY * r.Vector.Z) + (l.Bivector.YZ * r.Vector.X) + (l.Bivector.ZX * r.Vector.Y) + (l.Trivector.XYZ * r.Scalar)
        );

    #endregion
}
