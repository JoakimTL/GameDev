using Engine.Graphics.Objects;
using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.Transforms {
	public class Transform3iAABB : Transform<Vector3i, Quaternion, Vector3i> {

		private bool translationReversed;
		private Vector3i translation;
		private Vector3i scale;

		protected override Matrix4 TranslationMatrix => Matrix4Factory.CreateTranslation( translationReversed ? -translation.AsFloat : translation.AsFloat );
		protected override Matrix4 RotationMatrix => Matrix4Factory.CreateFromQuaternion( Quaternion.Identity );
		protected override Matrix4 ScaleMatrix => Matrix4Factory.CreateScale( scale.AsFloat );

		public override Vector3i Translation {
			get {
				return translation;
			}
			set {
				SetTranslation( value );
			}
		}

		public override Quaternion Rotation {
			get {
				return Quaternion.Identity;
			}
			set { }
		}

		public override Vector3i Scale {
			get {
				return scale;
			}
			set {
				SetScale( value );
			}
		}

		protected override Matrix4 CreateMatrix() {
			return ScaleMatrix * TranslationMatrix;
		}

		public Transform3iAABB( bool translationReversed = false ) {
			this.translationReversed = translationReversed;
			translation = Vector3i.Zero;
			scale = Vector3i.One;
		}

		public void SetTranslation( Vector3i v ) {
			if( translation == v )
				return;
			translation = v;
			SetChanged();
		}

		public void SetScale( Vector3i v ) {
			if( scale == v )
				return;
			scale = v;
			SetChanged();
		}

		public override Vector3i GlobalTranslation {
			get {
				if( Parent != null )
					return Vector3.Transform( translation.AsFloat, Parent.Matrix ).IntFloored;
				return translation;
			}
		}

		public Vector3 GlobalTranslationFloat {
			get {
				if( Parent != null )
					return Vector3.Transform( translation.AsFloat, Parent.Matrix );
				return translation.AsFloat;
			}
		}

		public override Quaternion GlobalRotation {
			get {
				return Quaternion.Identity;
			}
		}

		public override Vector3i GlobalScale {
			get {
				if( Parent != null ) {
					return scale * parent.GlobalScale;
				}
				return scale;
			}
		}
	}
}
