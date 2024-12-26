using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.Instancing {
	public abstract class InstancedMeshVBOArray : InstancedMesh {

		public int MeshSize { get; private set; }
		private PrimitiveType primitiveType;

		public InstancedMeshVBOArray( string name, int meshDataBytes, int instanceDataBytes, BufferUsage instanceDataBufferUsage, PrimitiveType renderPrimitive ) : base( name, meshDataBytes, instanceDataBytes, instanceDataBufferUsage ) {
			this.primitiveType = renderPrimitive;
		}

		public void LoadMesh( byte[] data, int sizePerVertex ) {
			DataSegments[ VBO ].SetRange( 0, data );
			MeshSize = data.Length / sizePerVertex;
		}

		public override void RenderMesh() {
			if( ActiveInstances == -1 )
				Logging.Warning( "Active instances has not been set, did you load in instance set?" );
			if( MeshSize == 0 )
				Logging.Warning( "Mesh size has not been set, did you load a mesh into the instancer?" );
			Gl.DrawArraysInstanced( primitiveType, DataSegments[ VBO ].Offset, MeshSize, ActiveInstances );
		}
	}
}
