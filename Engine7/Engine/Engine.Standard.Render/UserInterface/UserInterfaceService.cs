using Engine.Module.Entities.Services;
using Engine.Module.Render;
using Engine.Module.Render.Input;
using Engine.Processing;

namespace Engine.Standard.Render.UserInterface;

[Do<IUpdateable>.After<SynchronizedEntityContainerService>]
public sealed class UserInterfaceService : DisposableIdentifiable, IUpdateable {
	private readonly UserInterfaceStateManager _userInterfaceStateManager;
	private readonly CapturableUserInputEventService _capturableUserInputEventService;
	private readonly UserInterfaceServiceAccess _userInterfaceServiceAccess;

	public UserInterfaceStateManager UserInterfaceStateManager => this._userInterfaceStateManager;

	public UserInterfaceService( GameStateProvider gameStateProvider, CapturableUserInputEventService capturableUserInputEventService, RenderServiceAccess renderServiceAccess ) {
		this._capturableUserInputEventService = capturableUserInputEventService;
		this._userInterfaceServiceAccess = new( renderServiceAccess, "ui" );
		this._userInterfaceStateManager = new( this._userInterfaceServiceAccess, gameStateProvider );
		this._capturableUserInputEventService.AddListener( this._userInterfaceStateManager );
	}

	public void Update( double time, double deltaTime ) {
		this._userInterfaceStateManager.Update( time, deltaTime );
	}

	protected override bool InternalDispose() {
		this._capturableUserInputEventService.RemoveListener( this._userInterfaceStateManager );
		return true;
	}
}
