using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;

namespace Engine.Rendering;
public interface ISceneObject {
	VertexArrayObject VertexArrayObject { get; }
	ShaderBundle? ShaderBundle { get; }
	ulong ID { get; }
	ulong SortingIndex { get; }
	bool Valid { get; }
	bool HasTransparency { get; }
	uint Layer { get; }
	bool TryGetIndirectCommand( out IndirectCommand? validCommand );
	event Action<ISceneObject>? RenderPropertiesChanged;
	event Action<ISceneObject>? SceneObjectDisposed;
}
