using Engine.Graphics.Objects;
using Engine.LMath;

namespace Engine.Graphics.Objects.Default.Transforms {
	public class Transform2Particle : Transform<Vector2, float, float> {

		private Vector2 translation;
		private float rotation;
		private float scale;

		protected override Matrix4 TranslationMatrix => Matrix4Factory.CreateTranslation( translation.X, translation.Y, 0 );
		protected override Matrix4 RotationMatrix => Matrix4Factory.CreateRotationZ( rotation );
		protected override Matrix4 ScaleMatrix => Matrix4Factory.CreateScale( scale, scale, 0 );

		public override Vector2 Translation {
			get {
				return translation;
			}
			set {
				SetTranslation( value );
			}
		}
		public override float Rotation {
			get {
				return rotation;
			}
			set {
				SetRotation( value );
			}
		}
		public override float Scale {
			get {
				return scale;
			}
			set {
				SetScale( value );
			}
		}

		public Transform2Particle() {
			translation = Vector2.Zero;
			rotation = 0;
			scale = 1;
		}

		public void SetTranslation( Vector2 v ) {
			if( translation == v )
				return;
			translation = v;
			SetChanged();
		}

		public void SetRotation( float q ) {
			if( rotation == q )
				return;
			rotation = q;
			SetChanged();
		}

		public void SetScale( float v ) {
			if( scale == v )
				return;
			scale = v;
			SetChanged();
		}

		public override Vector2 GlobalTranslation {
			get {
				if( parent != null )
					return Vector3.Transform( new Vector3( translation.X, translation.Y, 0 ), parent.TMatrix ).XY;
				return translation;
			}
		}

		public override float GlobalRotation {
			get {
				if( parent != null )
					return rotation + parent.GlobalRotation;
				return rotation;
			}
		}

		public override float GlobalScale {
			get {
				if( parent != null ) {
					float pGlobal = parent.GlobalScale;
					return scale * pGlobal;
				}
				return scale;
			}
		}
	}
}
