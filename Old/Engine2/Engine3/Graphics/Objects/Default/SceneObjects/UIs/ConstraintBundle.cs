using Engine.LinearAlgebra;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public class ConstraintBundle {

		internal ConstraintTransform Transform { get; private set; }
		private readonly HashSet<IConstraint> constraints;

		public ConstraintBundle() {
			Transform = new ConstraintTransform();
			constraints = new HashSet<IConstraint>();
		}

		public ConstraintBundle(params IConstraint[] constraints) {
			Transform = new ConstraintTransform();
			this.constraints = new HashSet<IConstraint>( constraints );
		}

		public void Add(IConstraint constraint ) {
			constraints.Add( constraint );
		}

		internal void Update( float time, UIElement e, GLWindow window ) {
			Transform.Reset();
			foreach( IConstraint c in constraints ) {
				c.Apply( Transform, time, e, window );
			}
		}
	}
}
