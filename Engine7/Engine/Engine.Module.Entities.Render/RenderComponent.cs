namespace Engine.Module.Entities.Render;

/// <summary>
/// If an entity has this component any rendering system will attempt to render it if the current scene matches the <see cref="SceneName"/>.
/// </summary>
public sealed class RenderComponent : ComponentBase {

	public string SceneName { get; private set; }

	public RenderComponent() {
		this.SceneName = "Default";
	}

	public void SetSceneName( string sceneName ) {
		this.SceneName = sceneName;
		InvokeComponentChanged();
	}
}
