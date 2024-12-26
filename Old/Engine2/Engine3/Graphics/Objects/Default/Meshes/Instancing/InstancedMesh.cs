using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Data;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.Instancing {
	public abstract class InstancedMesh : Mesh {

		public uint VBO { get; private set; }
		public uint DBO { get; private set; }

		public int ActiveInstances { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="meshDataBytes">Bytes dedicated to the rendered mesh</param>
		/// <param name="instanceDataBytes">Bytes dedicated to the instance data</param>
		public InstancedMesh( string name, int meshDataBytes, int instanceDataBytes, BufferUsage instanceDataBufferUsage ) : base( name ) {
			VBO = AllocateBuffer( meshDataBytes, BufferUsage.DynamicDraw, BufferTarget.ArrayBuffer );
			DBO = AllocateBuffer( instanceDataBytes, instanceDataBufferUsage, BufferTarget.ArrayBuffer );
			ActiveInstances = -1;
		}

		public void SetActiveInstances( int num ) {
			if( num < 0 )
				return;
			ActiveInstances = num;
		}

		public void BufferData( int offset, IReadOnlyList<byte> data ) {
			Buffers[ DBO ].SetRange( offset, data.ToArray() );
		}

		public void BufferData( int offset, byte[] data ) {
			Buffers[ DBO ].SetRange( offset, data );
		}

		public override void Bind() {
			Gl.BindVertexArray( VAO );
		}

		public override void Unbind() {
			Gl.BindVertexArray( 0 );
		}

	}
}
