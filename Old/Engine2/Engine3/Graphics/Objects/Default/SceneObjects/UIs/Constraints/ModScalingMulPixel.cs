using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModScalingMulPixel : IConstraint {

		public Vector2 Scale { get; set; }

		public ModScalingMulPixel( Vector2 scaling )  {
			this.Scale = scaling;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Scale *= new Vector2( Scale.X / window.Size.X * window.AspectRatioVector.X * 2, Scale.Y / window.Size.Y * window.AspectRatioVector.Y * 2);
		}
	}
}
