namespace Engine.Rendering.Contexts.Objects.Scenes;

public class SceneRenderStage {

	public readonly VertexArrayObjectBase VAO;
	public readonly ShaderBundleBase Shaders;
	/// <summary>
	/// The number of commands in this stage
	/// </summary>
	public readonly int CommandCount;
	/// <summary>
	/// The offset of the drawcall!
	/// </summary>
	public readonly uint Offset;

	public SceneRenderStage( VertexArrayObjectBase vao, ShaderBundleBase bundle, int commandCount, uint offset ) {
		VAO = vao;
		Shaders = bundle;
		CommandCount = commandCount;
		Offset = offset;
	}

}
