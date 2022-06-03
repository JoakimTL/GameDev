using OpenGL;

namespace Engine.Rendering;

public interface IScene {
	public void Render( IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, uint shaderUse = 0, PrimitiveType prim = PrimitiveType.Triangles );
}
