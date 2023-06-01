using System.Numerics;

namespace Engine.Datatypes.Transforms;
public static class TransformExtensions
{
    public static TransformData<Vector3, Quaternion, Vector3> GetInterpolated(this TransformData<Vector3, Quaternion, Vector3> a, TransformData<Vector3, Quaternion, Vector3> b, float interpolation)
        => new(Vector3.Lerp(a.Translation, b.Translation, interpolation),
            Quaternion.Lerp( a.Rotation, b.Rotation, interpolation),
            Vector3.Lerp(a.Scale, b.Scale, interpolation));
    public static TransformData<Vector3, Quaternion, Vector3> GetExtrapolation(this TransformData<Vector3, Quaternion, Vector3> a, TransformData<Vector3, Quaternion, Vector3> b, float interpolation) 
        => new(Vector3.Lerp(a.Translation, a.Translation * 2 - b.Translation, interpolation),
                Quaternion.Lerp(b.Rotation, a.Rotation, interpolation + 1),
                Vector3.Lerp(a.Scale, a.Scale * 2 - b.Scale, interpolation));

    public static Matrix4x4 GetMatrix(this TransformData<Vector3, Quaternion, Vector3> transformData)
        => Matrix4x4.CreateScale(transformData.Scale) * Matrix4x4.CreateFromQuaternion(transformData.Rotation) * Matrix4x4.CreateTranslation(transformData.Translation);


}
