using Engine.LinearAlgebra;
using Engine.Physics.D3.Shapes;
using System;
using System.Collections.Generic;

namespace CollisionDetDev3.Collision {
	internal class Collision3 {

		public static bool IsColliding( Shape3Polygon shape1, Shape3Polygon shape2, out Collision3Result result ) {
			int it = 0;
			int res;
			Simplex3 s = null;
			Simplex3Faces sf = null;
			while( ( res = PerformStep( ref s, ref sf, shape1, shape2, out result, false, it, out _, out _ ) ) == 0 && it < 50 ) {
				it++;
			}
			if( res > 0 )
				return true;
			return false;
		}

		public static bool IsColliding( ref Simplex3 s, ref Simplex3Faces faces, Shape3Polygon shape1, Shape3Polygon shape2, out Collision3Result result, out List<Simplex3> history, out List<Simplex3Faces> history2, bool log = false ) {
			int it = 0;
			int res;
			history = new List<Simplex3>();
			history2 = new List<Simplex3Faces>();
			Simplex3 his;
			Simplex3Faces his2;
			while( ( res = PerformStep( ref s, ref faces, shape1, shape2, out result, log, it, out his, out his2 ) ) == 0 && it < 50 ) {
				it++;
				history.Add( his );
				history2.Add( his2 );
			}
			history.Add( his );
			history2.Add( his2 );
			if( res > 0 )
				return true;
			return false;
		}

		internal static int PerformStep( ref Simplex3 simplex, ref Simplex3Faces faces, Shape3Polygon shape1, Shape3Polygon shape2, out Collision3Result res, bool log, int it, out Simplex3 his, out Simplex3Faces his2 ) {
			if( simplex is null )
				simplex = new Simplex3();

			res = null;
			his2 = null;
			if( simplex.Hit ) {
				int r = EPA3.PerformStep( simplex, ref faces, shape1, shape2, it, out res, out his2, log );
				his = new Simplex3();
				for( int i = 0; i < simplex.Count; i++ ) {
					his.AddSupport( simplex[ i ] );
				}
				his.SearchDirection = simplex.SearchDirection;
				his.SearchOrigin = simplex.SearchOrigin;
				return r;
			} else {
				int hit = GJK3.PerformStep( simplex, shape1, shape2, log, it, out his );
				if( hit == -1 )
					return -1;
				if (hit == 1 ) {
					simplex.SetHit();
					return 0;
				}
			}
			return 0;
		}
	}
}