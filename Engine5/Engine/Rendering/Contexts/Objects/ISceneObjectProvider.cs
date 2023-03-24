using Engine.Rendering.Contexts.Objects.Assets;

namespace Engine.Rendering.Contexts.Objects;

public interface ISceneObjectAssetsProvider {
	event Action<ISceneObjectAssetsProvider>? Changed;
	MaterialAsset? MaterialAsset { get; }
	MeshDataAsset? MeshDataAsset { get; } 
}
