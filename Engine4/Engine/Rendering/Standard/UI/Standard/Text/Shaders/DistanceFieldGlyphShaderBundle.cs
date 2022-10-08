namespace Engine.Rendering.Standard.UI.Standard.Text.Shaders;

[Identification( "59150a91-036e-4cfb-ba6c-bd2ae1f3232f" )]
public class DistanceFieldGlyphShaderBundle : ShaderBundle {
	public DistanceFieldGlyphShaderBundle() : base( (0, Resources.Render.Shader.Pipelines.GetOrAdd<DistanceFieldGlyphShaderPipeline>()) ) { }

	public override bool UsesTransparency => true;
}

public class DistanceFieldGlyphShaderPipeline : ShaderPipeline {
	public DistanceFieldGlyphShaderPipeline() : base( typeof( DistanceFieldGlyphShaderProgramVertex ), typeof( DistanceFieldGlyphShaderProgramFragment ) ) { }
}

public class DistanceFieldGlyphShaderProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "ui/textShader.vert" ] );
}

public class DistanceFieldGlyphShaderProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "ui/textShader.frag" ] );
}