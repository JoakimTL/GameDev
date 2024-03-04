namespace Engine.Math.Old;

public readonly struct Complex<T>(T real, T imaginary) where T : System.Numerics.INumberBase<T>
{
    public readonly T Real = real;
    public readonly T Imaginary = imaginary;
}
