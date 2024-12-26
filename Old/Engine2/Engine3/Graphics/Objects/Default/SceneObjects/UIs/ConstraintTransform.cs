using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public sealed class ConstraintTransform {

		public Vector4 Color;
		public Vector2 Translation;
		public Vector2 Scale;
		public float Rotation;
		public ReadonlyInterface Interface { get; private set; }

		public ConstraintTransform( Vector4 color, Vector2 translation, Vector2 scale, float rot ) {
			Color = color;
			Translation = translation;
			Scale = scale;
			Rotation = rot;
			Interface = new ReadonlyInterface( this );
		}

		public ConstraintTransform( ConstraintTransform source ) {
			Color = source.Color;
			Translation = source.Translation;
			Scale = source.Scale;
			Rotation = source.Rotation;
			Interface = new ReadonlyInterface( this );
		}

		public ConstraintTransform() {
			Color = Vector4.One;
			Translation = Vector2.Zero;
			Scale = Vector2.One;
			Rotation = 0;
			Interface = new ReadonlyInterface( this );
		}

		public void Reset() {
			Color = Vector4.One;
			Translation = Vector2.Zero;
			Scale = Vector2.One;
			Rotation = 0;
		}

		public void Set( ConstraintTransform constraint ) {
			Color = constraint.Color;
			Translation = constraint.Translation;
			Scale = constraint.Scale;
			Rotation = constraint.Rotation;
		}

		public void SetFromInterpolation( ConstraintTransform current, ConstraintTransform target, float interp ) {
			Color = current.Color * ( 1 - interp ) + target.Color * interp;
			Translation = current.Translation * ( 1 - interp ) + target.Translation * interp;
			Scale = current.Scale * ( 1 - interp ) + target.Scale * interp;
			float shortest_angle = ( ( ( target.Rotation - current.Rotation ) % (float) ( Math.PI * 2 ) ) + (float) Math.PI * 3 ) % (float) ( Math.PI * 2 ) - (float) Math.PI;
			Rotation = current.Rotation + shortest_angle * interp;
		}

		public bool SameAs( ConstraintTransform c ) {
			return c.Rotation == Rotation && c.Scale == Scale && c.Translation == Translation && c.Color == Color;
		}

		public override string ToString() {
			return Translation + " " + Scale + " " + Rotation + " " + Color;
		}

		public sealed class ReadonlyInterface {

			private readonly ConstraintTransform transform;

			public Vector4 Color { get => transform.Color; }
			public Vector2 Translation { get => transform.Translation; }
			public Vector2 Scale { get => transform.Scale; }
			public float Rotation { get => transform.Rotation; }

			public ReadonlyInterface( ConstraintTransform transform ) {
				this.transform = transform;
			}

		}

	}
}
