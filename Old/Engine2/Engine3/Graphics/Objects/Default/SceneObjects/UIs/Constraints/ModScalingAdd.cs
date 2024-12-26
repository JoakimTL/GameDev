using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModScalingAdd : IConstraint {

		public Vector2 Scale { get; set; }

		public ModScalingAdd( Vector2 scaling )  {
			this.Scale = scaling;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Scale += Scale;
		}
	}
}
