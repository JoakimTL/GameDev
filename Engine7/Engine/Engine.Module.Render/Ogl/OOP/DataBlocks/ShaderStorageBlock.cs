using Engine.Module.Render.Ogl.OOP.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.DataBlocks;

public sealed class ShaderStorageBlock( OglBufferSegment bufferSegment, string blockName, Span<ShaderType> shaderTypes ) : DataBlockBase( bufferSegment, blockName, shaderTypes, DataBlockType.ShaderStorage ) {
	protected override BufferTarget Target => BufferTarget.ShaderStorageBuffer;

	public override void Bind( OglShaderProgramBase program ) {
		if (!program.GetShaderStorageBlockIndex( this.BlockName, out uint index ))
			return;
		BindBuffer( index );
		program.SetShaderStorageBinding( this.BoundIndex, this.BoundIndex );
	}
}
