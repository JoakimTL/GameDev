using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModColorTransparency : IConstraint {

		public float Alpha { get; set; }

		public ModColorTransparency( float a ) {
			this.Alpha = a;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Color *= new Vector4( 1, 1, 1, Alpha );
		}
	}
}
