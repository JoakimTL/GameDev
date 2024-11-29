using OpenGL;

namespace Engine.Module.Render.Ogl.Scenes;

public interface ISceneRender {
	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType prim = PrimitiveType.Triangles );
}
