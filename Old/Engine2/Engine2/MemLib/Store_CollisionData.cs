namespace Engine.MemLib {
	/*public class Store_CollisionData {

		public ShapePolygonData Cube { get; private set; }
		public ShapePolygonData BoxY { get; private set; }
		public ShapePolygonData BoxZ { get; private set; }
		public ShapePolygonData Square { get; private set; }

		private Dictionary<string, ShapePolygonData> spdDictionary;

		public Store_CollisionData() {
			spdDictionary = new Dictionary<string, ShapePolygonData>();

			Cube = Add( new ShapePolygonData( "cube", new List<List<Vector3>>() {
				new List<Vector3>() {
					new Vector3( -1, -1, -1 ), new Vector3( 1, -1, -1 ), new Vector3( 1, 1, -1 ), new Vector3( -1, 1, -1 ),
					new Vector3( -1, -1, 1 ), new Vector3( 1, -1, 1 ), new Vector3( 1, 1, 1 ), new Vector3( -1, 1, 1 )
				}
			} ) );
			BoxY = Add( new ShapePolygonData( "boxY", new List<List<Vector3>>() {
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
			} ) );

			Square = Add( new ShapePolygonData( "square", new List<List<Vector3>>() {
				new List<Vector3>() {
					new Vector3( -1, -1, 0 ), new Vector3( 1, -1, 0), new Vector3( 1, 1, 0), new Vector3( -1, 1, 0 )
				}
			} ) );
		}

		public ShapePolygonData Add( ShapePolygonData d ) {
			if( !( d is null ) && spdDictionary.ContainsKey( d.Name ) )
				spdDictionary.Add( d.Name, d );
			return d;
		}

		public ShapePolygonData Get( string key ) {
			if( spdDictionary.TryGetValue( key, out ShapePolygonData spd ) )
				return spd;
			return null;
		}
	}*/
}
