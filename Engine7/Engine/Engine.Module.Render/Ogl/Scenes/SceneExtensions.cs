using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;

namespace Engine.Module.Render.Ogl.Scenes;

public static class SceneExtensions {
	public static ulong? GetBindIndexWith( this OglVertexArrayObjectBase vertexArrayObject, ShaderBundleBase shaderBundle ) => vertexArrayObject.VertexArrayId | (ulong) shaderBundle.BundleID << 32;
	public static ulong? GetBindIndexWith( this ShaderBundleBase shaderBundle, OglVertexArrayObjectBase vertexArrayObject ) => vertexArrayObject.VertexArrayId | (ulong) shaderBundle.BundleID << 32;
}
