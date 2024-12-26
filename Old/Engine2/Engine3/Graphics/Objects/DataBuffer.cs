using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects {
	public class DataBuffer {

		//Replcaes the dataarray
		//Does not contain a byte array, but rather the size and id of OGL buffers,
		//Allows the user to buffer byte data to the buffer without handling ogl commands

		public uint BO { get; private set; }
		public BufferUsage Usage { get; private set; }
		public BufferTarget Target { get; private set; }
		public uint Size { get; private set; }
		public bool Disposed { get; private set; }

		public DataBuffer( uint sizeBytes, BufferUsage usage, BufferTarget target ) {
			BO = Gl.GenBuffer();
			Usage = usage;
			Target = target;
			Size = sizeBytes;
			Disposed = false;
			CreateBuffer( sizeBytes, usage );
		}

		private void CreateBuffer( uint sizeBytes, BufferUsage usage ) {
			if( Disposed ) {
				Logging.Warning( "Attempted to create a buffer with a disposed buffer object. Attempt ignored." );
				return;
			}
			Gl.BindBuffer( Target, BO );
			Gl.BufferData( Target, sizeBytes, (IntPtr) 0, Usage );
		}

		public void SetRange( int byteIndex, byte[] data ) {
			if( Disposed ) {
				Logging.Warning( "Attempted to buffer data to a disposed buffer. Attempt ignored." );
				return;
			}
			if( byteIndex < 0 || byteIndex + data.Length > Size ) {
				Logging.Warning( "Attempted to buffer data outside the buffer bounds. Attempt ignored." );
				return;
			}
			Gl.BindBuffer( Target, BO );
			Gl.BufferSubData( Target, (IntPtr) byteIndex, (uint) data.Length, data );
		}

		public void Resize( uint sizeBytes ) {
			if( Disposed ) {
				Logging.Warning( "Attempted to resize a disposed buffer. Attempt ignored." );
				return;
			}
			if( sizeBytes == 0 ) {
				Logging.Warning( "Attempted to resize buffer to 0 bytes. Attempt ignored." );
				return;
			}
			uint tempBuffer = Gl.GenBuffer();
			Gl.BindBuffer( BufferTarget.CopyWriteBuffer, tempBuffer );
			Gl.BufferData( BufferTarget.CopyWriteBuffer, Size, (IntPtr) 0, Usage );
			Gl.BindBuffer( BufferTarget.CopyReadBuffer, BO );
			Gl.CopyBufferSubData( BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, (IntPtr) 0, (IntPtr) 0, Size );
			uint prevSize = Size;
			Size = sizeBytes;
			CreateBuffer( Size, Usage );
			Gl.BindBuffer( BufferTarget.CopyWriteBuffer, BO );
			Gl.BindBuffer( BufferTarget.CopyReadBuffer, tempBuffer );
			Gl.CopyBufferSubData( BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, (IntPtr) 0, (IntPtr) 0, Math.Min( Size, prevSize ) );
			Gl.BindBuffer( BufferTarget.CopyWriteBuffer, 0 );
			Gl.BindBuffer( BufferTarget.CopyReadBuffer, 0 );
			Gl.DeleteBuffers( tempBuffer );
		}

		public void Dispose() {
			if( Disposed ) {
				Logging.Warning( "Attempted to dispose a disposed buffer. Attempt ignored." );
				return;
			}
			Gl.DeleteBuffers( BO );
			Disposed = true;
		}
	}
}
