using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;

namespace Engine.Module.Render.Ogl.Scenes;

public sealed class RenderStage {
	public OglVertexArrayObjectBase VertexArrayObject { get; }
	public ShaderBundleBase ShaderBundle { get; }
	public uint StageCommandStart { get; }
	public int StageCommandCount { get; }

	public RenderStage( OglVertexArrayObjectBase vertexArrayObject, ShaderBundleBase shaderBundle, uint stageCommandStart, int stageCommandCount ) {
		this.VertexArrayObject = vertexArrayObject;
		this.ShaderBundle = shaderBundle;
		this.StageCommandStart = stageCommandStart;
		this.StageCommandCount = stageCommandCount;
	}
}
