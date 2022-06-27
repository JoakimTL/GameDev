using System.Numerics;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Lighting.Point;
public abstract class PointLightRenderBase<SD> : ClosedSceneObject<Vector3, SD> where SD : unmanaged {
	public PointLight Light { get; }

	public PointLightRenderBase( PointLight light ) {
		this.Light = light;
		SetMesh( Resources.Render.Mesh3.CubePfx );
		SetSceneData( new SceneInstanceData<SD>( 1, 1 ) );
		light.Changed += LightChanged;
	}

	protected void SetSceneData() => LightChanged();
	protected abstract SD GetSceneData();

	private void LightChanged() => this.SceneData?.SetInstance( 0, GetSceneData() );
}
public class PointLightNoShadowRender : PointLightRenderBase<PointLightData> {
	public PointLightNoShadowRender( PointLight light ) : base( light ) {
		SetShaders( Resources.Render.Shader.Bundles.Get<PointLightNoShadowShaderBundle>() );
		SetSceneData();
	}

	protected override PointLightData GetSceneData() => new() {
		Color = this.Light.Color,
		Intensity = this.Light.Intensity,
		Radius = this.Light.Radius,
		Translation = this.Light.Translation
	};

	public override void Bind() { }
	protected override bool OnDispose() => true;
}

[Identification( "6d3f8379-1e29-4dc2-a3f6-c0a2297c8b83" )]
public class PointLightNoShadowShaderBundle : ShaderBundle {
	public PointLightNoShadowShaderBundle() : base( (0, Resources.Render.Shader.Pipelines.Get<PointLightNoShadowShader>()) ) { }
	public override bool UsesTransparency => true;
}

public class PointLightNoShadowShader : ShaderPipeline {
	public PointLightNoShadowShader() : base( typeof( PointLightProgramVertex ), typeof( PointLightNoShadowProgramFragment ) ) { }
}

public class PointLightProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "light/point.vert" ] );
}

public class PointLightNoShadowProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "light/pointNoShadow.frag" ] );
}
