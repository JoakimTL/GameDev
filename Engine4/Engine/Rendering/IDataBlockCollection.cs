namespace Engine.Rendering;

public interface IDataBlockCollection {
	void DirectBindShader( ShaderPipeline s );
	void DirectUnbindBuffers();
}