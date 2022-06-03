using Engine;
using Engine.Rendering;
using Engine.Rendering.Standard;

namespace TestPlatform.Voxels.Rendering;

[Identification( "d39b31f7-05a1-43ef-829f-f05c4f99990d" )]
public class VoxelShaderBundle : ShaderBundle {
	public VoxelShaderBundle() : base( "Voxels", 
		(0, Resources.Render.Shader.Pipelines.Get<VoxelShader>()), 
		(1, Resources.Render.Shader.Pipelines.Get<VoxelDirectionalShader>()) ) { }
	public override bool UsesTransparency => false;
}

[Identification( "e4fc3b59-2a1e-4b37-897d-bd9fafa18f59" )]
public class VoxelTransparentShaderBundle : ShaderBundle {
	public VoxelTransparentShaderBundle() : base( "VoxelsTransparent",
		(0, Resources.Render.Shader.Pipelines.Get<VoxelTransparentShader>()),
		(1, Resources.Render.Shader.Pipelines.Get<VoxelDirectionalTransparentShader>()) ) { }
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
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/voxels.vert" ] );
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