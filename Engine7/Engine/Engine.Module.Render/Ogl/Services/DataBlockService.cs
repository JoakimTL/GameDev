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
		Gl.GetInteger( GetPName.UniformBufferOffsetAlignment, out _alignment );
		Gl.GetInteger( GetPName.MaxUniformBlockSize, out _blockMaxSize );
	}

	public bool CreateUniformBlock( string blockName, uint size, Span<ShaderType> shaderTypes, [NotNullWhen(true)] out UniformBlock? block ) {
		block = null;
		if (size % _alignment != 0)
			return false;
		if (size > _blockMaxSize)
			return false;
		if (!oglBufferService.UniformBuffer.TryAllocate( size, out OglBufferSegment? segment ))
			return false;
		block = new( segment, blockName, shaderTypes );
		_dataBlocks.Add( new( block ) );
		return true;
	}

	public bool CreateShaderStorageBlock( string blockName, uint size, Span<ShaderType> shaderTypes, [NotNullWhen( true )] out ShaderStorageBlock? block ) {
		block = null;
		if (!oglBufferService.ShaderStorage.TryAllocate( size, out OglBufferSegment? segment ))
			return false;
		block = new( segment, blockName, shaderTypes );
		_dataBlocks.Add( new( block ) );
		return true;
	}

	protected override bool InternalDispose() {
		foreach (WeakReference<DataBlockBase> dataBlockRef in _dataBlocks) {
			if (dataBlockRef.TryGetTarget( out DataBlockBase? dataBlock ))
				dataBlock.Dispose();
		}
		return true;
	}
}
