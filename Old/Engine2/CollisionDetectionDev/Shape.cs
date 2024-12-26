using Engine.LMath;
using System.Collections.Generic;
using System.Drawing;

namespace CollisionDetectionDev {
	internal class Shape {

		public List<Vector2> points;
		public Matrix4 transform;

		public Color color;

		public Shape( Color color ) {
			this.color = color;
			transform = Matrix4.Identity;
			points = new List<Vector2>();
		}

		public Vector2 GetTransformed( int index ) {
			return Vector3.Transform( new Vector3( points[ index ], 0 ), transform ).XY;
		}

		public Vector2 GetFurthest( Vector2 dir ) {
			if( points.Count == 0 )
				return 0;
			Vector2 furthest = GetTransformed( 0 );
			float dot = Vector2.Dot( dir, furthest );
			for( int i = 1; i < points.Count; i++ ) {
				Vector2 tP = GetTransformed( i );
				float tDot = Vector2.Dot( dir, tP );
				if( tDot > dot ) {
					furthest = tP;
					dot = tDot;
				}
			}
			return furthest;
		}

	}
}