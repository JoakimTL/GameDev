using Civs.Render.World.Shaders;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Standard.Render;
using Civs.Logic.Nations;
using Engine.Module.Entities.Container;
using Engine.Module.Render.Entities.Providers;
using Engine.Standard;

namespace Civs.Render.Nations;
public sealed class PopulationCenterTileRenderBehaviour : DependentRenderBehaviourBase<PopulationCenterArchetype>, IInitializable {

	private PlayerComponent? _localPlayer;
	private PopulationCenterTileGroupSceneInstance _sceneInstance = null!;
	private bool _needsMeshUpdate = false;
	private bool _localPlayerChanged;

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
		if (_localPlayer is null)
			SetLocalPlayer();
		if (_sceneInstance.Allocated && _needsMeshUpdate) {
			Entity? parent = this.Archetype.Entity.Parent;
			_sceneInstance.UpdateMesh( [ .. this.Archetype.TileOwnership.OwnedFaces ], RenderEntity.ServiceAccess.MeshProvider, parent?.GetComponentOrDefault<PlayerComponent>()?.MapColor ?? 1 );
			_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
			_needsMeshUpdate = false;
		}
	}

	private void SetLocalPlayer() {
		if (_localPlayer is not null)
			return;
		SynchronizedEntityContainer? container = RenderEntity.ServiceAccess.Get<SynchronizedEntityContainerProvider>().SynchronizedContainers.FirstOrDefault();
		if (container is null)
			return;
		Guid? localPlayerId = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Guid?>( "localPlayerId" );
		if (!localPlayerId.HasValue)
			return;
		_localPlayer = container.SynchronizedEntities
			.Select(p => p.EntityCopy)
			.OfType<Entity>()
			.Select(p => p.GetComponentOrDefault<PlayerComponent>())
			.OfType<PlayerComponent>()
			.FirstOrDefault(p => p.Entity.EntityId == localPlayerId.Value);
		if (_localPlayer is null)
			return;
		_localPlayer.ComponentChanged += OnLocalPlayerChanged;
	}

	private void OnLocalPlayerChanged( ComponentBase component ) {
		_localPlayerChanged = true;
	}

	protected override bool InternalDispose() {
		Archetype.TileOwnership.ComponentChanged -= OnTileOwnershipChanged;
		return true;
	}
}
