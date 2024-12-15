using Engine.Module.Entities.Render;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Entities.Providers;

public sealed class CameraProvider(CameraService cameraService ) : IRenderEntityServiceProvider {
	public CameraSuite Main => cameraService.Main;
	public CameraSuite Get( string cameraName ) => cameraService.Get( cameraName );
}