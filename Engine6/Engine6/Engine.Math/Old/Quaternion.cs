namespace Engine.Math.Old;

public readonly struct Quaternion<T>(Bivector3<T> imaginary, T real) where T : System.Numerics.INumberBase<T>
{

    public readonly Bivector3<T> Imaginary = imaginary;
    public readonly T Real = real;
}
