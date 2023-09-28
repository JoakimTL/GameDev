using System.Numerics;

namespace Engine.Data.Transforms;

public class Transform3 : TransformBase<Vector3, Quaternion, Vector3> {

	public Transform3() {
		this.Rotation = Quaternion.Identity;
		this.Scale = Vector3.One;
	}

	protected override Quaternion GetGlobalRotation() {
		if ( this.Parent is not null )
			return this.Parent.GlobalRotation * this.Rotation;
		return this.Rotation;
	}

	protected override Vector3 GetGlobalScale() {
		if ( this.Parent is not null )
			return this.Scale * this.Parent.GlobalScale;
		return this.Scale;
	}

	protected override Vector3 GetGlobalTranslation() {
		if ( this.Parent is not null )
			return Vector3.Transform( this.Translation, this.Parent.Matrix );
		return this.Translation;
	}

	protected override TransformData<Vector3, Quaternion, Vector3> GetInterpolated( TransformBase<Vector3, Quaternion, Vector3> other, float interpolation ) =>
		new( Vector3.Lerp( this.GlobalTranslation, other.GlobalTranslation, interpolation ),
			Quaternion.Slerp( this.GlobalRotation, other.GlobalRotation, interpolation ),
			Vector3.Lerp( this.GlobalScale, other.GlobalScale, interpolation ) );

	protected override Matrix4x4 GetLocalMatrix() {
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation( this.Translation );
		Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion( this.Rotation );
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale( this.Scale );
		return scaleMatrix * rotationMatrix * translationMatrix;
	}

	protected override void RevertAdjustment( TransformBase<Vector3, Quaternion, Vector3>? parent ) {
		if ( parent is null )
			return;
		this.Translation = Vector3.Transform( this.Translation, parent.Matrix );
		this.Rotation = parent.GlobalRotation * this.Rotation;
		this.Scale *= parent.GlobalScale;
	}

	protected override void Adjust( TransformBase<Vector3, Quaternion, Vector3>? parent ) {
		if ( parent is null )
			return;
		this.Translation = Vector3.Transform( this.Translation, parent.InverseMatrix );
		this.Rotation = Quaternion.Inverse( parent.GlobalRotation ) * this.Rotation;
		this.Scale /= parent.GlobalScale;
	}
}
