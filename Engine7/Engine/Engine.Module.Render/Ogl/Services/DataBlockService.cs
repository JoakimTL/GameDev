using Engine.Module.Render.Ogl.OOP.Buffers;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using OpenGL;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Module.Render.Ogl.Services;

public sealed class DataBlockService( OglBufferService oglBufferService ) : DisposableIdentifiable, IInitializable {

	private readonly List<WeakReference<DataBlockBase>> _dataBlocks = [];
	private uint _blockMaxSize;
	private uint _alignment;

	public void Initialize() {
		Gl.GetInteger( GetPName.UniformBufferOffsetAlignment, out this._alignment );
		Gl.GetInteger( GetPName.MaxUniformBlockSize, out this._blockMaxSize );
	}

	public UniformBlock CreateUniformBlockOrThrow( string blockName, uint size, Span<ShaderType> shaderTypes ) {
		if (!TryCreateUniformBlock( blockName, size, shaderTypes, out UniformBlock? block ))
			throw new InvalidOperationException( "Failed to create uniform block" );
		return block;
	}

	public ShaderStorageBlock CreateShaderStorageBlockOrThrow( string blockName, uint size, Span<ShaderType> shaderTypes ) {
		if (!TryCreateShaderStorageBlock( blockName, size, shaderTypes, out ShaderStorageBlock? block ))
			throw new InvalidOperationException( "Failed to create shader block" );
		return block;
	}

	public bool TryCreateUniformBlock( string blockName, uint size, Span<ShaderType> shaderTypes, [NotNullWhen( true )] out UniformBlock? block ) {
		block = null;
		if (size % this._alignment != 0)
			return false;
		if (size > this._blockMaxSize)
			return false;
		if (!oglBufferService.UniformBuffer.TryAllocate( size, out OglBufferSegment? segment ))
			return false;
		block = new( segment, blockName, shaderTypes );
		this._dataBlocks.Add( new( block ) );
		return true;
	}

	public bool TryCreateShaderStorageBlock( string blockName, uint size, Span<ShaderType> shaderTypes, [NotNullWhen( true )] out ShaderStorageBlock? block ) {
		block = null;
		if (!oglBufferService.ShaderStorage.TryAllocate( size, out OglBufferSegment? segment ))
			return false;
		block = new( segment, blockName, shaderTypes );
		this._dataBlocks.Add( new( block ) );
		return true;
	}

	protected override bool InternalDispose() {
		foreach (WeakReference<DataBlockBase> dataBlockRef in this._dataBlocks) {
			if (dataBlockRef.TryGetTarget( out DataBlockBase? dataBlock ))
				dataBlock.Dispose();
		}
		return true;
	}
}
