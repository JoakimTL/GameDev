using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Engine.Datatypes.Transforms;
public static class TransformExtensions
{
    public static TransformData<Vector3, Quaternion, Vector3> GetInterpolated(this TransformData<Vector3, Quaternion, Vector3> a, TransformData<Vector3, Quaternion, Vector3> b, float interpolation)
        => new(Vector3.Lerp(a.Translation, b.Translation, interpolation),
            Quaternion.Slerp(a.Rotation, b.Rotation, interpolation),
            Vector3.Lerp(a.Scale, b.Scale, interpolation));
    public static TransformData<Vector3, Quaternion, Vector3> GetExtrapolation(this TransformData<Vector3, Quaternion, Vector3> a, TransformData<Vector3, Quaternion, Vector3> b, float interpolation)
    {
        var angleA = MathF.Acos(2 * a.Rotation.W);
        var angleB = MathF.Acos(2 * b.Rotation.W);
        return new(Vector3.Lerp(a.Translation, a.Translation * 2 - b.Translation, interpolation),
                Quaternion.CreateFromAxisAngle(a.Rotation.Backward(), angleA + (angleA - angleB) * interpolation),
                Vector3.Lerp(a.Scale, a.Scale * 2 - b.Scale, interpolation));
    }

    public static Matrix4x4 GetMatrix(this TransformData<Vector3, Quaternion, Vector3> transformData)
        => Matrix4x4.CreateScale(transformData.Scale) * Matrix4x4.CreateFromQuaternion(transformData.Rotation) * Matrix4x4.CreateTranslation(transformData.Translation);


}
