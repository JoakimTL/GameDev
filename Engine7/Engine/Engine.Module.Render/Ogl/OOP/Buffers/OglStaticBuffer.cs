using Engine.Logging;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Buffers;

public unsafe class OglStaticBuffer( BufferUsage usage, uint lengthBytes ) : OglBufferBase( usage, lengthBytes ), IWritableBuffer<uint> {
	public bool WriteRange<T>( Span<T> source, uint destinationOffsetBytes ) where T : unmanaged {
		if (this.Disposed)
			return this.LogWarningThenReturn( "Already disposed!", false );
		if (source.Length == 0)
			return this.LogWarningThenReturn( "Cannot write 0 bytes!", false );
		uint bytesToCopy = (uint) (source.Length * sizeof( T ));
		if (destinationOffsetBytes + bytesToCopy > this.LengthBytes)
			return false;
		fixed (T* srcPtr = source)
			Write( new nint( srcPtr ), destinationOffsetBytes, bytesToCopy );
		return true;
	}

	public unsafe bool WriteRange( void* srcPtr, uint srcLengthBytes, uint destinationOffsetBytes ) {
		if (this.Disposed)
			return this.LogWarningThenReturn( "Already disposed!", false );
		if (srcLengthBytes == 0)
			return this.LogWarningThenReturn( "Cannot write 0 bytes!", false );
		if (destinationOffsetBytes + srcLengthBytes > this.LengthBytes)
			return false;
		Write( new nint( srcPtr ), destinationOffsetBytes, srcLengthBytes );
		return true;
	}
}
