using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.D3 {
	public class Physics3Model : PhysicsModel<Transform3, Vector3> {

		private Vector3 min;
		public override Vector3 MinAABB => min;

		private Vector3 max;
		public override Vector3 MaxAABB => max;

		public Physics3Model( string name, Transform3 transform ) : base( name, transform ) { }

		public override Vector3 GetCenterOfMass() {
			Vector3 accumulatedOffset = new Vector3();
			float totalMass = 0;
			foreach( PhysicsShape<Transform3, Vector3> shape in Shapes ) {
				accumulatedOffset += shape.CenterOfMass * shape.MassProportion;
				totalMass += shape.MassProportion;
			}
			return accumulatedOffset / Math.Max( totalMass, 1 );
		}

		public override void UpdateAABB() {
			min = max = 0;
			if( ShapeCount == 0 )
				return;
			Shapes[ 0 ].GetAABB( out min, out max );
			for( int i = 1; i < ShapeCount; i++ ) {
				Shapes[ i ].GetAABB( out Vector3 nmin, out Vector3 nmax );
				min = Vector3.Min( min, nmin );
				max = Vector3.Max( max, nmax );
			}
		}
	}
}
