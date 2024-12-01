using System.Numerics;

namespace Engine.Transforms.Models;

public class Transform3<TScalar> : TransformBase<TScalar, Vector3<TScalar>, Rotor3<TScalar>, Vector3<TScalar>>
	where TScalar :
		unmanaged, IFloatingPointIeee754<TScalar> {

	public Transform3() {
		this.Rotation = Rotor3<TScalar>.MultiplicativeIdentity;
		this.Scale = Vector3<TScalar>.One;
	}

	protected override Rotor3<TScalar> GetGlobalRotation() {
		if (this.Parent is not null)
			return this.Parent.GlobalRotation * this.Rotation;
		return this.Rotation;
	}

	protected override Vector3<TScalar> GetGlobalScale() {
		if (this.Parent is not null)
			return this.Scale.MultiplyEntrywise( this.Parent.GlobalScale );
		return this.Scale;
	}

	protected override Vector3<TScalar> GetGlobalTranslation() {
		if (this.Parent is not null)
			return this.Parent.Matrix.TransformWorld( this.Translation ) ?? this.Translation;
		return this.Translation;
	}

	protected override TransformData<Vector3<TScalar>, Rotor3<TScalar>, Vector3<TScalar>> GetInterpolated( TransformBase<TScalar, Vector3<TScalar>, Rotor3<TScalar>, Vector3<TScalar>> other, TScalar interpolation )
		=> new( this.GlobalTranslation.Lerp( other.GlobalTranslation, interpolation ),
			this.GlobalRotation.Slerp( other.GlobalRotation, interpolation ),
			this.GlobalScale.Lerp( other.GlobalScale, interpolation ) );

	protected override Matrix4x4<TScalar> GetLocalMatrix() {
		Matrix4x4<TScalar> translationMatrix = Engine.Matrix.Create4x4.Translation( this.Translation );
		Matrix4x4<TScalar> rotationMatrix = Engine.Matrix.Create4x4.RotationFromRotor( this.Rotation );
		Matrix4x4<TScalar> scaleMatrix = Engine.Matrix.Create4x4.Scaling( this.Scale );
		return scaleMatrix * rotationMatrix * translationMatrix;
	}

	protected override void RevertAdjustment( TransformBase<TScalar, Vector3<TScalar>, Rotor3<TScalar>, Vector3<TScalar>>? parent ) {
		if (parent is null)
			return;
		this.Translation = parent.Matrix.TransformWorld( this.Translation ) ?? this.Translation;
		this.Rotation = parent.GlobalRotation * this.Rotation;
		this.Scale = this.Scale.MultiplyEntrywise( parent.GlobalScale );
	}

	protected override void Adjust( TransformBase<TScalar, Vector3<TScalar>, Rotor3<TScalar>, Vector3<TScalar>>? parent ) {
		if (parent is null)
			return;
		if (!parent.GlobalRotation.TryGetInverse( out Rotor3<TScalar> inverse ))
			throw new ArgumentOutOfRangeException( nameof( parent ), "Parent rotation cannot have a length of 0 (Rotor is invalid)." );
		this.Translation = parent.InverseMatrix.TransformWorld( this.Translation ) ?? this.Translation;
		this.Rotation = inverse * this.Rotation;
		this.Scale = this.Scale.DivideEntrywise( parent.GlobalScale );
	}
}
