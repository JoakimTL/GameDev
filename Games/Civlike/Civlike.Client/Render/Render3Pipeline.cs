using Engine;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render;
using Engine.Standard.Render.SingleTextureRendering;
using OpenGL;

namespace Civlike.Client.Render;

public sealed class Render3Pipeline( WindowService windowService, DataBlockService dataBlockService, SceneService sceneService, CameraService cameraService, TextureRenderingService textureRenderingService, FramebufferStateService framebufferStateService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {

	private readonly WindowService _windowService = windowService;
	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly SceneService _sceneService = sceneService;
	private readonly CameraService _cameraService = cameraService;
	private readonly TextureRenderingService _textureRenderingService = textureRenderingService;
	private readonly FramebufferStateService _framebufferStateService = framebufferStateService;

	private Render3PipelineState? _state = null!;

	public void Initialize() {
		_cameraService.Main.Projection3.FOV = 70;
		this._state = new( this._windowService, this._dataBlockService, this._sceneService, this._cameraService, this._textureRenderingService, this._framebufferStateService );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		if (this._state is null)
			throw new InvalidOperationException( $"{nameof( Render3Pipeline )} not initialized" );
		this._state.SceneCamera.Buffer.Write( 0u, new SceneCameraBlock( this._cameraService.Main.Camera3.Matrix, this._cameraService.Main.View3.Rotation.Up, -this._cameraService.Main.View3.Rotation.Left ) );
		this._windowService.Window.Title = $"Camera: {this._cameraService.Main.View3.Translation}";

		Gl.Disable( EnableCap.Blend );
		Gl.Enable( EnableCap.DepthTest );
		Gl.DepthMask( true );

		this._state.TerrainSceneRenderer.Render( "default", this._state.DataBlocks, null, PrimitiveType.Triangles );

		this._state.GameObjectSceneRenderer.Render( "default", this._state.DataBlocks, null, PrimitiveType.Triangles );

		//_state.GridSceneRenderer.BlitDepthBuffer( _state.TerrainSceneRenderer );

		Gl.Enable( EnableCap.Blend );
		Gl.Disable( EnableCap.DepthTest );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.DepthMask( false );

		this._state.GridSceneRenderer.Render( "default", this._state.DataBlocks, null, PrimitiveType.Triangles );

	}

	public void DrawToScreen() {
		if (this._state is null)
			throw new InvalidOperationException( $"{nameof( Render3Pipeline )} not initialized" );
		Gl.Enable( EnableCap.Blend );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );

		this._textureRenderingService.RenderTexture( this._state.TerrainSceneRenderer.DisplayTexture.TextureReference.GetHandle() );
		this._textureRenderingService.RenderTexture( this._state.GameObjectSceneRenderer.DisplayTexture.TextureReference.GetHandle() );
		this._textureRenderingService.RenderTexture( this._state.GridSceneRenderer.DisplayTexture.TextureReference.GetHandle() );

	}

	protected override bool InternalDispose() {
		this._state?.Dispose();
		return true;
	}
}