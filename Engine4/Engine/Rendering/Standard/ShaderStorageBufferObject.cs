﻿using OpenGL;

namespace Engine.Rendering.Standard;
public class ShaderStorageBufferObject : DataBlock {

	/// <summary>
	/// Creates a new shader storage block, ready for use. This block has a unique binding location, and can be reused in multiple block collections.
	/// </summary>
	/// <param name="shaderTypes">The types of shaders this block pertains to.</param>
	/// <param name="blockName">The block name.</param>
	/// <param name="sizeBytes">The size of the uniform block in bytes. GLSL works in 4-byte alignment, meaning this number must be a multiple of 4!</param>
	public ShaderStorageBufferObject( string blockName, uint sizeBytes, params ShaderType[] shaderTypes ) : base( Resources.Render.VBOs.ShaderStorage, shaderTypes, blockName, sizeBytes, 0, 0 ) { }

	protected override BufferTarget Target => BufferTarget.ShaderStorageBuffer;

	public override void DirectBindShader( ShaderProgram p ) {
		if ( !p.GetShaderStorageBlockIndex( this.BlockName, out uint index ) )
			return;
		DirectBindBuffer( index );
		p.SetShaderStorageBinding( this.BoundIndex, this.BoundIndex );
	}
}
