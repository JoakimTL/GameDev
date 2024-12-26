using Engine.Graphics.Objects.Default.SceneObjects;

namespace Engine.Graphics {
	public delegate void ShaderBind( Objects.Shader s );
	public delegate void RenderMethod<T>( Objects.SceneObject<T> entity, Objects.Shader s, Objects.IView view ) where T : SceneObjectData;
}
