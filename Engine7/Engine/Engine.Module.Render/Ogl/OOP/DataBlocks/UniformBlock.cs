using Engine.Module.Render.Ogl.OOP.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.DataBlocks;

public sealed class UniformBlock( OglBufferSegment bufferSegment, string blockName, Span<ShaderType> shaderTypes ) : DataBlockBase(bufferSegment, blockName, shaderTypes, DataBlockType.Uniform) {
	protected override BufferTarget Target => BufferTarget.UniformBuffer;

	public override void Bind( OglShaderProgramBase program ) {
		if (!program.GetUniformBlockIndex( BlockName, out uint index ))
			return;
		BindBuffer( index );
		program.SetUniformBinding( BoundIndex, BoundIndex );
	}
}
