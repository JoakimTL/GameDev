﻿using Engine.Graphics.Objects;
using Engine.LMath;

namespace Engine.Graphics.Objects.Default.Transforms {
	public class Transform3i : Transform<Vector3i, Quaternion, Vector3i> {

		private bool translationReversed;
		private Vector3i translation;
		private Quaternion rotation;
		private Vector3i scale;

		protected override Matrix4 TranslationMatrix => Matrix4Factory.CreateTranslation( translationReversed ? -translation.AsFloat : translation.AsFloat );
		protected override Matrix4 RotationMatrix => Matrix4Factory.CreateFromQuaternion( rotation );
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
				return rotation;
			}
			set {
				SetRotation( value );
			}
		}
		public override Vector3i Scale {
			get {
				return scale;
			}
			set {
				SetScale( value );
			}
		}

		public Transform3i( bool translationReversed = false ) {
			this.translationReversed = translationReversed;
			translation = Vector3i.Zero;
			rotation = Quaternion.Identity;
			scale = Vector3i.One;
		}

		public void SetTranslation( Vector3i v ) {
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

		public void SetScale( Vector3i v ) {
			if( scale == v )
				return;
			scale = v;
			SetChanged();
		}

		public override Vector3i GlobalTranslation {
			get {
				if( parent != null )
					return Vector3.Transform( translation.AsFloat, parent.TMatrix ).IntFloored;
				return translation;
			}
		}

		public Vector3 GlobalTranslationFloat {
			get {
				if( parent != null )
					return Vector3.Transform( translation.AsFloat, parent.TMatrix );
				return translation.AsFloat;
			}
		}

		public override Quaternion GlobalRotation {
			get {
				if( parent != null )
					return rotation * parent.GlobalRotation;
				return rotation;
			}
		}

		public override Vector3i GlobalScale {
			get {
				if( parent != null ) {
					Vector3i pGlobal = parent.GlobalScale;
					return scale * pGlobal;
				}
				return scale;
			}
		}
	}
}
