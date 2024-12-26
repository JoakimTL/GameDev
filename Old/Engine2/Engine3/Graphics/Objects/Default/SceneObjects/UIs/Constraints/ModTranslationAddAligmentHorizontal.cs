using Engine.LinearAlgebra;
using Engine.Utilities.Time;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints {
	public class ModTranslationAddAligmentHorizontal : IConstraint {

		public HorizontalAlignment Alignment { get; set; }
		public bool AccountForAspect { get; set; }

		public ModTranslationAddAligmentHorizontal( HorizontalAlignment alignment, bool aspect ) {
			this.Alignment = alignment;
			this.AccountForAspect = aspect;
		}

		public void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window ) {
			if( AccountForAspect )
				transform.Translation += new Vector2( window.AspectRatioVector.X * (int) Alignment, 0 );
			else
				transform.Translation += new Vector2( (int) Alignment, 0 );
		}
	}
}
