using Engine.Utilities.Data;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public class ConstraintAnimation {

		private ConstraintTransform sourceTransform;
		private readonly ConstraintBundle target;
		private readonly float duration;
		private float startTime;
		private readonly InterpolationMethod interpolation;

		public bool Animating { get; private set; }

		public ConstraintAnimation( ConstraintBundle target, float duration, InterpolationMethod interpolation ) {
			this.target = target;
			this.duration = duration;
			this.interpolation = interpolation;
			Animating = duration > 0;
		}

		internal void Start( ConstraintTransform source, float time ) {
			sourceTransform = new ConstraintTransform( source );
			startTime = time;
			Animating = duration > 0;
		}

		internal void Update( float time, ConstraintTransform transform, UIElement e, GLWindow window ) {
			target.Update( time, e, window );
			if( duration > 0 && Animating ) {
				transform.SetFromInterpolation( sourceTransform, target.Transform, interpolation.Invoke( ( time - startTime ) / duration ) );
				Animating = time - startTime < duration;
			} else {
				transform.Set( target.Transform );
			}
		}

		internal void FinalizeAnimation( float time, ConstraintTransform transform, UIElement e, GLWindow window ) {
			target.Update( time, e, window );
			transform.Set( target.Transform );
		}
	}
}
