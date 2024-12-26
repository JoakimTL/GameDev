using Engine.Graphics.Objects;
using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.Transforms {
	public class Transform3 : Transform<Vector3, Quaternion, Vector3> {

		private bool translationReversed;
		private Vector3 translation;
		private Quaternion rotation;
		private Vector3 scale;

		protected override Matrix4 TranslationMatrix => Matrix4Factory.CreateTranslation( translationReversed ? -translation : translation );
		protected override Matrix4 RotationMatrix => Matrix4Factory.CreateFromQuaternion( rotation );
		protected override Matrix4 ScaleMatrix => Matrix4Factory.CreateScale( scale );

		public override Vector3 Translation {
			get {
				return translation;
			}
			set {
				SetTranslation( value );
			}
		}
		public override Quaternion Rotation {
			get {
				return rotation;
			}
			set {
				SetRotation( value );
			}
		}
		public override Vector3 Scale {
			get {
				return scale;
			}
			set {
				SetScale( value );
			}
		}

		public Transform3( bool translationReversed = false ) {
			this.translationReversed = translationReversed;
			translation = Vector3.Zero;
			rotation = Quaternion.Identity;
			scale = Vector3.One;
		}

		public void SetTranslation( Vector3 v ) {
			if( translation == v )
				return;
			translation = v;
			SetChanged();
		}

		public void SetRotation( Quaternion q ) {
			if( rotation == q )
				return;
			rotation = q;
			SetChanged();
		}

		public void SetScale( Vector3 v ) {
			if( scale == v )
				return;
			scale = v;
			SetChanged();
		}

		public override Vector3 GlobalTranslation {
			get {
				if( Parent != null )
					return Vector3.Transform( translation, parent.Matrix );
				return translation;
			}
		}

		public override Quaternion GlobalRotation {
			get {
				if( Parent != null )
					return parent.GlobalRotation * rotation;
				return rotation;
			}
		}

		public override Vector3 GlobalScale {
			get {
				if( Parent != null ) {
					Vector3 pGlobal = parent.GlobalScale;
					return scale * pGlobal;
				}
				return scale;
			}
		}
	}
}
