using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModTranslationAddPixel : IConstraint {

		public Vector2 Translation { get; set; }

		public ModTranslationAddPixel( Vector2i translation ) {
			this.Translation = translation.AsFloat;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			transform.Translation += new Vector2( Translation.X / window.Size.X * window.AspectRatioVector.X * 2, Translation.Y / window.Size.Y * window.AspectRatioVector.Y * 2 );
		}
	}
}
