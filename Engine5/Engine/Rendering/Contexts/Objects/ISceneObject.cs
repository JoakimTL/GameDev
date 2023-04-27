using Engine.Rendering.Contexts.Objects.Scenes;

namespace Engine.Rendering.Contexts.Objects;

public interface ISceneObject {

	VertexArrayObjectBase? VertexArrayObject { get; }
	ShaderBundleBase? ShaderBundle { get; }
	ulong Uid { get; }
	ulong SortingIndex { get; }
	bool Valid { get; }
	uint Layer { get; }
	bool TryGetIndirectCommand( out IndirectCommand validCommand );
	void Bind();
	event Action<ISceneObject>? RenderPropertiesChanged;
	event Action<ISceneObject>? SceneObjectDisposed;
}
