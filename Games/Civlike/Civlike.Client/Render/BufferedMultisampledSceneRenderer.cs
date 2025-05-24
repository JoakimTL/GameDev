using Engine;
using Engine.Module.Render.Ogl.OOP.Framebuffers;
using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using OpenGL;

namespace Civlike.Client.Render;

public sealed class BufferedMultisampledSceneRenderer : Identifiable {
	public readonly Scene Scene;
	private readonly FramebufferStateService _framebufferStateService;

	public readonly AutoscalingFramebuffer MultisampledFramebuffer;
	public readonly AutoGeneratingFramebuffer MultisampledFramebufferGenerator;

	public readonly AutoscalingFramebuffer Framebuffer;
	public readonly AutoGeneratingFramebuffer FramebufferGenerator;

	public readonly FramebufferScaledTextureGenerator DisplayTexture;

	public BufferedMultisampledSceneRenderer( Scene scene, WindowService windowService, FramebufferStateService framebufferStateService, uint samples = 4 ) {
		this.Scene = scene;
		this._framebufferStateService = framebufferStateService;

		this.MultisampledFramebuffer = framebufferStateService.CreateAutoscalingFramebuffer( windowService.Window, 1 );
		this.MultisampledFramebufferGenerator = new( this.MultisampledFramebuffer );
		this.MultisampledFramebufferGenerator.AddTexture( FramebufferAttachment.ColorAttachment0, dimensions => new OglMultisampledTexture( $"{this.FullName}MultisampledColor", TextureTarget.Texture2dMultisample, dimensions, samples, InternalFormat.Rgba8, true ) );
		this.MultisampledFramebufferGenerator.AddRenderBuffer( FramebufferAttachment.DepthAttachment, InternalFormat.DepthComponent, samples );

		this.Framebuffer = framebufferStateService.CreateAutoscalingFramebuffer( windowService.Window, 1 );
		this.FramebufferGenerator = new( this.Framebuffer );
		this.DisplayTexture = this.FramebufferGenerator.AddTexture( FramebufferAttachment.ColorAttachment0, dimensions => new OglTexture( $"{this.FullName}Color", TextureTarget.Texture2d, dimensions, InternalFormat.Rgba8, (TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear), (TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear) ) );
	}

	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType primitiveType ) {
		this._framebufferStateService.BindFramebuffer( FramebufferTarget.Framebuffer, this.MultisampledFramebuffer );
		this.MultisampledFramebuffer.Clear( OpenGL.Buffer.Color, 0, [ 0 ] );
		this.MultisampledFramebuffer.Clear( OpenGL.Buffer.Depth, 0, [ 1f ] );
		this.Scene.Render( shaderIndex, dataBlocks, blendActivationFunction, primitiveType );
		this._framebufferStateService.UnbindFramebuffer( FramebufferTarget.Framebuffer );
		this._framebufferStateService.BlitToFrameBuffer( this.MultisampledFramebuffer, this.Framebuffer, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear );
	}

	internal void BlitDepthBuffer( BufferedSceneRenderer sceneRenderer ) {
		this._framebufferStateService.BlitToFrameBuffer( sceneRenderer.Framebuffer, this.MultisampledFramebuffer, ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest );
	}
}
