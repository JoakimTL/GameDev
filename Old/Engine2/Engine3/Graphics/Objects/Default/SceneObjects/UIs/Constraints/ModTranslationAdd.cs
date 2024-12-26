using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModTranslationAdd : IConstraint {

		public Vector2 Translation { get; set; }

		public ModTranslationAdd( Vector2 translation )  {
			this.Translation = translation;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Translation += Translation;
		}
	}
}
