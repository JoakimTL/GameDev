using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.D2 {
	public class Physics2Model : PhysicsModel<Transform2, Vector2> {

		private Vector2 min;
		public override Vector2 MinAABB => min;

		private Vector2 max;
		public override Vector2 MaxAABB => max;

		public Physics2Model( string name, Transform2 transform ) : base( name, transform ) { }

		public override Vector2 GetCenterOfMass() {
			Vector2 accumulatedOffset = new Vector2();
			float totalMass = 0;
			foreach( PhysicsShape<Transform2, Vector2> shape in Shapes ) {
				accumulatedOffset += shape.CenterOfMass * shape.MassProportion;
				totalMass += shape.MassProportion;
			}
			return accumulatedOffset / totalMass;
		}

		public override void UpdateAABB() {
			min = max = 0;
			if( ShapeCount == 0 )
				return;
			Shapes[ 0 ].GetAABB( out min, out max );
			for( int i = 1; i < ShapeCount; i++ ) {
				Shapes[ i ].GetAABB( out Vector2 nmin, out Vector2 nmax );
				min.X = Math.Min( min.X, nmin.X );
				min.Y = Math.Min( min.Y, nmin.Y );
				max.X = Math.Max( max.X, nmax.X );
				max.Y = Math.Max( max.Y, nmax.Y );
			}
		}
	}
}
