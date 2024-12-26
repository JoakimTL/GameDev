using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModTranslationSetAligmentVertical : IConstraint {
		
		public VerticalAlignment Alignment { get; set; }
		public bool AccountForAspect { get; set; }

		public ModTranslationSetAligmentVertical( VerticalAlignment alignment, bool aspect ) {
			this.Alignment = alignment;
			this.AccountForAspect = aspect;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			if( AccountForAspect )
				transform.Translation = new Vector2( transform.Translation.X, window.AspectRatioVector.Y * (int) Alignment );
			else
				transform.Translation = new Vector2( transform.Translation.X, (int) Alignment );
		}
	}
}
