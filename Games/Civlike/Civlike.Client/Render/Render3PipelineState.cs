using Engine;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render;
using Engine.Standard.Render.SingleTextureRendering;
using OpenGL;

namespace Civlike.Client.Render;

public sealed class Render3PipelineState : DisposableIdentifiable {

	public readonly UniformBlock SceneCamera;
	public readonly DataBlockCollection DataBlocks;

	public readonly BufferedSceneRenderer TerrainSceneRenderer;
	public readonly BufferedSceneRenderer GameObjectSceneRenderer;
	public readonly BufferedMultisampledSceneRenderer GridSceneRenderer;

	public Render3PipelineState( WindowService windowService, DataBlockService dataBlockService, SceneService sceneService, CameraService cameraService, TextureRenderingService textureRenderingService, FramebufferStateService framebufferStateService ) {
		this.SceneCamera = dataBlockService.CreateUniformBlockOrThrow( nameof( SceneCameraBlock ), 256, [ ShaderType.VertexShader ] );
		this.DataBlocks = new DataBlockCollection( this.SceneCamera );

		this.TerrainSceneRenderer = new BufferedSceneRenderer( sceneService.GetScene( RenderConstants.TerrainSceneName ), windowService, framebufferStateService );
		this.GameObjectSceneRenderer = new BufferedSceneRenderer( sceneService.GetScene( RenderConstants.GameObjectSceneName ), windowService, framebufferStateService );
		this.GridSceneRenderer = new BufferedMultisampledSceneRenderer( sceneService.GetScene( RenderConstants.GridSceneName ), windowService, framebufferStateService );
		cameraService.Main.Projection3.ZNear = 0.00048828125F;
		cameraService.Main.Projection3.ZFar = 32;
	}

	protected override bool InternalDispose() {
		return true;
	}
}
