using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModColorAdd : IConstraint {

		public Vector4 Color { get; set; }

		public ModColorAdd( Vector4 col ) {
			this.Color = col;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Color += Color;
		}
	}
}
