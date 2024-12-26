using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.Utilities.Data;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2 {
	public class Particle2Mesh : InstancedMeshVBOArray {

		private static float[] verticePositions = new float[] {
			-.5f, .5f,
			-.5f, -.5f,
			.5f, .5f,
			.5f, -.5f
		};

		public Particle2Mesh( int numInstances ) : base( "ParticleMesh", verticePositions.Length * sizeof( float ), numInstances * Particle2.SIZEBYTES, BufferUsage.StreamDraw, PrimitiveType.TriangleStrip ) {
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
			Gl.VertexAttribPointer( 1, 3, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) 0 );
			Gl.VertexAttribPointer( 2, 4, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) ( 3 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 3, 4, VertexAttribType.UnsignedByte, true, Particle2.SIZEBYTES, (IntPtr) ( 7 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 4, 1, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) ( 8 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 5, 2, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) ( 9 * sizeof( float ) ) );
			Gl.VertexAttribDivisor( 1, 1 );
			Gl.VertexAttribDivisor( 2, 1 );
			Gl.VertexAttribDivisor( 3, 1 );
			Gl.VertexAttribDivisor( 4, 1 );
			Gl.VertexAttribDivisor( 5, 1 );
			if( VAO > 0 )
				Gl.BindVertexArray( 0 );
		}
	}
}
