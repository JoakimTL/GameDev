using Engine.Module.Entities.Render;
using Engine.Module.Render.Ogl.Services;
using Engine.Transforms;
using Engine.Transforms.Camera;

namespace Engine.Module.Render.Entities.Providers;

public sealed class CameraProvider(CameraService cameraService ) : IRenderEntityServiceProvider {
	public CameraSuite Main => cameraService.Main;
	public CameraSuite Get( string cameraName ) => cameraService.Get( cameraName );
}