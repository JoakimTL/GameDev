using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.VertexMeshing {
	public class LineMesh2 : VertexMesh<LineVertex2> {

		public float Width { get; private set; }

		public LineMesh2( string name, IReadOnlyList<LineVertex2> vertices, IReadOnlyList<int> indices ) : base( name ) {
			VBO = AllocateBuffer( vertices.Count * LineVertex2.SIZEBYTES, BufferUsage.DynamicDraw, BufferTarget.ArrayBuffer );
			IBO = AllocateBuffer( indices.Count * sizeof( int ), BufferUsage.DynamicDraw, BufferTarget.ElementArrayBuffer );
			LoadVertices( VBO, 0, vertices );
			LoadIndices( IBO, 0, indices );

			Setup( VAO );
		}

		public override void LoadVertices( uint buffer, int offset, IReadOnlyList<LineVertex2> list ) {
			byte[] data = new byte[ list.Count * LineVertex2.SIZEBYTES ];
			unsafe {
				fixed( LineVertex2* lP = list.ToArray() ) {
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
			Gl.VertexAttribPointer( 0, 2, VertexAttribType.Float, false, LineVertex2.SIZEBYTES, (IntPtr) 0 );//v
			Gl.VertexAttribPointer( 3, 4, VertexAttribType.UnsignedByte, true, LineVertex2.SIZEBYTES, (IntPtr) 8 );//c
			Gl.BindBuffer( BufferTarget.ElementArrayBuffer, IBO );
			if( VAO > 0 )
				Gl.BindVertexArray( 0 );
		}

		public void SetWidth( float width ) {
			if( width <= 0 )
				return;
			Width = width;
		}

		public override void Bind() {
			Gl.BindVertexArray( VAO );
		}

		public override void RenderMesh() {
			Gl.LineWidth( Width );
			Gl.DrawElements( PrimitiveType.Lines, Size, DrawElementsType.UnsignedInt, (IntPtr) 0 );
		}

		public override void Unbind() {
			Gl.BindVertexArray( 0 );
		}
	}

	[StructLayout( LayoutKind.Explicit )]
	public struct LineVertex2 {

		public const int SIZEBYTES = 12;

		[FieldOffset( 0 )]
		private Vector2 position;
		public Vector2 Position { get => position; set => SetPosition( value ); }
		[FieldOffset( 8 )]
		private Vector4b color;
		public Vector4b Color { get => color; set => SetColor( value ); }

		public LineVertex2( Vector2 position ) {
			this.position = position;
			this.color = Vector4b.Byte;
		}

		public LineVertex2( Vector2 position, Vector4i color ) {
			this.position = position;
			this.color = color.AsByte;
		}

		public LineVertex2 SetPosition( Vector2 value ) {
			position = value;
			return this;
		}

		public LineVertex2 SetColor( Vector4b value ) {
			color = value;
			return this;
		}
	}
}
