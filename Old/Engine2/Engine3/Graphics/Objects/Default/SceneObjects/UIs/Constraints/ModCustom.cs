using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModCustom : IConstraint {

		public ConstraintModifierHandler Application { get; set; }

		public ModCustom( ConstraintModifierHandler app ) {
			this.Application = app;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			Application?.Invoke( transform, time, e, window );
		}
	}
}
