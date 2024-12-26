using Engine.Graphics.Objects.Default.Meshes.Instancing;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2 {
	public class ParticleMesh : InstancedMeshVBOArray {

		private static float[] verticePositions = new float[] {
			-.5f, .5f,
			-.5f, -.5f,
			.5f, .5f,
			.5f, -.5f
		};

		public ParticleMesh( int numInstances ) : base( "ParticleMesh", verticePositions.Length * sizeof( float ), numInstances * Particle2.SIZEBYTES, BufferUsage.StreamDraw, PrimitiveType.TriangleStrip ) {

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
			Gl.EnableVertexAttribArray( 6 );
			Gl.EnableVertexAttribArray( 7 );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			Gl.VertexAttribPointer( 0, 2, VertexAttribType.Float, false, 2 * sizeof( float ), (IntPtr) ( DataSegments[ VBO ].Offset ) );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, DBO );
			Gl.VertexAttribPointer( 1, 4, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) ( DataSegments[ DBO ].Offset ) );
			Gl.VertexAttribPointer( 2, 4, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) ( DataSegments[ DBO ].Offset + 4 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 3, 4, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) ( DataSegments[ DBO ].Offset + 2 * 4 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 4, 4, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) ( DataSegments[ DBO ].Offset + 3 * 4 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 5, 4, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) ( DataSegments[ DBO ].Offset + 4 * 4 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 6, 4, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) ( DataSegments[ DBO ].Offset + 5 * 4 * sizeof( float ) ) );
			Gl.VertexAttribPointer( 7, 1, VertexAttribType.Float, false, Particle2.SIZEBYTES, (IntPtr) ( DataSegments[ DBO ].Offset + 6 * 4 * sizeof( float ) ) );
			Gl.VertexAttribDivisor( 1, 1 );
			Gl.VertexAttribDivisor( 2, 1 );
			Gl.VertexAttribDivisor( 3, 1 );
			Gl.VertexAttribDivisor( 4, 1 );
			Gl.VertexAttribDivisor( 5, 1 );
			Gl.VertexAttribDivisor( 6, 1 );
			Gl.VertexAttribDivisor( 7, 1 );
			if( VAO > 0 )
				Gl.BindVertexArray( 0 );
		}
	}
}
