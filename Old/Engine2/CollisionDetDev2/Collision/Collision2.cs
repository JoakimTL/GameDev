using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	public static class Collision2 {

		public static bool CheckCollision( PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB, out Collision2Result result, out bool collision, int maxIteration = 60 ) {
			int it = 0;
			Hull2 s = null;
			result = null;
			while( it < maxIteration ) {
				int r;
				if( ( r = PerformStep( ref s, shapeA, shapeB, it, out result, false ) ) != 0 ) {
					collision = s.Hit;
					return r == 1;
				}
				it++;
			}
			collision = s.Hit;
			return s.GJKDone;
		}

		public static int PerformStep( ref Hull2 simplex, PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB, int iteration, out Collision2Result result, bool log ) {
			if( simplex == null )
				simplex = new Hull2();
			result = null;

			if( simplex.GJKDone ) {
				return EPA2.PerformStep( simplex, shapeA, shapeB, iteration, out result, log );
			} else {
				int gjkResult = GJK2.PerformStep( simplex, shapeA, shapeB, log );
				if( gjkResult == 1 ) {
					simplex.SetHit( gjkResult == 1 );
					return 0;
				}
				if( gjkResult == -1 )
					return -1;
			}
			return 0;
		}

	}
}
