using Engine.Module.Render.Ogl.OOP.Shaders;

namespace Engine.Module.Render.Ogl.Scenes;

public interface IDataBlockCollection {
	void BindShader( OglShaderPipelineBase s );
	void UnbindBuffers();
}
