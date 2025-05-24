using Engine;
using Engine.Module.Render.Ogl.OOP.Framebuffers;
using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using OpenGL;

namespace Civlike.Client.Render;

public sealed class BufferedSceneRenderer : Identifiable {
	public readonly Scene Scene;
	private readonly FramebufferStateService _framebufferStateService;

	public readonly AutoscalingFramebuffer Framebuffer;
	public readonly AutoGeneratingFramebuffer FramebufferGenerator;

	public readonly FramebufferScaledTextureGenerator DisplayTexture;

	public BufferedSceneRenderer( Scene scene, WindowService windowService, FramebufferStateService framebufferStateService ) {
		this.Scene = scene;
		this._framebufferStateService = framebufferStateService;
		this.Framebuffer = framebufferStateService.CreateAutoscalingFramebuffer( windowService.Window, 1 );
		this.FramebufferGenerator = new( this.Framebuffer );
		this.DisplayTexture = this.FramebufferGenerator.AddTexture( FramebufferAttachment.ColorAttachment0, dimensions => new OglTexture( $"{this.FullName}Color", TextureTarget.Texture2d, dimensions, InternalFormat.Rgba8, (TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear), (TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear) ) );
		this.FramebufferGenerator.AddRenderBuffer( FramebufferAttachment.DepthAttachment, InternalFormat.DepthComponent, 0 );
	}

	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType primitiveType ) {
		this._framebufferStateService.BindFramebuffer( FramebufferTarget.Framebuffer, this.Framebuffer );
		this.Framebuffer.Clear( OpenGL.Buffer.Color, 0, [ 0 ] );
		this.Framebuffer.Clear( OpenGL.Buffer.Depth, 0, [ 1f ] );
		this.Scene.Render( shaderIndex, dataBlocks, blendActivationFunction, primitiveType );
		this._framebufferStateService.UnbindFramebuffer( FramebufferTarget.Framebuffer );
	}
}