using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Shaders;
public class BlurRedShader : ShaderPipeline {
	public BlurRedShader() : base( typeof( PfxShaderProgramVertex ), typeof( BlurRedShaderProgramFragment ) ) { }
}

[StructLayout( LayoutKind.Sequential )]
public struct BlurRedShaderUniformBlock {
	public ulong TextureHandle;
	public Vector2 TextureSize;
	public Vector2 BlurVector;
}

public class BlurRedShaderProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "pfx/blurRed.frag" ] );
}