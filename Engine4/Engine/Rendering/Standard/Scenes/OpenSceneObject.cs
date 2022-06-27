namespace Engine.Rendering.Standard.Scenes;

public class OpenSceneObject<V, SD> : ClosedSceneObject<V, SD> where V : unmanaged where SD : unmanaged {
	public event Action? OnBind;

	public new void SetMesh( BufferedMesh? newMesh ) => base.SetMesh( newMesh );

	public new void SetSceneData( SceneInstanceData<SD>? newSceneData ) => base.SetSceneData( newSceneData );

	public new void SetShaders( ShaderBundle? newShaderBundle ) => base.SetShaders( newShaderBundle );
	public void SetShaders<T>() where T : ShaderBundle => base.SetShaders( Resources.Render.Shader.Bundles.Get<T>() );
	public override sealed void Bind() => OnBind?.Invoke();
}
