using System.Numerics;

namespace Engine.Data.Transforms;
public class Transform2 : TransformBase<Vector2, float, Vector2> {

	public Transform2() {
		this.Scale = Vector2.One;
	}

	protected override float GetGlobalRotation() {
		if ( this.Parent is not null )
			return this.Rotation + this.Parent.GlobalRotation;
		return this.Rotation;
	}

	protected override Vector2 GetGlobalScale() {
		if ( this.Parent is not null )
			return this.Scale * this.Parent.GlobalScale;
		return this.Scale;
	}

	protected override Vector2 GetGlobalTranslation() {
		if ( this.Parent is not null )
			return Vector2.Transform( this.Translation, this.Parent.Matrix );
		return this.Translation;
	}

	protected override TransformData<Vector2, float, Vector2> GetInterpolated( TransformBase<Vector2, float, Vector2> other, float interpolation ) =>
		new( Vector2.Lerp( this.GlobalTranslation, other.GlobalTranslation, interpolation ),
			 (this.GlobalRotation * ( 1 - interpolation )) + (other.GlobalRotation * interpolation),
			Vector2.Lerp( this.GlobalScale, other.GlobalScale, interpolation ) );

	protected override Matrix4x4 GetLocalMatrix() {
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation( new Vector3( this.Translation, 0 ) );
		Matrix4x4 rotationMatrix = Matrix4x4.CreateRotationZ( this.Rotation );
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale( new Vector3( this.Scale, 0 ) );
		return scaleMatrix * rotationMatrix * translationMatrix;
	}


	protected override void RevertAdjustment( TransformBase<Vector2, float, Vector2>? parent ) {
		if ( parent is null )
			return;
		this.Translation = Vector2.Transform( this.Translation, parent.Matrix );
		this.Rotation = parent.GlobalRotation * this.Rotation;
		this.Scale *= parent.GlobalScale;
	}

	protected override void Adjust( TransformBase<Vector2, float, Vector2>? parent ) {
		if ( parent is null )
			return;
		this.Translation = Vector2.Transform( this.Translation, parent.InverseMatrix );
		this.Rotation -= parent.GlobalRotation;
		this.Scale /= parent.GlobalScale;
	}
}
