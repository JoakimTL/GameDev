using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Data;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.VertexMeshing {
	public class Mesh3 : VertexMesh<Vertex3> {

		public Mesh3( string name, IReadOnlyList<Vertex3> vertices, IReadOnlyList<int> indices, bool calculateNormals = true ) : base( name ) {
			if( calculateNormals )
				vertices = Mesh3Utils.CalculateNormals( vertices, indices );
			VBO = AllocateBuffer( vertices.Count * Vertex3.SIZEBYTES, BufferUsage.DynamicDraw, BufferTarget.ArrayBuffer );
			IBO = AllocateBuffer( indices.Count * sizeof( int ), BufferUsage.DynamicDraw, BufferTarget.ElementArrayBuffer );
			LoadVertices( VBO, 0, vertices );
			LoadIndices( IBO, 0, indices );

			Setup( VAO );
		}

		public Mesh3( string name, string filepath, bool calculateNormals = true ) : base( name ) {
			Mesh3Utils.LoadFile( filepath, out Vertex3[] vertices, out int[] indices );
			if( calculateNormals )
				vertices = Mesh3Utils.CalculateNormals( vertices, indices );
			VBO = AllocateBuffer( vertices.Length * Vertex3.SIZEBYTES, BufferUsage.DynamicDraw, BufferTarget.ArrayBuffer );
			IBO = AllocateBuffer( indices.Length * sizeof( int ), BufferUsage.DynamicDraw, BufferTarget.ElementArrayBuffer );
			LoadVertices( VBO, 0, vertices );
			LoadIndices( IBO, 0, indices );

			Setup( VAO );
		}

		public override void LoadVertices( uint buffer, int offset, IReadOnlyList<Vertex3> list ) {
			byte[] data = new byte[ list.Count * Vertex3.SIZEBYTES ];
			unsafe {
				fixed( Vertex3* lP = list.ToArray() ) {
					Marshal.Copy( (IntPtr) lP, data, 0, data.Length );
				}
			}
			LoadBytes( buffer, offset, data );
		}

		public override void Setup( uint VAO ) {
			if( VAO > 0 )
				Gl.BindVertexArray( VAO );
			Gl.EnableVertexAttribArray( 0 );
			Gl.EnableVertexAttribArray( 1 );
			Gl.EnableVertexAttribArray( 2 );
			Gl.EnableVertexAttribArray( 3 );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			Gl.VertexAttribPointer( 0, 3, VertexAttribType.Float, false, Vertex3.SIZEBYTES, (IntPtr) 0 );//v
			Gl.VertexAttribPointer( 1, 2, VertexAttribType.Float, false, Vertex3.SIZEBYTES, (IntPtr) 12 );//uv
			Gl.VertexAttribPointer( 2, 3, VertexAttribType.Float, false, Vertex3.SIZEBYTES, (IntPtr) 20 );//n
			Gl.VertexAttribPointer( 3, 4, VertexAttribType.UnsignedByte, true, Vertex3.SIZEBYTES, (IntPtr) 32 );//c
			Gl.BindBuffer( BufferTarget.ElementArrayBuffer, IBO );
			if( VAO > 0 )
				Gl.BindVertexArray( 0 );
		}

		public override void Bind() {
			Gl.BindVertexArray( VAO );
		}

		public override void RenderMesh() {
			Gl.DrawElements( PrimitiveType.Triangles, Size, DrawElementsType.UnsignedInt, (IntPtr) 0 );
		}

		public override void Unbind() {
			Gl.BindVertexArray( 0 );
		}
	}

	[StructLayout( LayoutKind.Explicit )]
	public struct Vertex3 {

		public const int SIZEBYTES = 36;

		[FieldOffset( 0 )]
		private Vector3 position;
		public Vector3 Position { get => position; set => SetPosition( value ); }
		[FieldOffset( 12 )]
		private Vector2 uv;
		public Vector2 UV { get => uv; set => SetUV( value ); }
		[FieldOffset( 20 )]
		private Vector3 normal;
		public Vector3 Normal { get => normal; set => SetNormal( value ); }
		[FieldOffset( 32 )]
		private Vector4b color;
		public Vector4b Color { get => color; set => SetColor( value ); }

		public Vertex3( Vector3 position ) {
			this.position = position;
			this.uv = Vector2.Zero;
			this.normal = Vector3.Zero;
			this.color = Vector4b.Byte;
		}

		public Vertex3( Vector3 position, Vector2 uv ) {
			this.position = position;
			this.uv = uv;
			this.normal = Vector3.Zero;
			this.color = Vector4b.Byte;
		}

		public Vertex3( Vector3 position, Vector2 uv, Vector3 normal ) {
			this.position = position;
			this.uv = uv;
			this.normal = normal;
			this.color = Vector4b.Byte;
		}

		public Vertex3( Vector3 position, Vector2 uv, Vector3 normal, Vector4i color ) {
			this.position = position;
			this.uv = uv;
			this.normal = normal;
			this.color = color.AsByte;
		}

		public Vertex3 SetPosition( Vector3 value ) {
			position = value;
			return this;
		}

		public Vertex3 SetUV( Vector2 value ) {
			uv = value;
			return this;
		}

		public Vertex3 SetNormal( Vector3 value ) {
			normal = value;
			return this;
		}

		public Vertex3 SetColor( Vector4b value ) {
			color = value;
			return this;
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

				string line;
				StreamReader file = new StreamReader( $"{Directory.GetCurrentDirectory()}\\{filepath}" );
				while( ( line = file.ReadLine() ) != null ) {

					string[] tokens = RemoveEmptyStrings( line.Split( ' ' ) );

					if( tokens.Length == 0 || tokens[ 0 ].Equals( "#" ) ) {
						continue;
					} else if( tokens[ 0 ].Equals( "v" ) ) {
						tVertices.Add(
							new Vertex3(
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

				Logging.Routine( $"Mesh Loaded: {Directory.GetCurrentDirectory()}\\{filepath}" );
				return;
			} else
				Mem.Logs.Error.WriteLine( $"Could not load mesh file at {Directory.GetCurrentDirectory()}\\{filepath}" );

			vertices = new Vertex3[ 0 ];
			indices = new int[ 0 ];
		}

		public static Vertex3[] CalculateNormals( IReadOnlyList<Vertex3> internalVertexList, IReadOnlyList<int> internalIndexList ) {
			Vertex3[] newList = new Vertex3[ internalVertexList.Count ];
			for( int i = 0; i < newList.Length; i++ ) {
				newList[ i ] = internalVertexList[ i ];
				Vertex3 n = newList[ i ];
				n.Normal = 0;
				newList[ i ] = n;
			}

			for( int i = 0; i < internalIndexList.Count; i += 3 ) {
				int i0 = internalIndexList[ i ];
				int i1 = internalIndexList[ i + 1 ];
				int i2 = internalIndexList[ i + 2 ];

				Vector3 v1 = newList[ i1 ].Position - newList[ i0 ].Position;
				Vector3 v2 = newList[ i2 ].Position - newList[ i0 ].Position;

				Vector3 normal = Vector3.Cross( v1, v2 );
				normal.Normalize();

				Vertex3 n0 = newList[ i0 ];
				n0.Normal += normal;
				Vertex3 n1 = newList[ i1 ];
				n1.Normal += normal;
				Vertex3 n2 = newList[ i2 ];
				n2.Normal += normal;
				newList[ i0 ] = n0;
				newList[ i1 ] = n1;
				newList[ i2 ] = n2;
			}

			for( int i = 0; i < newList.Length; i++ ) {
				Vertex3 n = newList[ i ];
				n.Normal = n.Normal.Normalized;
				newList[ i ] = n;
			}

			return newList;
		}
	}
}
