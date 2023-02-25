using OpenGL;

namespace Engine.Rendering.Contexts.Objects;

public unsafe class UniformBlock : DataBlock
{

    /// <summary>
    /// Creates a new uniform block, ready for use. This block has a unique binding location, and can be reused in multiple block collections.
    /// </summary>
    /// <param name="shaderTypes">The types of shaders this block pertains to.</param>
    /// <param name="blockName">The block name.</param>
    /// <param name="sizeBytes">The size of the uniform block in bytes. GLSL works in 4-byte alignment, meaning this number must be a multiple of 4!</param>
    public UniformBlock(SegmentedVertexBufferObject svbo, string blockName, uint sizeBytes, params ShaderType[] shaderTypes) : base(svbo, shaderTypes, blockName, sizeBytes, GetAlignment(), GetMaxSize()) { }

    protected override BufferTarget Target => BufferTarget.UniformBuffer;

    private static uint GetAlignment()
    {
        Gl.GetInteger(GetPName.UniformBufferOffsetAlignment, out uint alignment);
        return alignment;
    }

    private static uint GetMaxSize()
    {
        Gl.GetInteger(GetPName.MaxUniformBlockSize, out uint blockMaxSize);
        return blockMaxSize;
    }

    public override void BindShader(ShaderProgramBase p)
    {
        if (!p.GetUniformBlockIndex(BlockName, out uint index))
            return;
        BindBuffer(index);
        p.SetUniformBinding(BoundIndex, BoundIndex);
    }
}
