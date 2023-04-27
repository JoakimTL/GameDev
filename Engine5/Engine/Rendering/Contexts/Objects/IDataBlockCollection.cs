namespace Engine.Rendering.Contexts.Objects;

public interface IDataBlockCollection {
	void BindShader( ShaderPipelineBase s );
	void UnbindBuffers();
}
