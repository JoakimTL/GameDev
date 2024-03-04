namespace Engine.Math.Old;

public static class ComplexOperations
{

    public static Complex<T> Negate<T>(in this Complex<T> c) where T : System.Numerics.ISignedNumber<T>
        => new(-c.Real, -c.Imaginary);

    public static Complex<T> Add<T>(in this Complex<T> l, in Complex<T> r) where T : System.Numerics.INumberBase<T>
        => new(l.Real + r.Real, l.Imaginary + r.Imaginary);

    public static Complex<T> Subtract<T>(in this Complex<T> l, in Complex<T> r) where T : System.Numerics.INumberBase<T>
        => new(l.Real - r.Real, l.Imaginary - r.Imaginary);

    public static Complex<T> ScalarMultiplication<T>(in this Complex<T> l, T r) where T : System.Numerics.INumberBase<T>
        => new(l.Real * r, l.Imaginary * r);


    public static Complex<T> Multiplication<T>(in this Complex<T> l, in Complex<T> r) where T : System.Numerics.INumberBase<T>
        => new(
            l.Real * r.Real - l.Imaginary * r.Imaginary,
            l.Real * r.Imaginary + l.Imaginary * r.Real
        );


}