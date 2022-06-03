using System.Runtime.InteropServices;
using Engine.Data.Buffers;
using Engine.Rendering.Standard;
using OpenGL;

namespace Engine.Rendering;

public abstract unsafe class DataBlock : DisposableIdentifiable {

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
		this._buffer = buffer.VBO;
		this.Segment = buffer.AllocateSynchronized( sizeBytes ) ?? throw new Exception( "Allocate data blocks only on the context thread." );
		this._bytes = (byte*) NativeMemory.Alloc( sizeBytes );
		this._shaderTypes = new List<ShaderType>( shaderTypes );
	}

	public abstract void DirectBindShader( ShaderProgram p );

	protected void DirectBindBuffer( uint index ) {
		this.BoundIndex = index;
		Gl.BindBufferRange( this.Target, this.BoundIndex, this._buffer.BufferId, (IntPtr) this.Segment.OffsetBytes, this.Segment.SizeBytes );
	}

	public void DirectUnbindBuffer() => Gl.BindBufferRange( this.Target, this.BoundIndex, 0, IntPtr.Zero, 0 );

	public void DirectWrite<T>( T t, uint offsetBytes = 0 ) where T : unmanaged {
		if ( !Resources.Render.InThread ) {
			this.LogError( "Can't write to uniform buffer outside context thread." );
			return;
		}
		if ( Marshal.SizeOf<T>() + offsetBytes > this.Segment.SizeBytes ) {
			this.LogError( "Can't write outsite block." );
			return;
		}
		( (T*) (this._bytes + offsetBytes ) )[ 0 ] = t;
		this._buffer.DirectWrite( new IntPtr( this._bytes ), (uint) this.Segment.OffsetBytes + offsetBytes, offsetBytes, (uint) Marshal.SizeOf<T>() );
	}

	protected override bool OnDispose() {
		NativeMemory.Free( this._bytes );
		return true;
	}
}
