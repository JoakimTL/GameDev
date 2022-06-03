using System.Globalization;
using System.Numerics;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Standard.Meshing;

public static class VertexMeshUtilities {

	public static VertexMesh<V>? LoadMesh<V>( string filepath ) where V : unmanaged {
		byte[] fileData;
		try {
			fileData = File.ReadAllBytes( filepath );
			return new VertexMesh<V>( Path.GetFileName( filepath ), fileData );
		} catch ( Exception e ) {
			Log.Error( e );
		}
		return null;
	}

	public static class Mesh3 {

		public static VertexMesh<Vertex3> CreateNormalized( string name, Vertex3[] vertexArray, uint[] elementArray ) {
			Vertex3[] normalizedVertices = CalculateNormals( vertexArray, elementArray );
			return new VertexMesh<Vertex3>( name, normalizedVertices, elementArray );
		}

		public static void LoadObjFile( string filepath, out Vertex3[] vertices, out uint[] indices ) {
			static string[] RemoveEmptyStrings( string[] data ) {
				List<string> result = new();

				for ( int i = 0; i < data.Length; i++ )
					if ( !data[ i ].Equals( "" ) )
						result.Add( data[ i ] );

				return result.ToArray();
			}

			string[] splitName = filepath.Split( '\\' );
			string[] ext = splitName[ ^1 ].Split( '.' );

			if ( ext[ ^1 ].Equals( "obj" ) ) {

				List<Vertex3> tVertices = new();
				List<uint> tIndices = new();

				string? line;
				StreamReader file = new( $"{Directory.GetCurrentDirectory()}\\{filepath}" );
				while ( ( line = file.ReadLine() ) is not null ) {

					string[] tokens = RemoveEmptyStrings( line.Split( ' ' ) );

					if ( tokens.Length == 0 || tokens[ 0 ].Equals( "#" ) )
						continue;
					else if ( tokens[ 0 ].Equals( "v" ) )
						tVertices.Add(
							new Vertex3(
								new Vector3(
									float.Parse( tokens[ 1 ], CultureInfo.InvariantCulture.NumberFormat ),
									float.Parse( tokens[ 2 ], CultureInfo.InvariantCulture.NumberFormat ),
									float.Parse( tokens[ 3 ], CultureInfo.InvariantCulture.NumberFormat )
								)
							)
						);
					else if ( tokens[ 0 ].Equals( "f" ) ) {
						tIndices.Add( uint.Parse( tokens[ 1 ].Split( '/' )[ 0 ] ) - 1 );
						tIndices.Add( uint.Parse( tokens[ 2 ].Split( '/' )[ 0 ] ) - 1 );
						tIndices.Add( uint.Parse( tokens[ 3 ].Split( '/' )[ 0 ] ) - 1 );

						if ( tokens.Length > 4 ) {
							tIndices.Add( uint.Parse( tokens[ 1 ].Split( '/' )[ 0 ] ) - 1 );
							tIndices.Add( uint.Parse( tokens[ 3 ].Split( '/' )[ 0 ] ) - 1 );
							tIndices.Add( uint.Parse( tokens[ 4 ].Split( '/' )[ 0 ] ) - 1 );
						}
					}
				}

				file.Close();

				vertices = tVertices.ToArray();
				indices = tIndices.ToArray();

				Log.Line( $"Mesh Loaded: {Directory.GetCurrentDirectory()}\\{filepath}", Log.Level.NORMAL );
				return;
			} else
				Log.Warning( $"Could not load mesh file at {Directory.GetCurrentDirectory()}\\{filepath}" );

			vertices = Array.Empty<Vertex3>();
			indices = Array.Empty<uint>();
		}

		public static Vertex3[] CalculateNormals( Vertex3[] internalVertexList, uint[] internalIndexList ) {
			Vertex3[] newList = new Vertex3[ internalVertexList.Length ];
			for ( int i = 0; i < newList.Length; i++ ) {
				newList[ i ] = internalVertexList[ i ];
				Vertex3 n = newList[ i ];
				n = n.SetNormal( Vector3.Zero );
				newList[ i ] = n;
			}

			for ( int i = 0; i < internalIndexList.Length; i += 3 ) {
				uint i0 = internalIndexList[ i ];
				uint i1 = internalIndexList[ i + 1 ];
				uint i2 = internalIndexList[ i + 2 ];

				Vector3 v1 = newList[ i1 ].Translation - newList[ i0 ].Translation;
				Vector3 v2 = newList[ i2 ].Translation - newList[ i0 ].Translation;

				Vector3 normal = Vector3.Normalize( Vector3.Cross( v1, v2 ) );

				Vertex3 n0 = newList[ i0 ];
				n0 = n0.SetNormal( n0.Normal + normal );
				Vertex3 n1 = newList[ i1 ];
				n1 = n1.SetNormal( n1.Normal + normal );
				Vertex3 n2 = newList[ i2 ];
				n2 = n2.SetNormal( n2.Normal + normal );
				newList[ i0 ] = n0;
				newList[ i1 ] = n1;
				newList[ i2 ] = n2;
			}

			for ( int i = 0; i < newList.Length; i++ ) {
				Vertex3 n = newList[ i ];
				n = n.SetNormal( Vector3.Normalize( n.Normal ) );
				newList[ i ] = n;
			}

			return newList;
		}
	}
}
