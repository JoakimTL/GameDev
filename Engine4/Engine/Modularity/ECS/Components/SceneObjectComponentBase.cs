using Engine.Rendering.Standard;

namespace Engine.Modularity.ECS.Components;


[OverrideType( typeof( SceneObjectComponentBase ) )]
public abstract class SceneObjectComponentBase : Component { }

public abstract class SceneObjectComponentBase<V, SD> : SceneObjectComponentBase where V : unmanaged where SD : unmanaged {

	public SD SceneData { get; private set; }
	public BufferedMesh? Mesh { get; private set; }
	public ShaderBundle? ShaderBundle { get; private set; }

	public SceneObjectComponentBase( BufferedMesh mesh, ShaderBundle shaderBundle ) {
		this.Mesh = mesh;
		this.ShaderBundle = shaderBundle;
	}

	protected void SetSceneData( SD sceneData ) {
		this.SceneData = sceneData;
		TriggerChanged();
	}

	protected void SetMesh( BufferedMesh mesh ) {
		this.Mesh = mesh;
		TriggerChanged();
	}

	protected void SetShaderBundle( ShaderBundle shaderBundle ) {
		this.ShaderBundle = shaderBundle;
		TriggerChanged();
	}
}