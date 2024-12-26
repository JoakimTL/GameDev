using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModColorInversion : IConstraint {
		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Color = new Vector4( 1 - transform.Color.XYZ, transform.Color.W );
		}
	}
}
