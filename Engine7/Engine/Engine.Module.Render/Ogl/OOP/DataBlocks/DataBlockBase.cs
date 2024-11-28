using Engine.Module.Render.Ogl.OOP.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Module.Render.Ogl.OOP.DataBlocks;
public abstract unsafe class DataBlockBase : DisposableIdentifiable {

	public readonly DataBlockType DatablockType;
	private readonly OglBufferSegment _bufferSegment;
	private readonly void* _dataPtr;

	public string BlockName { get; }
	public uint BoundIndex { get; private set; }
	/// <summary>
	/// The shader type this block is found in.
	/// </summary>
	public IReadOnlyList<ShaderType> ShaderTypes { get; }
	public IWritableBuffer<uint> Buffer => this._bufferSegment;

	protected DataBlockBase( OglBufferSegment bufferSegment, string blockName, Span<ShaderType> shaderTypes, DataBlockType dataBlockType ) {
		this._bufferSegment = bufferSegment;
		this.BlockName = blockName;
		this.DatablockType = dataBlockType;
		this._dataPtr = NativeMemory.Alloc( bufferSegment.LengthBytes );
		this.ShaderTypes = shaderTypes.ToArray().AsReadOnly();
	}

	protected void BindBuffer( uint index ) {
		BoundIndex = index;
		Gl.BindBufferRange( Target, BoundIndex, _bufferSegment.BufferId, (nint) _bufferSegment.OffsetBytes,  _bufferSegment.LengthBytes );
	}
	public void UnbindBuffer() => Gl.BindBufferRange( Target, BoundIndex, 0, nint.Zero, 0 );

	public abstract void Bind( OglShaderProgramBase program );
	protected abstract BufferTarget Target { get; }

	protected override bool InternalDispose() {
		this._bufferSegment.Dispose();
		NativeMemory.Free( this._dataPtr );
		return true;
	}

}
