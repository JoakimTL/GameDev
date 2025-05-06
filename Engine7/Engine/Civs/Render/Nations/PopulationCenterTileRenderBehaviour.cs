using Civs.Render.World.Shaders;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Standard.Render;
using Civs.Logic.Nations;
using Engine.Module.Entities.Container;

namespace Civs.Render.Nations;
public sealed class PopulationCenterTileRenderBehaviour : DependentRenderBehaviourBase<PopulationCenterArchetype>, IInitializable {

	private PopulationCenterTileGroupSceneInstance _sceneInstance = null!;
	private bool _needsMeshUpdate = false;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	protected override void OnArchetypeSet() {
		base.OnArchetypeSet();
		Archetype.TileOwnership.ComponentChanged += OnTileOwnershipChanged;
	}

	private void OnTileOwnershipChanged( ComponentBase component ) {
		_needsMeshUpdate = true;
	}

	public void Initialize() {
		_sceneInstance = RenderEntity.RequestSceneInstance<PopulationCenterTileGroupSceneInstance>( RenderConstants.GameObjectSceneName, 0 );
		_sceneInstance.SetShaderBundle( RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<GlobeTerrainShaderBundle>() );
		_sceneInstance.SetVertexArrayObject( RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity3SceneData>() );
		_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
		_needsMeshUpdate = true;
	}

	public override void Update( double time, double deltaTime ) {
		if (_sceneInstance.Allocated && _needsMeshUpdate) {
			var parent = this.Archetype.Entity.Parent;
			_sceneInstance.UpdateMesh( [ .. this.Archetype.TileOwnership.OwnedFaces ], RenderEntity.ServiceAccess.MeshProvider, parent?.GetComponentOrDefault<PlayerComponent>()?.MapColor ?? 1 );
			_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
			_needsMeshUpdate = false;
		}
	}

	protected override bool InternalDispose() {
		Archetype.TileOwnership.ComponentChanged -= OnTileOwnershipChanged;
		return true;
	}
}
