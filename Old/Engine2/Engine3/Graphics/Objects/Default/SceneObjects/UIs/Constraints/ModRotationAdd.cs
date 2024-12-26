using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModRotationAdd : IConstraint {

		public float Rotation { get; set; }

		public ModRotationAdd( float rotation )  {
			this.Rotation = rotation;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Rotation += Rotation;
		}
	}
}
