using System.Numerics;

namespace Engine.Math.NewFolder;

public static class Matrix2x2Factory
{
    public static Matrix2x2<T> CreateRotationFromRotor<T>(in Rotor2<T> rotor) where T : unmanaged, INumber<T>
    {
        T two = T.One + T.One;

        T z = rotor.Bivector.XY;
        T w = rotor.Scalar;

        T m00 = T.One - (two * z * z);  //1-2(y^2+z^2)
        T m01 = -two * w * z;           //2(xy-wz)
        T m10 = two * w * z;            //2(xy+wz)
        T m11 = T.One - (two * z * z);  //1-2(x^2+z^2)

        return new(
            m00, m01,
            m10, m11
        );
    }
}
