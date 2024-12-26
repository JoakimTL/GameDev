using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModScalingBaselineX : IConstraint {

		private float ratio;

		public ModScalingBaselineX( float ratio ) {
			this.ratio = ratio;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Scale.Y = transform.Scale.X * ratio;
		}
	}
}
