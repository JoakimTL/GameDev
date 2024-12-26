using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.Instancing {
	public abstract class InstancedMeshIndexed : InstancedMesh {

		public uint IBO { get; private set; }

		public int Size { get; private set; }
		private PrimitiveType primitiveType;

		public InstancedMeshIndexed( string name, int meshDataBytes, int indexDataBytes, int instanceDataBytes, BufferUsage instanceDataBufferUsage, PrimitiveType renderPrimitive = PrimitiveType.Triangles ) : base( name, meshDataBytes, instanceDataBytes, instanceDataBufferUsage ) {
			IBO = AllocateBuffer( meshDataBytes, StorageType.INDEXDATA, BufferUsage.DynamicDraw );
			this.primitiveType = renderPrimitive;
		}

		public void LoadVertices( byte[] data ) {
			DataSegments[ VBO ].SetRange( 0, data );
		}

		public void LoadIndexes( int[] data ) {
			LoadInts( IBO, 0, data );
		}

		public override void RenderMesh() {
			if( ActiveInstances == -1 )
				Logging.Warning( "Active instances has not been set, did you load in instance set?" );
			if ( Size == 0 )
				Logging.Warning( "Mesh size has not been set, did you load a mesh into the instancer?" );
			Gl.DrawElementsInstanced( primitiveType, Size, DrawElementsType.UnsignedInt, (IntPtr) ( DataSegments[ IBO ].Offset ), ActiveInstances );
		}
	}
}
