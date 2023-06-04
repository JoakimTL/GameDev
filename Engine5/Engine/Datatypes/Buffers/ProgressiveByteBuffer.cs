using System.Runtime.InteropServices;

namespace Engine.Datatypes.Buffers;
public unsafe class ProgressiveByteBuffer {

    private byte* _ptr;
    private uint _size;
    private uint _caret;

    /// <summary>
    /// The size of the buffer. Not as relevant as the caret position.
    /// </summary>
    public uint Size => _size;
    /// <summary>
    /// The position of the buffer caret. This determines where the next data will be put.
    /// </summary>
    public uint CaretPosition => _caret;

    public ProgressiveByteBuffer() {
        _size = 512;
        _ptr = (byte*) NativeMemory.Alloc( _size );
    }

    public void Add( byte[] bytes, uint length ) {
        if ( length > bytes.Length )
            throw new ArgumentException( $"{nameof( length )} cannot be greater than {nameof( bytes )}.Length!" );
        if ( _caret + length > _size ) {
            _size = _caret + length;
            _ptr = (byte*) NativeMemory.Realloc( _ptr, _size );
        }
        fixed ( byte* srcPtr = bytes )
            Buffer.MemoryCopy( srcPtr, _ptr + _caret, _size - _caret, length );
        _caret += length;
    }
    /// <summary>
    /// Resets the caret, and returns the buffer.
    /// </summary>
    /// <returns>A byte array copy of the buffer data</returns>
    public byte[] Flush( uint startByte ) {
        if ( _caret <= startByte )
            throw new ArgumentOutOfRangeException( nameof( startByte ), "Cannot start after the end." );
        uint length = _caret - startByte;
        byte[] data = new byte[ length ];
        fixed ( byte* dstPtr = data )
            Buffer.MemoryCopy( _ptr + startByte, dstPtr, length, length );
        _caret = 0;
        return data;
    }

    public T Read<T>( uint offsetBytes ) where T : unmanaged {
        if ( offsetBytes + sizeof( T ) > _size )
            throw new ArgumentOutOfRangeException( nameof( offsetBytes ), "Cannot read data outside buffer." );
        return *(T*) ( _ptr + offsetBytes );
    }

}
