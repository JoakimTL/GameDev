using Engine;
using Engine.Module.Render.Entities;
using Engine.Standard.Render;
using Engine.Module.Entities.Container;
using Civlike.Client.Render.World.Shaders;
using Civlike.Logic.Nations.ECS;
using Civlike.World.Render.Shaders;

namespace Civlike.Client.Render.Nations;
public sealed class PopulationCenterTileRenderBehaviour : DependentRenderBehaviourBase<PopulationCenterArchetype>, IInitializable {

	//private PlayerComponent? _localPlayer;
	private PopulationCenterTileGroupSceneInstance _sceneInstance = null!;
	private bool _needsMeshUpdate = false;
	//private bool _localPlayerChanged;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	protected override void OnArchetypeSet() {
		base.OnArchetypeSet();
		this.Archetype.TileOwnership.ComponentChanged += OnTileOwnershipChanged;
	}

	private void OnTileOwnershipChanged( ComponentBase component ) {
		this._needsMeshUpdate = true;
	}

	public void Initialize() {
		this._sceneInstance = this.RenderEntity.RequestSceneInstance<PopulationCenterTileGroupSceneInstance>( RenderConstants.GameObjectSceneName, 0 );
		this._sceneInstance.SetShaderBundle( this.RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<GlobeTerrainShaderBundle>() );
		this._sceneInstance.SetVertexArrayObject( this.RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity3SceneData>() );
		this._sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
		this._needsMeshUpdate = true;

	}

	public override void Update( double time, double deltaTime ) {
		//if (_localPlayer is null)
		//	SetLocalPlayer();
		if (this._sceneInstance.Allocated && this._needsMeshUpdate) {
			Entity? parent = this.Archetype.Entity.Parent;
			//this._sceneInstance.UpdateMesh( [ .. this.Archetype.TileOwnership.OwnedTiles ], this.RenderEntity.ServiceAccess.MeshProvider, parent?.GetComponentOrDefault<PlayerComponent>()?.MapColor ?? 1 );
			this._sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
			this._needsMeshUpdate = false;
		}
	}

	//private void SetLocalPlayer() {
	//	if (_localPlayer is not null)
	//		return;
	//	SynchronizedEntityContainer? container = RenderEntity.ServiceAccess.Get<SynchronizedEntityContainerProvider>().SynchronizedContainers.FirstOrDefault();
	//	if (container is null)
	//		return;
	//	Guid? localPlayerId = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Guid?>( "localPlayerId" );
	//	if (!localPlayerId.HasValue)
	//		return;
	//	_localPlayer = container.SynchronizedEntities
	//		.Select(p => p.EntityCopy)
	//		.OfType<Entity>()
	//		.Select(p => p.GetComponentOrDefault<PlayerComponent>())
	//		.OfType<PlayerComponent>()
	//		.FirstOrDefault(p => p.Entity.EntityId == localPlayerId.Value);
	//	if (_localPlayer is null)
	//		return;
	//	_localPlayer.ComponentChanged += OnLocalPlayerChanged;
	//}

	//private void OnLocalPlayerChanged( ComponentBase component ) {
	//	_localPlayerChanged = true;
	//}

	protected override bool InternalDispose() {
		this.Archetype.TileOwnership.ComponentChanged -= OnTileOwnershipChanged;
		return true;
	}
}
