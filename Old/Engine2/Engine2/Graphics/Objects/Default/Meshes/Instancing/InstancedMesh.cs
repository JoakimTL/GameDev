using Engine.MemLib;
using Engine.Utilities.Data;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.Instancing {
	public abstract class InstancedMesh : Mesh {

		public uint VBO { get; private set; }
		public uint DBO { get; private set; }

		public int ActiveInstances { get; private set; }

		public InstancedMesh( string name, int meshDataBytes, int instanceDataBytes, BufferUsage instanceDataBufferUsage ) : base( name ) {
			VBO = AllocateBuffer( meshDataBytes, StorageType.VERTEXDATA, BufferUsage.DynamicDraw );
			DBO = AllocateBuffer( instanceDataBytes, StorageType.VERTEXDATA, instanceDataBufferUsage );
			ActiveInstances = -1;
		}

		public void SetActiveInstances( int num ) {
			if( num < 0 )
				return;
			ActiveInstances = num;
		}

		public void GiveSubSegment( Instance o, int offset ) {
			DataArray<byte>.DataSegment.SubSegment subseg = DataSegments[ DBO ].CreateSubSegment( offset );
			o.SetSubSegment( subseg );
		}

		public override void Bind() {
			Gl.BindVertexArray( VAO );
		}

		public override void Unbind() {
			Gl.BindVertexArray( 0 );
		}

	}
}
