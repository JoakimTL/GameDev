using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Sandbox.Logic.World;

namespace Sandbox.Render.World;

public sealed class WorldTilingBehaviour : DependentRenderBehaviourBase<WorldArchetype>, IInitializable {

	private SceneInstanceCollection<Vertex3, Entity2SceneData>? _sceneInstanceCollection;
	private readonly List<WorldTileSceneInstance> _instances = [];

	public void Initialize() {
		if (!RenderEntity.TryGetBehaviour( out WorldTileLoDBehaviour? lodBehaviour ))
			throw new Exception( $"Needs {nameof( WorldTileLoDBehaviour )} to function!" );
		_sceneInstanceCollection = RenderEntity.RequestSceneInstanceCollection<Vertex3, Entity2SceneData, TestShaderBundle>( "gameObjects", 0 );
		foreach (RenderedFoundationTile tile in lodBehaviour.Tiles) {
			WorldTileSceneInstance instance = _sceneInstanceCollection.Create<WorldTileSceneInstance>();
			instance.SetRenderTile( tile );
			_instances.Add( instance );
		}
	}

	public override void Update( double time, double deltaTime ) {
		foreach (WorldTileSceneInstance instance in _instances)
			instance.Update( RenderEntity.ServiceAccess.MeshProvider );
	}

	protected override bool InternalDispose() {
		return true;
	}
}

public sealed class WorldTileLoD {



}