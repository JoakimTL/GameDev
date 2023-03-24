namespace Engine.Rendering.Contexts.Objects.Scenes;

public class SceneObjectUsage {
	public readonly string ShaderIndex;
	public readonly ISceneObject SceneObject;

	public SceneObjectUsage( ISceneObject sceneObject, string shaderIndex ) {
		SceneObject = sceneObject;
		ShaderIndex = shaderIndex;
	}
}
