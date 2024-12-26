using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModTransformSetAdd : IConstraint {

		public Vector2 Translation { get; set; }
		public Vector2 Scale { get; set; }

		public ModTransformSetAdd( Vector2 scaling, Vector2 translation )  {
			this.Scale = scaling;
			this.Translation = translation;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Scale = Scale;
			transform.Translation += Translation;
		}
	}
}
