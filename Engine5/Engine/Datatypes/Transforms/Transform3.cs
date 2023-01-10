using System.Numerics;

namespace Engine.Datatypes.Transforms;

public class Transform3 : TransformBase<Vector3, Quaternion, Vector3> {

	public Transform3() {
		Rotation = Quaternion.Identity;
		Scale = Vector3.One;
	}

	protected override Quaternion GetGlobalRotation() {
		if ( Parent is not null )
			return Parent.GlobalRotation * Rotation;
		return Rotation;
	}

	protected override Vector3 GetGlobalScale() {
		if ( Parent is not null )
			return Scale * Parent.GlobalScale;
		return Scale;
	}

	protected override Vector3 GetGlobalTranslation() {
		if ( Parent is not null )
			return Vector3.Transform( Translation, Parent.Matrix );
		return Translation;
	}

	protected override TransformData<Vector3, Quaternion, Vector3> GetInterpolated( TransformBase<Vector3, Quaternion, Vector3> other, float interpolation ) =>
		new( Vector3.Lerp( GlobalTranslation, other.GlobalTranslation, interpolation ),
			Quaternion.Slerp( GlobalRotation, other.GlobalRotation, interpolation ),
			Vector3.Lerp( GlobalScale, other.GlobalScale, interpolation ) );

	protected override Matrix4x4 GetLocalMatrix() {
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation( Translation );
		Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion( Rotation );
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale( Scale );
		return scaleMatrix * rotationMatrix * translationMatrix;
	}

	protected override void RevertAdjustment( TransformBase<Vector3, Quaternion, Vector3>? parent ) {
		if ( parent is null )
			return;
		Translation = Vector3.Transform( Translation, parent.Matrix );
		Rotation = parent.GlobalRotation * Rotation;
		Scale *= parent.GlobalScale;
	}

	protected override void Adjust( TransformBase<Vector3, Quaternion, Vector3>? parent ) {
		if ( parent is null )
			return;
		Translation = Vector3.Transform( Translation, parent.InverseMatrix );
		Rotation = Quaternion.Inverse(parent.GlobalRotation) * Rotation;
		Scale /= parent.GlobalScale;
	}
}
