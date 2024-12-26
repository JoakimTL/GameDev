using Engine.LMath;
using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.VertexMeshing {
	public class Mesh3 : VertexMesh<Vertex3> {

		private int Size;

		public Mesh3( string name, IReadOnlyList<Vertex3> vertices, IReadOnlyList<int> indices, bool calculateNormals = true ) : base( name ) {
			if( calculateNormals )
				Mesh3Utils.CalculateNormals( vertices, indices );
			VBO = AllocateBuffer( vertices.Count * Vertex3.SIZE * sizeof( float ), StorageType.VERTEXDATA, BufferUsage.DynamicDraw );
			IBO = AllocateBuffer( indices.Count * sizeof( int ), StorageType.INDEXDATA, BufferUsage.DynamicDraw );
			LoadVertices( VBO, 0, vertices );
			LoadInts( IBO, 0, indices );

			Size = indices.Count;
			Setup( VAO );
		}

		public Mesh3( string name, string filepath ) : base( name ) {
			Mesh3Utils.LoadFile( filepath, out Vertex3[] vertices, out int[] indices );
			VBO = AllocateBuffer( vertices.Length * Vertex3.SIZE * sizeof( float ), StorageType.VERTEXDATA, BufferUsage.DynamicDraw );
			IBO = AllocateBuffer( indices.Length * sizeof( int ), StorageType.INDEXDATA, BufferUsage.DynamicDraw );
			LoadVertices( VBO, 0, vertices );
			LoadInts( IBO, 0, indices );

			Size = indices.Length;
			Setup( VAO );
		}

		public override void Setup( uint VAO ) {
			if( VAO > 0 )
				Gl.BindVertexArray( VAO );
			Gl.EnableVertexAttribArray( 0 );
			Gl.EnableVertexAttribArray( 1 );
			Gl.EnableVertexAttribArray( 2 );
			Gl.EnableVertexAttribArray( 3 );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			Gl.VertexAttribPointer( 0, 3, VertexAttribType.Float, false, Vertex3.SIZE * sizeof( float ), (IntPtr) ( 0 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 1, 3, VertexAttribType.Float, false, Vertex3.SIZE * sizeof( float ), (IntPtr) ( 3 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 2, 3, VertexAttribType.Float, false, Vertex3.SIZE * sizeof( float ), (IntPtr) ( 6 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 3, 2, VertexAttribType.Float, false, Vertex3.SIZE * sizeof( float ), (IntPtr) ( 9 * sizeof( float ) ) );
			Gl.BindBuffer( BufferTarget.ElementArrayBuffer, IBO );
			if( VAO > 0 )
				Gl.BindVertexArray( 0 );
		}

		public override void Bind() {
			Gl.BindVertexArray( VAO );
		}

		public override void RenderMesh() {
			Gl.DrawElements( PrimitiveType.Triangles, Size, DrawElementsType.UnsignedInt, (IntPtr) ( DataSegments[IBO].Offset ) );
		}

		public override void Unbind() {
			Gl.BindVertexArray( 0 );
		}
	}

	public class Vertex3 : IVertex {

		public const int SIZE = 11;

		public Vector3 Position;
		public Vector3 Color;
		public Vector3 Normal;
		public Vector2 UV;

		public Vertex3() {
			Position = Vector3.Zero;
			Color = Vector3.One;
			Normal = Vector3.Zero;
			UV = Vector2.Zero;
		}

		public Vertex3 SetPosition( Vector3 value ) {
			Position = value;
			return this;
		}

		public Vertex3 SetColor( Vector3 value ) {
			Color = value;
			return this;
		}

		public Vertex3 SetNormal( Vector3 value ) {
			Normal = value;
			return this;
		}

		public Vertex3 SetUV( Vector2 value ) {
			UV = value;
			return this;
		}

		public void AddVertex( List<float> list ) {
			list.Add( Position.X );
			list.Add( Position.Y );
			list.Add( Position.Z );
			list.Add( Color.X );
			list.Add( Color.Y );
			list.Add( Color.Z );
			list.Add( Normal.X );
			list.Add( Normal.Y );
			list.Add( Normal.Z );
			list.Add( UV.X );
			list.Add( UV.Y );
		}

		public void AddVertex( float[] arr, int index ) {
			int i = index * SIZE;
			arr[ i++ ] = ( Position.X );
			arr[ i++ ] = ( Position.Y );
			arr[ i++ ] = ( Position.Z );
			arr[ i++ ] = ( Color.X );
			arr[ i++ ] = ( Color.Y );
			arr[ i++ ] = ( Color.Z );
			arr[ i++ ] = ( Normal.X );
			arr[ i++ ] = ( Normal.Y );
			arr[ i++ ] = ( Normal.Z );
			arr[ i++ ] = ( UV.X );
			arr[ i++ ] = ( UV.Y );
		}

		public int GetSize() {
			return SIZE;
		}
	}

	public static class Mesh3Utils {
		public static void LoadFile( string filepath, out Vertex3[] vertices, out int[] indices ) {
			static string[] RemoveEmptyStrings( string[] data ) {
				List<string> result = new List<string>();

				for( int i = 0; i < data.Length; i++ )
					if( !data[ i ].Equals( "" ) )
						result.Add( data[ i ] );

				return result.ToArray();
			}

			string[] splitName = filepath.Split( '\\' );
			string[] ext = splitName[ ^1 ].Split( '.' );

			if( ext[ ^1 ].Equals( "obj" ) ) {

				List<Vertex3> tVertices = new List<Vertex3>();
				List<int> tIndices = new List<int>();

				Mem.Logs.Routine.WriteLine( $"Mesh Loaded: {Directory.GetCurrentDirectory()}\\{filepath}" );

				string line;
				StreamReader file = new StreamReader( $"{Directory.GetCurrentDirectory()}\\{filepath}" );
				while( ( line = file.ReadLine() ) != null ) {

					string[] tokens = RemoveEmptyStrings( line.Split( ' ' ) );

					if( tokens.Length == 0 || tokens[ 0 ].Equals( "#" ) ) {
						continue;
					} else if( tokens[ 0 ].Equals( "v" ) ) {
						tVertices.Add(
							new Vertex3()
							.SetPosition(
								new Vector3(
									float.Parse( tokens[ 1 ], CultureInfo.InvariantCulture.NumberFormat ),
									float.Parse( tokens[ 2 ], CultureInfo.InvariantCulture.NumberFormat ),
									float.Parse( tokens[ 3 ], CultureInfo.InvariantCulture.NumberFormat )
									)
								)
						);
					} else if( tokens[ 0 ].Equals( "f" ) ) {
						tIndices.Add( int.Parse( tokens[ 1 ].Split( '/' )[ 0 ] ) - 1 );
						tIndices.Add( int.Parse( tokens[ 2 ].Split( '/' )[ 0 ] ) - 1 );
						tIndices.Add( int.Parse( tokens[ 3 ].Split( '/' )[ 0 ] ) - 1 );

						if( tokens.Length > 4 ) {
							tIndices.Add( int.Parse( tokens[ 1 ].Split( '/' )[ 0 ] ) - 1 );
							tIndices.Add( int.Parse( tokens[ 3 ].Split( '/' )[ 0 ] ) - 1 );
							tIndices.Add( int.Parse( tokens[ 4 ].Split( '/' )[ 0 ] ) - 1 );
						}
					}
				}

				file.Close();

				vertices = tVertices.ToArray();
				indices = tIndices.ToArray();

				return;
			} else
				Mem.Logs.Error.WriteLine( $"Could not load mesh file at {Directory.GetCurrentDirectory()}\\{filepath}" );

			vertices = new Vertex3[ 0 ];
			indices = new int[ 0 ];
		}

		public static void CalculateNormals( IReadOnlyList<Vertex3> internalVertexList, IReadOnlyList<int> internalIndexList ) {
			for( int i = 0; i < internalVertexList.Count; i++ )
				internalVertexList[ i ].Normal = new Vector3( 0, 0, 0 );

			for( int i = 0; i < internalIndexList.Count; i += 3 ) {
				int i0 = internalIndexList[ i ];
				int i1 = internalIndexList[ i + 1 ];
				int i2 = internalIndexList[ i + 2 ];

				Vector3 v1 = internalVertexList[ i1 ].Position - internalVertexList[ i0 ].Position;
				Vector3 v2 = internalVertexList[ i2 ].Position - internalVertexList[ i0 ].Position;

				Vector3 normal = Vector3.Cross( v1, v2 );
				normal.Normalize();

				internalVertexList[ i0 ].Normal += normal;
				internalVertexList[ i1 ].Normal += normal;
				internalVertexList[ i2 ].Normal += normal;
			}

			for( int i = 0; i < internalVertexList.Count; i++ )
				internalVertexList[ i ].Normal.Normalize();
		}
	}
}
