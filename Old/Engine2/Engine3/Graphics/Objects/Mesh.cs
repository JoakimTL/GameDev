using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Data;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Graphics.Objects {
	public abstract class Mesh : Cacheable, ICountable {

		public uint VAO { get; private set; }
		public uint ID { get => VAO; }
		protected ShaderBind shaderBind;
		protected Dictionary<uint, DataBuffer> Buffers { get; private set; }

		public Mesh( string name ) : base( name ) {
			Buffers = new Dictionary<uint, DataBuffer>();
			shaderBind = null;
			VAO = Gl.GenVertexArray();
			Logging.Memory( $"[{Name}]: Allocated VAO at index {VAO}" );
		}

		protected uint AllocateBuffer( int size, BufferUsage bufferUse, BufferTarget target ) {
			if( size <= 0 )
				return 0;
			DataBuffer buffer = new DataBuffer( (uint) size, bufferUse, target );
			Buffers.Add( buffer.BO, buffer );
			return buffer.BO;
		}

		protected uint AllocateBuffer( uint size, BufferUsage bufferUse, BufferTarget target ) {
			if( size <= 0 )
				return 0;
			DataBuffer buffer = new DataBuffer( size, bufferUse, target );
			Buffers.Add( buffer.BO, buffer );
			return buffer.BO;
		}

		protected void LoadBytes( uint bufferId, int offset, byte[] data ) {
			if( !Buffers.TryGetValue( bufferId, out DataBuffer buffer ) ) {
				Mem.Logs.Warning.WriteLine( $"[{Name}] Mesh reference is null." );
				return;
			}
			buffer.SetRange( offset, data );
		}

		protected void LoadBytes( uint bufferId, int offset, IReadOnlyList<byte> data ) {
			if( !Buffers.TryGetValue( bufferId, out DataBuffer buffer ) ) {
				Mem.Logs.Warning.WriteLine( $"[{Name}] Mesh reference is null." );
				return;
			}
			buffer.SetRange( offset, data.ToArray() );
		}

		protected void LoadFloats( uint bufferId, int offset, IReadOnlyList<float> data ) {
			if( !Buffers.TryGetValue( bufferId, out DataBuffer buffer ) ) {
				Mem.Logs.Warning.WriteLine( $"[{Name}] Mesh reference is null." );
				return;
			}
			byte[] dataBytes = DataTransform.GetBytes( data );
			buffer.SetRange( offset, dataBytes );
		}

		protected void LoadInts( uint bufferId, int offset, IReadOnlyList<int> data ) {
			if( !Buffers.TryGetValue( bufferId, out DataBuffer buffer ) ) {
				Mem.Logs.Warning.WriteLine( $"[{Name}] Mesh reference is null." );
				return;
			}
			byte[] dataBytes = DataTransform.GetBytes( data );
			buffer.SetRange( offset, dataBytes );
		}

		public override void Dispose() {
			if( VAO > 0 ) {
				Gl.DeleteVertexArrays( VAO );
				foreach( DataBuffer buffer in Buffers.Values ) {
					buffer.Dispose();
				}
				Mem.Logs.MemoryLogger.WriteLine( $"[{Name}]: Disposed [{VAO}]!" );
			}
			VAO = 0;
		}

		public override int GetHashCode() {
			return (int) ID;
		}

		public override bool Equals( object obj ) {
			if( !( obj is Mesh o ) )
				return false;
			return Equals( o );
		}

		public bool Equals( Mesh obj ) {
			return obj.ID == ID && obj.VAO == VAO;
		}

		public static bool operator ==( Mesh a, Mesh b ) {
			if( a is null || b is null )
				return false;
			return a.Equals( b );
		}

		public static bool operator !=( Mesh a, Mesh b ) {
			return !( a == b );
		}

		public void BindShader( Shader s ) => shaderBind?.Invoke( s );
		public abstract void Setup( uint VAO );
		public abstract void Bind();
		public abstract void RenderMesh();
		public abstract void Unbind();

	}
}
