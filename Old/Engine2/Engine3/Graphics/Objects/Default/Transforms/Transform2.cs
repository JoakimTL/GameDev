using System;
using Engine.Graphics.Objects;
using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.Transforms {
	public class Transform2 : Transform<Vector2, float, Vector2> {

		private Vector2 translation;
		private float rotation;
		private Vector2 scale;
		private bool normalizedScale;

		protected override Matrix4 TranslationMatrix => Matrix4Factory.CreateTranslation( translation.X, translation.Y, 0 );
		protected override Matrix4 RotationMatrix => Matrix4Factory.CreateRotationZ( rotation );
		protected override Matrix4 ScaleMatrix {
			get {
				if( normalizedScale && Parent != null ) {
					Vector2 pGlobal = ( (Transform2) Parent ).GlobalScale;
					float minScale = Math.Min( pGlobal.X, pGlobal.Y );
					return Matrix4Factory.CreateScale( scale.X / pGlobal.X * minScale, scale.Y / pGlobal.Y * minScale, 0 );
				}
				return Matrix4Factory.CreateScale( scale.X, scale.Y, 0 );
			}
		}

		public Transform2() {
			translation = Vector2.Zero;
			rotation = 0;
			scale = Vector2.One;
			normalizedScale = false;
		}

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
		public override Vector2 Scale {
			get {
				return scale;
			}
			set {
				SetScale( value );
			}
		}
		public bool Normalized {
			get {
				return normalizedScale;
			}
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

		public void SetScale( Vector2 v ) {
			if( scale == v )
				return;
			scale = v;
			SetChanged();
		}

		public void SetNormalization( bool n ) {
			if( normalizedScale == n )
				return;
			normalizedScale = n;
			SetChanged();
		}

		public override Vector2 GlobalTranslation {
			get {
				if( Parent != null )
					return Vector2.Transform( translation, parent.Matrix );
				return translation;
			}
		}

		public override float GlobalRotation {
			get {
				if( Parent != null )
					return rotation + parent.GlobalRotation;
				return rotation;
			}
		}

		public override Vector2 GlobalScale {
			get {
				if( Parent != null ) {
					Vector2 pGlobal = parent.GlobalScale;
					if( normalizedScale ) {
						float minScale = Math.Min( pGlobal.X, pGlobal.Y );
						return new Vector2( scale.X / pGlobal.X * minScale, scale.Y / pGlobal.Y * minScale ) * pGlobal;
					} else
						return scale * pGlobal;
				}
				return scale;
			}
		}

	}
}
