namespace Engine.Module.Render.Ogl.Services;

public sealed class CameraService( WindowService windowService ) : IInitializable {
	private readonly WindowService _windowService = windowService;

	private readonly Dictionary<string, CameraSuite> _cameras = [];

	private CameraSuite? _mainCamera;

	public CameraSuite Main => _mainCamera ?? throw new InvalidOperationException( "Main camera not initialized" );

	public void Initialize() {
		_mainCamera = new( "main", _windowService );
	}

	public CameraSuite Get( string cameraName ) {
		if (_cameras.TryGetValue( cameraName, out CameraSuite? camera ))
			return camera;
		camera = new( cameraName, _windowService );
		_cameras.Add( cameraName, camera );
		return camera;
	}
}