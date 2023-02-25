using OpenGL;

namespace Engine.Rendering.Contexts.Objects;

public interface ISceneRender {
	public void Render( IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, uint shaderUse = 0, PrimitiveType prim = PrimitiveType.Triangles );
}
