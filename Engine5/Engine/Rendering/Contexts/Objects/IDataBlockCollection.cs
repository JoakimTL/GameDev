namespace Engine.Rendering.Contexts.Objects;

public interface IDataBlockCollection {
	void DirectBindShader( ShaderPipelineBase s );
	void DirectUnbindBuffers();
}
