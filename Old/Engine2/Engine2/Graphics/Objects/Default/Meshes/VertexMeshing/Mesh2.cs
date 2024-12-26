using Engine.LMath;
using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.VertexMeshing {
	public class Mesh2 : VertexMesh<Vertex2> {

		private int Size;

		public Mesh2( string name, IReadOnlyList<Vertex2> vertices, IReadOnlyList<int> indices ) : base( name ) {
			VBO = AllocateBuffer( vertices.Count * Vertex2.SIZE * sizeof( float ), StorageType.VERTEXDATA, BufferUsage.DynamicDraw );
			IBO = AllocateBuffer( indices.Count * sizeof( int ), StorageType.INDEXDATA, BufferUsage.DynamicDraw );
			LoadVertices( VBO, 0, vertices );
			LoadInts( IBO, 0, indices );

			Size = indices.Count;
			Setup( VAO );
		}

		public override void Setup( uint VAO ) {
			if( VAO > 0 )
				Gl.BindVertexArray( VAO );
			Gl.EnableVertexAttribArray( 0 );
			Gl.EnableVertexAttribArray( 1 );
			Gl.EnableVertexAttribArray( 3 );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			Gl.VertexAttribPointer( 0, 2, VertexAttribType.Float, false, Vertex2.SIZE * sizeof( float ), (IntPtr) ( 0 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 1, 3, VertexAttribType.Float, false, Vertex2.SIZE * sizeof( float ), (IntPtr) ( 2 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 3, 2, VertexAttribType.Float, false, Vertex2.SIZE * sizeof( float ), (IntPtr) ( 6 * sizeof( float ) ) );
			Gl.BindBuffer( BufferTarget.ElementArrayBuffer, IBO );
			if( VAO > 0 )
				Gl.BindVertexArray( 0 );
		}

		public override void Bind() {
			Gl.BindVertexArray( VAO );
		}

		public override void RenderMesh() {
			Gl.DrawElements( PrimitiveType.Triangles, Size, DrawElementsType.UnsignedInt, (IntPtr) ( DataSegments[ IBO ].Offset ) );
		}

		public override void Unbind() {
			Gl.BindVertexArray( 0 );
		}

	}

	public class Vertex2 : IVertex {

		public const int SIZE = 8;

		public Vector2 Position;
		public Vector4 Color;
		public Vector2 UV;

		public Vertex2() {
			Position = Vector2.Zero;
			Color = Vector4.One;
			UV = Vector2.Zero;
		}

		public Vertex2( Vector2 pos, Vector4 color, Vector2 uv ) {
			Position = pos;
			Color = color;
			UV = uv;
		}

		public Vertex2 SetPosition( Vector2 value ) {
			Position = value;
			return this;
		}

		public Vertex2 SetColor( Vector4 value ) {
			Color = value;
			return this;
		}

		public Vertex2 SetUV( Vector2 value ) {
			UV = value;
			return this;
		}

		public void AddVertex( List<float> list ) {
			list.Add( Position.X );
			list.Add( Position.Y );
			list.Add( Color.X );
			list.Add( Color.Y );
			list.Add( Color.Z );
			list.Add( Color.W );
			list.Add( UV.X );
			list.Add( UV.Y );
		}

		public void AddVertex( float[] arr, int index ) {
			int i = index * SIZE;
			arr[ i++ ] = ( Position.X );
			arr[ i++ ] = ( Position.Y );
			arr[ i++ ] = ( Color.X );
			arr[ i++ ] = ( Color.Y );
			arr[ i++ ] = ( Color.Z );
			arr[ i++ ] = ( Color.W );
			arr[ i++ ] = ( UV.X );
			arr[ i++ ] = ( UV.Y );
		}

		public int GetSize() {
			return SIZE;
		}
	}
}
