using Engine.Rendering.Standard;

namespace Engine.Rendering.Lighting.Directional;

[Identification( "00559aa3-9968-464f-a6ee-df011921eddf" )]
public class DirectionalLightShaderBundle : ShaderBundle {
	public DirectionalLightShaderBundle() : base( (0, Resources.Render.Shader.Pipelines.Get<DirectionalLightShader>()) ) { }
	public override bool UsesTransparency => true;
}

public class DirectionalLightShader : ShaderPipeline {
	public DirectionalLightShader() : base( typeof( DirectionalLightProgramVertex ), typeof( DirectionalLightProgramFragment ) ) { }
}

public class DirectionalLightProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "light/directional.vert" ] );
}

[Identification( "f78943b6-984d-432f-b8ee-127c3d118f9c" )]
public class DirectionalShadowedLightShaderBundle : ShaderBundle {
	public DirectionalShadowedLightShaderBundle() : base( (0, Resources.Render.Shader.Pipelines.Get<DirectionalShadowedLightShader>()) ) { }
	public override bool UsesTransparency => true;
}

public class DirectionalShadowedLightShader : ShaderPipeline {
	public DirectionalShadowedLightShader() : base( typeof( DirectionalLightProgramVertex ), typeof( DirectionalShadowedLightProgramFragment ) ) { }
}

public class DirectionalShadowedLightProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "light/directionalShadow.frag" ] );
}

public class DirectionalLightProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "light/directionalNoShadow.frag" ] );
}