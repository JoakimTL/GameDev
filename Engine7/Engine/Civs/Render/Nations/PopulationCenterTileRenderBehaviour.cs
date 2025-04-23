using Civs.Render.World.Shaders;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Standard.Render;
using Civs.Logic.Nations;

namespace Civs.Render.Nations;
public sealed class PopulationCenterTileRenderBehaviour : DependentRenderBehaviourBase<PopulationCenterArchetype>, IInitializable {

	private TileGroupSceneInstance _sceneInstance = null!;
	private bool _needsMeshUpdate = false;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	protected override void OnArchetypeSet() {
		base.OnArchetypeSet();
		Archetype.TileOwnershipComponent.TileOwnershipChanged += OnTileOwnershipChanged;
	}

	private void OnTileOwnershipChanged() {
		_needsMeshUpdate = true;
	}

	public void Initialize() {
		_sceneInstance = RenderEntity.RequestSceneInstance<TileGroupSceneInstance>( RenderConstants.GameObjectSceneName, 0 );
		_sceneInstance.SetShaderBundle( RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<GlobeTerrainShaderBundle>() );
		_sceneInstance.SetVertexArrayObject( RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity3SceneData>() );
		_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
		_needsMeshUpdate = true;
	}

	public override void Update( double time, double deltaTime ) {
		//TODO: properly separate logic and render in all components. If an update happens which should be rendered occurs, it's the game logic's (ecs) responsibility to serialize and ship the new components to the render system. This means behaviours can in reality only depend on serializable components.
		if (_sceneInstance.Allocated && _needsMeshUpdate) {
			_sceneInstance.UpdateMesh( this.Archetype.TileOwnershipComponent.OwnedTiles, RenderEntity.ServiceAccess.MeshProvider, this.Archetype.TileOwnershipRenderComponent.Color );
			_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
			_needsMeshUpdate = false;
		}
	}

	protected override bool InternalDispose() {
		Archetype.TileOwnershipComponent.TileOwnershipChanged -= OnTileOwnershipChanged;
		return true;
	}
}
