using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModTranslationSetAligmentHorizontal : IConstraint {
		
		public HorizontalAlignment Alignment { get; set; }
		public bool AccountForAspect { get; set; }

		public ModTranslationSetAligmentHorizontal( HorizontalAlignment alignment, bool aspect ) {
			this.Alignment = alignment;
			this.AccountForAspect = aspect;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			if( AccountForAspect )
				transform.Translation = new Vector2( window.AspectRatioVector.X * (int) Alignment, transform.Translation.Y );
			else
				transform.Translation = new Vector2( (int) Alignment, transform.Translation.Y );
		}
	}
}
