using Engine.LinearAlgebra;
using Engine.Physics;
using Engine.Physics.D2.Shapes;
using Engine.Physics.D3.Shapes;
using Engine.Utilities.Tools;
using System.Collections.Generic;

namespace Engine.MemLib {
	public class SampleCollisionData {

		public Shape3Polygon.Mold CubeUniform { get; private set; }
		public Shape3Polygon.Mold IcosphereUniform { get; private set; }
		/*public ShapePolygonData BoxY { get; private set; }
		public ShapePolygonData BoxZ { get; private set; }*/
		public Shape2Polygon.Mold SquareUniform { get; private set; }

		public SampleCollisionData() {
			CubeUniform = new Shape3Polygon.Mold( new List<PointMass<Vector3>>() {
				new PointMass<Vector3>( (-1, -1, -1), 1 ),
				new PointMass<Vector3>( (1, -1, -1 ), 1 ),
				new PointMass<Vector3>( (1, 1, -1 ), 1 ),
				new PointMass<Vector3>( (-1, 1, -1 ), 1 ),

				new PointMass<Vector3>( (-1, -1, 1 ), 1 ),
				new PointMass<Vector3>( (1, -1, 1 ), 1 ),
				new PointMass<Vector3>( (1, 1, 1 ), 1 ),
				new PointMass<Vector3>( (-1, 1, 1 ), 1 )
			} );

			MeshGenerator.GenerateIcosphereVectors( 3, out List<Vector3> vecs, out _ );
			IcosphereUniform = new Shape3Polygon.Mold( vecs );

			/*BoxY = Add( new ShapePolygonData( "boxY", new List<List<Vector3>>() {
				new List<Vector3>() {
					new Vector3( -1, 0, -1 ), new Vector3( 1, 0, -1 ), new Vector3( 1, 0, -1 ), new Vector3( -1, 0, -1 ),
					new Vector3( -1, 0, 1 ), new Vector3( 1, 0, 1 ), new Vector3( 1, 0, 1 ), new Vector3( -1, 0, 1 )
				}
			} ) );

			BoxZ = Add( new ShapePolygonData( "boxZ", new List<List<Vector3>>() {
				new List<Vector3>() {
					new Vector3( -1, -1, 0 ), new Vector3( 1, -1, 0 ), new Vector3( -1, 1, 0 ), new Vector3( 1, 1, 0 ),
					new Vector3( -1, -1, 1 ), new Vector3( 1, -1, 1 ), new Vector3( -1, 1, 1 ), new Vector3( 1, 1, 1 )
				}
			} ) );*/

			SquareUniform = new Shape2Polygon.Mold( new List<PointMass<Vector2>>() {
				new PointMass<Vector2>( ( -1, -1 ), 1 ),
				new PointMass<Vector2>( ( 1, -1 ), 1 ),
				new PointMass<Vector2>( ( 1, 1 ), 1 ),
				new PointMass<Vector2>( ( -1, 1 ), 1 )
			} );
		}
	}
}
