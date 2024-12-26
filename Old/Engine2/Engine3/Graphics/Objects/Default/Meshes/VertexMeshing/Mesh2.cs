using Engine.LinearAlgebra;
using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.VertexMeshing {
	public class Mesh2 : VertexMesh<Vertex2> {

		public Mesh2( string name, IReadOnlyList<Vertex2> vertices, IReadOnlyList<int> indices ) : base( name ) {
			VBO = AllocateBuffer( vertices.Count * Vertex2.SIZEBYTES, BufferUsage.DynamicDraw, BufferTarget.ArrayBuffer );
			IBO = AllocateBuffer( indices.Count * sizeof( int ), BufferUsage.DynamicDraw, BufferTarget.ElementArrayBuffer );
			LoadVertices( VBO, 0, vertices );
			LoadIndices( IBO, 0, indices );

			Setup( VAO );
		}

		public override void LoadVertices( uint buffer, int offset, IReadOnlyList<Vertex2> list ) {
			byte[] data = new byte[ list.Count * Vertex2.SIZEBYTES ];
			unsafe {
				fixed( Vertex2* lP = list.ToArray() ) {
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
			Gl.EnableVertexAttribArray( 3 );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			Gl.VertexAttribPointer( 0, 3, VertexAttribType.Float, false, Vertex2.SIZEBYTES, (IntPtr) 0 );
			Gl.VertexAttribPointer( 1, 2, VertexAttribType.Float, false, Vertex2.SIZEBYTES, (IntPtr) ( 3 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 3, 4, VertexAttribType.UnsignedByte, true, Vertex2.SIZEBYTES, (IntPtr) ( 5 * sizeof( float ) ) );
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
	public struct Vertex2 {

		public const int SIZEBYTES = 24;

		[FieldOffset( 0 )]
		private Vector3 position;
		public Vector2 Position { get => position.XY; set => SetPosition( value ); }
		[FieldOffset( 12 )]
		private Vector2 uv;
		public Vector2 UV { get => uv; set => SetUV( value ); }
		[FieldOffset( 20 )]
		private Vector4b color;
		public Vector4b Color { get => color; set => SetColor( value ); }

		public Vertex2( Vector2 position, Vector2 uv ) {
			this.position = new Vector3( position, 0 );
			this.uv = uv;
			this.color = Vector4b.Byte;
		}

		public Vertex2( Vector2 position, Vector2 uv, Vector4i color ) {
			this.position = new Vector3( position, 0 );
			this.uv = uv;
			this.color = color.AsByte;
		}

		public Vertex2 SetPosition( Vector2 value ) {
			position = new Vector3( value, 0 );
			return this;
		}

		public Vertex2 SetColor( Vector4b value ) {
			color = value;
			return this;
		}

		public Vertex2 SetUV( Vector2 value ) {
			uv = value;
			return this;
		}
	}
}
