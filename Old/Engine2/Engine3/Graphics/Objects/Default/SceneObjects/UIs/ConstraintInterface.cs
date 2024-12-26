using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public interface IConstraint {
		void Apply( ConstraintTransform transform, float time, UIElement e, GLWindow window );
	}
}
