using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.Utilities.Data;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class TextMesh : InstancedMeshVBOArray {

		public const int INSTANCEDATA = 9;
		public const int SIZEBYTES = 36;

		private static float[] verticePositions = new float[] {
			0, 1,
			0, 0,
			1, 1,
			1, 0
		};

		public TextMesh( string name, int maxInstances ) : base( name, verticePositions.Length * sizeof( float ), maxInstances * SIZEBYTES, BufferUsage.DynamicDraw, PrimitiveType.TriangleStrip ) {
			LoadMesh( DataTransform.GetBytes( verticePositions ), sizeof( float ) * 2 );

			Setup( VAO );
		}

		public override void Setup( uint VAO ) {
			if( VAO > 0 )
				Gl.BindVertexArray( VAO );
			Gl.EnableVertexAttribArray( 0 );
			Gl.EnableVertexAttribArray( 1 );
			Gl.EnableVertexAttribArray( 2 );
			Gl.EnableVertexAttribArray( 3 );
			Gl.EnableVertexAttribArray( 4 );
			Gl.EnableVertexAttribArray( 5 );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			Gl.VertexAttribPointer( 0, 2, VertexAttribType.Float, false, 2 * sizeof( float ), (IntPtr) 0 );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, DBO );
			Gl.VertexAttribPointer( 1, 2, VertexAttribType.Float, false, SIZEBYTES, (IntPtr) 0 );
			Gl.VertexAttribPointer( 2, 2, VertexAttribType.Float, false, SIZEBYTES, (IntPtr) ( 2 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 3, 2, VertexAttribType.Float, false, SIZEBYTES, (IntPtr) ( 4 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 4, 2, VertexAttribType.Float, false, SIZEBYTES, (IntPtr) ( 6 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 5, 4, VertexAttribType.UnsignedByte, true, SIZEBYTES, (IntPtr) ( 8 * sizeof( float ) ) );
			Gl.VertexAttribDivisor( 1, 1 );
			Gl.VertexAttribDivisor( 2, 1 );
			Gl.VertexAttribDivisor( 3, 1 );
			Gl.VertexAttribDivisor( 4, 1 );
			Gl.VertexAttribDivisor( 5, 1 );
			if( VAO > 0 )
				Gl.BindVertexArray( 0 );
		}

		internal void FillData( IReadOnlyList<byte> data ) {
			Buffers[ DBO ].SetRange( 0, data.ToArray() );
			SetActiveInstances( data.Count / SIZEBYTES );
		}

	}
}
