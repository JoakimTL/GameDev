using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Lighting.Directional;
public class DirectionalNoShadowLightRender : DirectionalLightRenderBase<DirectionalLightData> {
	public DirectionalNoShadowLightRender( DirectionalLight light ) : base( light ) {
		SetShaders( Resources.Render.Shader.Bundles.Get<DirectionalLightShaderBundle>() );
		SetSceneData();
	}

	protected override DirectionalLightData GetSceneData() => new() {
		Color = this.Light.Color,
		Intensity = this.Light.Intensity,
		Direction = this.Light.Direction
	};
}