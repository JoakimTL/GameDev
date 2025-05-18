using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.Providers;

public sealed class CameraProvider( CameraService cameraService ) : IServiceProvider {
	public CameraSuite Main => cameraService.Main;
	public CameraSuite Get( string cameraName ) => cameraService.Get( cameraName );
}
