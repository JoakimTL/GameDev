using OpenGL;

namespace Engine.Rendering.Contexts.Objects;

public interface ISceneRender {
	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType prim = PrimitiveType.Triangles );
}
