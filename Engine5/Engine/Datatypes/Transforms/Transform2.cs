using System.Numerics;

namespace Engine.Datatypes.Transforms;
public class Transform2 : TransformBase<Vector2, float, Vector2>
{

    public Transform2()
    {
        Scale = Vector2.One;
    }

    protected override float GetGlobalRotation()
    {
        if (Parent is not null)
            return Rotation + Parent.GlobalRotation;
        return Rotation;
    }

    protected override Vector2 GetGlobalScale()
    {
        if (Parent is not null)
            return Scale * Parent.GlobalScale;
        return Scale;
    }

    protected override Vector2 GetGlobalTranslation()
    {
        if (Parent is not null)
            return Vector2.Transform(Translation, Parent.Matrix);
        return Translation;
    }

    protected override TransformData<Vector2, float, Vector2> GetInterpolated(TransformBase<Vector2, float, Vector2> other, float interpolation) =>
        new(Vector2.Lerp(GlobalTranslation, other.GlobalTranslation, interpolation),
            (GlobalRotation * (1 - interpolation)) + (other.GlobalRotation * interpolation),
            Vector2.Lerp(GlobalScale, other.GlobalScale, interpolation));

    protected override Matrix4x4 GetLocalMatrix()
    {
        Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(new Vector3(Translation, 0));
        Matrix4x4 rotationMatrix = Matrix4x4.CreateRotationZ(Rotation);
        Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(new Vector3(Scale, 0));
        return scaleMatrix * rotationMatrix * translationMatrix;
    }


    protected override void RevertAdjustment(TransformBase<Vector2, float, Vector2>? parent)
    {
        if (parent is null)
            return;
        Translation = Vector2.Transform(Translation, parent.Matrix);
        Rotation = parent.GlobalRotation * Rotation;
        Scale *= parent.GlobalScale;
    }

    protected override void Adjust(TransformBase<Vector2, float, Vector2>? parent)
    {
        if (parent is null)
            return;
        Translation = Vector2.Transform(Translation, parent.InverseMatrix);
        Rotation -= parent.GlobalRotation;
        Scale /= parent.GlobalScale;
    }
}
