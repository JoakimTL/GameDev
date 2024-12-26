using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.VertexMeshing {
	public abstract class VertexMesh<T> : Mesh where T : struct {

		public uint VBO { get; protected set; }
		public uint IBO { get; protected set; }

		protected int Size { get; private set; }

		public VertexMesh( string name ) : base( name ) {

		}

		public abstract void LoadVertices( uint buffer, int offset, IReadOnlyList<T> list );

		public void LoadIndices( uint buffer, int offset, IReadOnlyList<int> list ) {
			LoadInts( buffer, offset, list );
			Size = list.Count;
		}
	}
}
