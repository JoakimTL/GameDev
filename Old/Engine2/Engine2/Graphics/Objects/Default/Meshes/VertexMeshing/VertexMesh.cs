using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.VertexMeshing {
	public abstract class VertexMesh<T> : Mesh where T : IVertex {

		public uint VBO { get; protected set; }
		public uint IBO { get; protected set; }

		public VertexMesh( string name ) : base( name ) {

		}

		public void LoadVertices( uint buffer, int offset, IReadOnlyList<T> list ) {
			List<float> data = new List<float>();
			for( int i = 0; i < list.Count; i++ )
				list[ i ].AddVertex( data );
			LoadFloats( buffer, offset, data );
		}

	}

	public interface IVertex {
		void AddVertex( List<float> list );
		void AddVertex( float[] array, int index );
		int GetSize();
	}
}
