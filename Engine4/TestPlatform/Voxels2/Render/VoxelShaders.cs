using Engine;
using Engine.Rendering;
using Engine.Rendering.Standard;

namespace TestPlatform.Voxels2.Render;

[Identification( "b8ec47e5-d428-42fa-a19a-c4c0427648a2" )]
public class VoxelShaderBundle : ShaderBundle {
	public VoxelShaderBundle() : base( 
		(0, Resources.Render.Shader.Pipelines.GetOrAdd<VoxelShader>()),
		(1, Resources.Render.Shader.Pipelines.GetOrAdd<VoxelDirectionalShader>()) ) { }
	public override bool UsesTransparency => false;
}

[Identification( "b9200a9f-0999-4e76-872d-b242f76ecc1d" )]
public class VoxelTransparentShaderBundle : ShaderBundle {
	public VoxelTransparentShaderBundle() : base( 
		(0, Resources.Render.Shader.Pipelines.GetOrAdd<VoxelTransparentShader>()),
		(1, Resources.Render.Shader.Pipelines.GetOrAdd<VoxelDirectionalTransparentShader>()) ) { }
	public override bool UsesTransparency => true;
}

public class VoxelShader : ShaderPipeline {
	public VoxelShader() : base( typeof( VoxelProgramVertex ), typeof( VoxelProgramFragment ) ) { }
}

public class VoxelDirectionalShader : ShaderPipeline {
	public VoxelDirectionalShader() : base( typeof( VoxelProgramVertex ), typeof( VoxelDirectionalProgramFragment ) ) { }
}

public class VoxelTransparentShader : ShaderPipeline {
	public VoxelTransparentShader() : base( typeof( VoxelProgramVertex ), typeof( VoxelTransparentProgramFragment ) ) { }
}

public class VoxelDirectionalTransparentShader : ShaderPipeline {
	public VoxelDirectionalTransparentShader() : base( typeof( VoxelProgramVertex ), typeof( VoxelDirectionalTransparentProgramFragment ) ) { }
}

public class VoxelProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/voxels2.vert" ] );
}

public class VoxelProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/voxels.frag" ] );
}

public class VoxelDirectionalProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/voxelsDirectional.frag" ] );
}

public class VoxelTransparentProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/voxelsTransparent.frag" ] );
}

public class VoxelDirectionalTransparentProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/voxelsDirectionalTransparent.frag" ] );
}