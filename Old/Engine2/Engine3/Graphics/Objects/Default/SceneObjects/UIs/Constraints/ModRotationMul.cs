using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModRotationMul : IConstraint {

		public float Rotation { get; set; }

		public ModRotationMul( float rotation )  {
			this.Rotation = rotation;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Rotation *= Rotation;
		}
	}
}
