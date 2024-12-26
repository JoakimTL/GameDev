using Engine.Graphics.Objects;
using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.Transforms {
	public class Transform3Particle : Transform<Vector3, float, float> {

		private Vector3 translation;
		private float rotation;
		private float scale;

		protected override Matrix4 TranslationMatrix => Matrix4Factory.CreateTranslation( translation );
		protected override Matrix4 RotationMatrix => GetRotation();

		private Matrix4 GetRotation() {
			Matrix4 m = Matrix4.Identity;

			m.M00 = _vMat.M00;
			m.M01 = _vMat.M10;
			m.M02 = _vMat.M20;
			m.M10 = _vMat.M01;
			m.M11 = _vMat.M11;
			m.M12 = _vMat.M21;
			m.M20 = _vMat.M02;
			m.M21 = _vMat.M12;
			m.M22 = _vMat.M22;

			return Matrix4Factory.CreateRotationZ( rotation ) * m;
		}

		protected override Matrix4 ScaleMatrix => Matrix4Factory.CreateScale( scale );

		public override Vector3 Translation { get { return translation; } set { SetTranslation( value ); } }
		public override float Rotation { get { return rotation; } set { SetRotation( value ); } }
		public override float Scale { get { return scale; } set { SetScale( value ); } }

		private Matrix4 _vMat;

		public Transform3Particle() {
			translation = Vector3.Zero;
			rotation = 0;
			scale = 1;
		}

		public void SetViewMatrix( Matrix4 vMat ) {
			_vMat = vMat;
		}
		
		public void SetTranslation( Vector3 v ) {
			if ( translation == v )
				return;
			translation = v;
			SetChanged();
		}

		public void SetRotation( float q ) {
			if ( rotation == q )
				return;
			rotation = q;
			SetChanged();
		}

		public void SetScale( float v ) {
			if ( scale == v )
				return;
			scale = v;
			SetChanged();
		}

		public override Vector3 GlobalTranslation {
			get {
				if ( Parent != null )
					return Vector3.Transform( translation, parent.Matrix );
				return translation;
			}
		}

		public override float GlobalRotation {
			get {
				if ( Parent != null )
					return rotation + parent.GlobalRotation;
				return rotation;
			}
		}

		public override float GlobalScale {
			get {
				if ( Parent != null ) {
					float pGlobal = parent.GlobalScale;
					return scale * pGlobal;
				}
				return scale;
			}
		}
	}
}
