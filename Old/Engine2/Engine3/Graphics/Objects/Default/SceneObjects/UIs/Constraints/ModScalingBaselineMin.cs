using Engine.LinearAlgebra;
using Engine.Utilities.Time;
using System;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModScalingBaselineMin : IConstraint {
		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			float s = Math.Min( transform.Scale.X, transform.Scale.Y );
			transform.Scale = s;
		}
	}
}
