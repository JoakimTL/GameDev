using Engine.Module.Render;
using Engine.Module.Render.Input;

namespace Engine.Standard.Render.UserInterface;

public sealed class UserInterfaceService : DisposableIdentifiable, IUpdateable {
	private readonly UserInterfaceStateManager _userInterfaceStateManager;
	private readonly CapturableUserInputEventService _capturableUserInputEventService;
	private readonly UserInterfaceServiceAccess _userInterfaceServiceAccess;

	public UserInterfaceStateManager UserInterfaceStateManager => _userInterfaceStateManager;

	public UserInterfaceService( GameStateProvider gameStateProvider, CapturableUserInputEventService capturableUserInputEventService, RenderServiceAccess renderServiceAccess ) {
		this._capturableUserInputEventService = capturableUserInputEventService;
		this._userInterfaceServiceAccess = new( renderServiceAccess, "ui" );
		_userInterfaceStateManager = new( _userInterfaceServiceAccess, gameStateProvider );
		this._capturableUserInputEventService.AddListener( _userInterfaceStateManager );
	}

	public void Update( double time, double deltaTime ) {
		this._userInterfaceStateManager.Update( time, deltaTime );
	}

	protected override bool InternalDispose() {
		this._capturableUserInputEventService.RemoveListener( _userInterfaceStateManager );
		return true;
	}
}
