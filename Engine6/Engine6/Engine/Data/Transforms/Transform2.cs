using System.Numerics;

namespace Engine.Data.Transforms;
public class Transform2<TScalar> : TransformBase<TScalar, Vector2<TScalar>, TScalar, Vector2<TScalar>>
	where TScalar :
		unmanaged, IFloatingPointIeee754<TScalar> {

	public Transform2() {
		this.Scale = Vector2<TScalar>.One;
	}

	protected override TScalar GetGlobalRotation() {
		if (this.Parent is not null)
			return this.Rotation + this.Parent.GlobalRotation;
		return this.Rotation;
	}

	protected override Vector2<TScalar> GetGlobalScale() {
		if (this.Parent is not null)
			return this.Scale.MultiplyEntrywise( this.Parent.GlobalScale );
		return this.Scale;
	}

	protected override Vector2<TScalar> GetGlobalTranslation() {
		if (this.Parent is not null)
			return this.Parent.Matrix.TransformWorld( this.Translation ) ?? this.Translation;
		return this.Translation;
	}

	protected override TransformData<Vector2<TScalar>, TScalar, Vector2<TScalar>> GetInterpolated( TransformBase<TScalar, Vector2<TScalar>, TScalar, Vector2<TScalar>> other, TScalar interpolation ) =>
		new( this.GlobalTranslation.Lerp( other.GlobalTranslation, interpolation ),
			 (this.GlobalRotation * (TScalar.One - interpolation)) + (other.GlobalRotation * interpolation),
			this.GlobalScale.Lerp( other.GlobalScale, interpolation ) );

	protected override Matrix4x4<TScalar> GetLocalMatrix() {
		Matrix4x4<TScalar> translationMatrix = Engine.Matrix.Create4x4.Translation( this.Translation );
		Matrix4x4<TScalar> rotationMatrix = Engine.Matrix.Create4x4.RotationZ( this.Rotation );
		Matrix4x4<TScalar> scaleMatrix = Engine.Matrix.Create4x4.Scaling( this.Scale );
		return scaleMatrix * rotationMatrix * translationMatrix;
	}


	protected override void RevertAdjustment( TransformBase<TScalar, Vector2<TScalar>, TScalar, Vector2<TScalar>>? parent ) {
		if (parent is null)
			return;
		this.Translation = parent.Matrix.TransformWorld( this.Translation ) ?? this.Translation;
		this.Rotation += parent.GlobalRotation;
		this.Scale = this.Scale.MultiplyEntrywise( parent.GlobalScale );
	}

	protected override void Adjust( TransformBase<TScalar, Vector2<TScalar>, TScalar, Vector2<TScalar>>? parent ) {
		if (parent is null)
			return;
		this.Translation = parent.InverseMatrix.TransformWorld( this.Translation ) ?? this.Translation;
		this.Rotation -= parent.GlobalRotation;
		this.Scale = this.Scale.DivideEntrywise( parent.GlobalScale );
	}
}
