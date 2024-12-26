using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.Instancing {
	public abstract class InstancedMeshVBOArray : InstancedMesh {

		public int MeshSize { get; private set; }
		private PrimitiveType primitiveType;

		/// <param name="meshDataBytes">Bytes dedicated to the rendered mesh</param>
		/// <param name="instanceDataBytes">Bytes dedicated to the instance data</param>
		public InstancedMeshVBOArray( string name, int meshDataBytes, int instanceDataBytes, BufferUsage instanceDataBufferUsage, PrimitiveType renderPrimitive ) : base( name, meshDataBytes, instanceDataBytes, instanceDataBufferUsage ) {
			this.primitiveType = renderPrimitive;
		}

		public void LoadMesh( byte[] data, int sizePerVertex ) {
			Buffers[ VBO ].SetRange( 0, data );
			MeshSize = data.Length / sizePerVertex;
		}

		public override void RenderMesh() {
			if( ActiveInstances == -1 )
				Logging.Warning( "Active instances has not been set, did you load in instance set?" );
			if( MeshSize == 0 )
				Logging.Warning( "Mesh size has not been set, did you load a mesh into the instancer?" );
			Gl.DrawArraysInstanced( primitiveType, 0, MeshSize, ActiveInstances );
		}
	}
}
