using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModScalingBaselineY : IConstraint {

		private float ratio;

		public ModScalingBaselineY( float ratio ) {
			this.ratio = ratio;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Scale.X = transform.Scale.Y * ratio;
		}
	}
}
