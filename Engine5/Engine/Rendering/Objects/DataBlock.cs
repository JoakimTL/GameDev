using Engine.Structure.Interfaces;
using OpenGL;
using System.Diagnostics;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Objects;

public abstract unsafe class DataBlock : Identifiable, IDisposable {

	/// <summary>
	/// The name of the block.
	/// </summary>
	public string BlockName { get; private set; }
	/// <summary>
	/// The segment in the databuffer.
	/// </summary>
	public IDataSegmentInformation Segment { get; private set; }
	/// <summary>
	/// The shader type this block is found in.
	/// </summary>
	public IReadOnlyList<ShaderType> ShaderTypes => this._shaderTypes;
	public uint BoundIndex { get; private set; }
	private readonly byte* _bytes;
	private readonly VertexBufferObject _buffer;
	protected readonly List<ShaderType> _shaderTypes;
#if DEBUG
	public Memory<byte> Bytes => DebugUtilities.PointerToMemory( _bytes, Segment.SizeBytes );
#endif

	protected abstract BufferTarget Target { get; }

	public DataBlock( SegmentedVertexBufferObject buffer, ShaderType[] shaderTypes, string blockName, uint sizeBytes, uint alignmentBytes, uint maxSizeBytes = 0 ) : base( blockName ) {
		if ( maxSizeBytes > 0 && sizeBytes > maxSizeBytes )
			throw new ArgumentOutOfRangeException( $"The argument 'size' exceeds the block size limit of {maxSizeBytes}!" );
		if ( alignmentBytes > 0 && sizeBytes % alignmentBytes != 0 ) {
			uint newSize = ( ( sizeBytes / alignmentBytes ) + 1 ) * alignmentBytes;
			Log.Line( $"Size [{sizeBytes}] of uniform block {this.FullName} is not aligned. Size changed to {newSize}!", Log.Level.NORMAL );
			sizeBytes = newSize;
		}
		this.BlockName = blockName;
		this._buffer = buffer;
		this.Segment = buffer.AllocateSynchronized( sizeBytes ) ?? throw new Exception( "Allocate data blocks only on the context thread." );
		this._bytes = (byte*) NativeMemory.Alloc( sizeBytes );
		this._shaderTypes = new List<ShaderType>( shaderTypes );
	}

#if DEBUG
	~DataBlock() {
		Debug.Fail( "Data block was not disposed!" );
	}
#endif

	public abstract void BindShader( ShaderProgramBase p );

	protected void BindBuffer( uint index ) {
		this.BoundIndex = index;
		Gl.BindBufferRange( this.Target, this.BoundIndex, this._buffer.BufferId, (IntPtr) this.Segment.OffsetBytes, this.Segment.SizeBytes );
	}

	public void UnbindBuffer() => Gl.BindBufferRange( this.Target, this.BoundIndex, 0, IntPtr.Zero, 0 );

	public void Write<T>( T t, uint offsetBytes = 0 ) where T : unmanaged {
		if ( Marshal.SizeOf<T>() + offsetBytes > this.Segment.SizeBytes ) {
			this.LogError( "Can't write outsite block." );
			return;
		}
		( (T*) ( this._bytes + offsetBytes ) )[ 0 ] = t;
		this._buffer.Write( new IntPtr( this._bytes ), (uint) this.Segment.OffsetBytes + offsetBytes, offsetBytes, (uint) Marshal.SizeOf<T>() );
	}

	public void Write<T>( Span<T> data, uint offsetBytes = 0 ) where T : unmanaged {
		if ( Marshal.SizeOf<T>() * data.Length + offsetBytes > this.Segment.SizeBytes ) {
			this.LogError( "Can't write outsite block." );
			return;
		}
		fixed ( T* src = data )
			Unsafe.CopyBlock( this._bytes + offsetBytes, src, (uint) ( data.Length * Marshal.SizeOf<T>() ) );
		this._buffer.Write( new IntPtr( this._bytes ), (uint) this.Segment.OffsetBytes + offsetBytes, offsetBytes, (uint) ( data.Length * Marshal.SizeOf<T>() ) );
	}

	public void Write( void* src, uint length, uint offsetBytes = 0 ) {
		if ( length + offsetBytes > this.Segment.SizeBytes ) {
			this.LogError( "Can't write outsite block." );
			return;
		}
		Unsafe.CopyBlock( this._bytes + offsetBytes, src, length );
		this._buffer.Write( new IntPtr( this._bytes ), (uint) this.Segment.OffsetBytes + offsetBytes, offsetBytes, length );
	}

	public void Dispose() {
		NativeMemory.Free( this._bytes );
		GC.SuppressFinalize( this );
	}
}
