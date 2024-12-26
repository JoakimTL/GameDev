using Engine.Graphics.Objects;
using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.Transforms {
	public class Transform3Scale : Transform<Vector3i, Quaternion, Vector3> {

		private Vector3 scale;

		protected override Matrix4 TranslationMatrix => Matrix4.Identity;
		protected override Matrix4 RotationMatrix => Matrix4.Identity;
		protected override Matrix4 ScaleMatrix => Matrix4Factory.CreateScale( scale );

		public override Vector3i Translation {
			get {
				return 0;
			}
			set { }
		}

		public override Quaternion Rotation {
			get {
				return Quaternion.Identity;
			}
			set { }
		}

		public override Vector3 Scale {
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

		public Transform3Scale() {
			scale = Vector3.One;
		}

		public void SetScale( Vector3 v ) {
			if( scale == v )
				return;
			scale = v;
			SetChanged();
		}

		public override Vector3i GlobalTranslation {
			get {
				return 0;
			}
		}

		public override Quaternion GlobalRotation {
			get {
				return Quaternion.Identity;
			}
		}

		public override Vector3 GlobalScale {
			get {
				if( Parent != null ) {
					return scale * parent.GlobalScale;
				}
				return scale;
			}
		}
	}
}
