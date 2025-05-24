using Civlike.Logic.Nations;
using Civlike.World.GameplayState;
using Engine;
using Engine.Module.Entities.Container;
using Engine.Module.Render.Entities.Providers;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard;

namespace Civlike.Client.Render;

public sealed class CameraPanOnTribeCreated( CameraService cameraService, ActiveGlobeTrackingService activeGlobeTrackingService, SynchronizedEntityContainerProvider synchronizedEntityContainerProvider, GameStateProvider gameStateProvider ) : IInitializable, IUpdateable {
	private readonly CameraService _cameraService = cameraService;
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;
	private readonly SynchronizedEntityContainerProvider _synchronizedEntityContainerProvider = synchronizedEntityContainerProvider;
	private readonly GameStateProvider _gameStateProvider = gameStateProvider;
	private bool _newGlobe = false;

	public void Initialize() {
		this._activeGlobeTrackingService.AfterGlobeChange += AfterGlobeChanged;
	}

	private void AfterGlobeChanged( Globe? model ) {
		this._gameStateProvider.SetNewState( "startLocation", null );
		this._newGlobe = model is not null;
	}

	public void Update( double time, double deltaTime ) {
		if (!this._newGlobe)
			return;
		SynchronizedEntityContainer? container = this._synchronizedEntityContainerProvider.SynchronizedContainers.FirstOrDefault();
		if (container is null)
			return;
		Guid? localPlayer = this._gameStateProvider.Get<Guid?>( "localPlayerId" );
		if (!localPlayer.HasValue)
			return;
		Entity? popCenter = container.SynchronizedEntities
			.Select( p => p.EntityCopy )
			.OfType<Entity>()
			.Where( p => p.ParentId == localPlayer.Value && p.IsArchetype<PopulationCenterArchetype>() )
			.FirstOrDefault();
		if (popCenter is null)
			return;
		this._gameStateProvider.SetNewState( "startLocation", popCenter.GetComponentOrThrow<FaceOwnershipComponent>().OwnedFaces.Single().Blueprint.GetCenter() );
		this._newGlobe = false;
	}
}
