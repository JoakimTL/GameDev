using System.Numerics;
using Engine.Rendering.Standard;

namespace Engine.Rendering.Lighting.Directional;

public abstract class DirectionalLightRenderBase<SD> : ClosedSceneObject<Vector2, SD> where SD : unmanaged {
	public DirectionalLight Light { get; }

	public DirectionalLightRenderBase( DirectionalLight light ) {
		this.Light = light;
		SetMesh( Resources.Render.Mesh2.SquarePFX );
		SetSceneData( new SceneInstanceData<SD>( 1, 1 ) );
		light.Changed += LightChanged;
	}

	protected void SetSceneData() => LightChanged();
	protected abstract SD GetSceneData();

	private void LightChanged() => this.SceneData?.SetInstance( 0, GetSceneData() );

}
