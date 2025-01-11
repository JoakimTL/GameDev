namespace Engine.Module.Render.Ogl.OOP.Framebuffers;

/*
 * 

	public void Initialize() {
		_uiScene = _sceneService.GetScene( "ui" );
		_uiSceneCamera = _dataBlockService.CreateUniformBlockOrThrow( nameof( SceneCameraBlock ), 256, [ ShaderType.VertexShader ] );
		_cameraSuite = _cameraService.Get( "ui" );
		_dataBlockCollection = new( _uiSceneCamera );
		_framebuffer = _framebufferStateService.CreateFramebuffer( _windowService.Window.Size );
		_framebuffer.OnFramebufferGeneration += UiFramebufferGeneration;
		_copyFramebuffer = _framebufferStateService.CreateFramebuffer( _windowService.Window.Size );
		_copyFramebuffer.OnFramebufferGeneration += CopyFramebufferGeneration;
		_windowService.Window.OnResized += OnWindowResized;
	}

	private void CopyFramebufferGeneration( OglFramebuffer framebuffer ) {
		_copyTextureReference = null;
		_copyFramebufferTexture?.Dispose();
		_copyFramebufferTexture = new( "copyFrameBuffer", TextureTarget.Texture2d, _windowService.Window.Size, InternalFormat.Rgba8, (TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear), (TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear) );
		_copyFramebuffer.AttachTexture( FramebufferAttachment.ColorAttachment0, _copyFramebufferTexture.TextureID, 0 );
		_copyFramebuffer.EnableCurrentColorAttachments();
	}

	private void UiFramebufferGeneration( OglFramebuffer framebuffer ) {
		_textureReference = null;
		_framebufferTexture?.Dispose();
		_framebufferTexture = new OglMultisampledTexture( "uiFramebufferColor", TextureTarget.Texture2dMultisample, _framebuffer.Size, 4, InternalFormat.Rgba8, true );
		framebuffer.AttachTexture( FramebufferAttachment.ColorAttachment0, _framebufferTexture.TextureID, 0 );
		framebuffer.EnableCurrentColorAttachments();
	}

	private void OnWindowResized( IResizableSurface<int> surface ) {
		if (surface.Size == 0)
			return;
		_framebuffer.DetachTexture( FramebufferAttachment.ColorAttachment0 );
		_framebuffer.Resize( surface.Size );
		_copyFramebuffer.DetachTexture( FramebufferAttachment.ColorAttachment0 );
		_copyFramebuffer.Resize( surface.Size );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		Gl.Enable( EnableCap.Blend );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		Gl.Enable( EnableCap.Multisample );
		Vector2<float> cameraRotationRight = new( float.Cos( this._cameraSuite.View2.Rotation ), float.Sin( this._cameraSuite.View2.Rotation ) );
		Vector2<float> cameraRotationUp = new( -cameraRotationRight.Y, cameraRotationRight.X );
		_uiSceneCamera.Buffer.Write( 0u, new SceneCameraBlock( _cameraSuite.Camera2.Matrix, (cameraRotationUp.X, cameraRotationUp.Y, 0), (cameraRotationRight.X, cameraRotationRight.Y, 0) ) );
		_framebufferStateService.BindFramebuffer( FramebufferTarget.Framebuffer, _framebuffer );
		_framebuffer.Clear( OpenGL.Buffer.Color, 0, [ 0 ] );
		_uiScene.Render( "default", _dataBlockCollection, null, PrimitiveType.Triangles );
		_framebufferStateService.UnbindFramebuffer( FramebufferTarget.Framebuffer );
		_framebufferStateService.BlitToFrameBuffer( _framebuffer, _copyFramebuffer, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear );
	}

	public void DrawToScreen() {
		//Draw the framebuffer to the screen.
		Gl.Enable( EnableCap.Blend );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		_copyTextureReference ??= _copyFramebufferTexture.GetTextureReference();
		_textureRenderingService.RenderTexture( _copyTextureReference.GetHandle() );
	}
 */

public sealed class AutoscalingFramebuffer : OglFramebuffer {

	private readonly IResizableSurface<int> _surface;
	public float ScalingFactor { get; }

	public AutoscalingFramebuffer( IResizableSurface<int> surface, float scalingFactor ) : base( surface.Size ) {
		this._surface = surface;
		this.ScalingFactor = scalingFactor;
		_surface.OnResized += OnExternalSurfaceResized;
	}

	private void OnExternalSurfaceResized( IResizableSurface<int> surface )
		=> Resize( (surface.Size.CastSaturating<int, float>() * ScalingFactor)
			.Round<Vector2<float>, float>( 0, MidpointRounding.AwayFromZero )
			.CastSaturating<float, int>()
			.Max( 1 ) );

	protected override bool InternalDispose() {
		_surface.OnResized -= OnExternalSurfaceResized;
		return true;
	}
}
