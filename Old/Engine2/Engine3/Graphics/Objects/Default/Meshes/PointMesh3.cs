using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.VertexMeshing {
	public class PointMesh3 : Mesh {

		public uint VBO { get; protected set; }
		public float Radius { get; private set; }
		protected int Size { get; private set; }

		public PointMesh3( string name, IReadOnlyList<PointVertex3> vertices ) : base( name ) {
			VBO = AllocateBuffer( vertices.Count * PointVertex3.SIZEBYTES, BufferUsage.DynamicDraw, BufferTarget.ArrayBuffer );
			LoadVertices( VBO, 0, vertices );
			Size = vertices.Count;

			Radius = 2;

			Setup( VAO );
		}

		public PointMesh3( string name ) : base( name ) {
			Radius = 2;
		}

		public void Create( IReadOnlyList<PointVertex3> vertices ) {
			VBO = AllocateBuffer( vertices.Count * PointVertex3.SIZEBYTES, BufferUsage.DynamicDraw, BufferTarget.ArrayBuffer );
			LoadVertices( VBO, 0, vertices );
			Size = vertices.Count;
			Setup( VAO );
		}

		public void LoadVertices( uint buffer, int offset, IReadOnlyList<PointVertex3> list ) {
			byte[] data = new byte[ list.Count * PointVertex3.SIZEBYTES ];
			unsafe {
				fixed( PointVertex3* lP = list.ToArray() ) {
					Marshal.Copy( (IntPtr) lP, data, 0, data.Length );
				}
			}
			LoadBytes( buffer, offset, data );
		}

		public override void Setup( uint VAO ) {
			if( VAO > 0 )
				Gl.BindVertexArray( VAO );
			Gl.EnableVertexAttribArray( 0 );
			Gl.EnableVertexAttribArray( 3 );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			Gl.VertexAttribPointer( 0, 3, VertexAttribType.Float, false, PointVertex3.SIZEBYTES, (IntPtr) 0 );//v
			Gl.VertexAttribPointer( 3, 4, VertexAttribType.UnsignedByte, true, PointVertex3.SIZEBYTES, (IntPtr) 12 );//c
			if( VAO > 0 )
				Gl.BindVertexArray( 0 );
		}

		public void SetRadius( float radius ) {
			if( radius <= 0 )
				return;
			Radius = radius;
		}

		public override void Bind() {
			Gl.BindVertexArray( VAO );
		}

		public override void RenderMesh() {
			Gl.PointSize( Radius );
			Gl.DrawArrays( PrimitiveType.Points, 0, Size );
		}

		public override void Unbind() {
			Gl.BindVertexArray( 0 );
		}
	}

	[StructLayout( LayoutKind.Explicit )]
	public struct PointVertex3 {

		public const int SIZEBYTES = 16;

		[FieldOffset( 0 )]
		private Vector3 position;
		public Vector3 Position { get => position; set => SetPosition( value ); }
		[FieldOffset( 12 )]
		private Vector4b color;
		public Vector4b Color { get => color; set => SetColor( value ); }

		public PointVertex3( Vector3 position ) {
			this.position = position;
			this.color = Vector4b.Byte;
		}

		public PointVertex3( Vector3 position, Vector4i color ) {
			this.position = position;
			this.color = color.AsByte;
		}

		public PointVertex3 SetPosition( Vector3 value ) {
			position = value;
			return this;
		}

		public PointVertex3 SetColor( Vector4b value ) {
			color = value;
			return this;
		}
	}
}
