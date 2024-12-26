using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModTranslationAddAligmentVertical : IConstraint {
		
		public VerticalAlignment Alignment { get; set; }
		public bool AccountForAspect { get; set; }

		public ModTranslationAddAligmentVertical( VerticalAlignment alignment, bool aspect ) {
			this.Alignment = alignment;
			this.AccountForAspect = aspect;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			if( AccountForAspect )
				transform.Translation += new Vector2( 0, window.AspectRatioVector.Y * (int) Alignment );
			else
				transform.Translation += new Vector2( 0, (int) Alignment );
		}
	}
}
