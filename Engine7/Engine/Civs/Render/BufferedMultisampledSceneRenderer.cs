using Engine;
using Engine.Module.Render.Ogl.OOP.Framebuffers;
using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using OpenGL;

namespace Civs.Render;

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

		MultisampledFramebuffer = framebufferStateService.CreateAutoscalingFramebuffer( windowService.Window, 1 );
		MultisampledFramebufferGenerator = new( MultisampledFramebuffer );
		MultisampledFramebufferGenerator.AddTexture( FramebufferAttachment.ColorAttachment0, dimensions => new OglMultisampledTexture( $"{this.FullName}MultisampledColor", TextureTarget.Texture2dMultisample, dimensions, samples, InternalFormat.Rgba8, true ) );

		Framebuffer = framebufferStateService.CreateAutoscalingFramebuffer( windowService.Window, 1 );
		FramebufferGenerator = new( Framebuffer );
		DisplayTexture = FramebufferGenerator.AddTexture( FramebufferAttachment.ColorAttachment0, dimensions => new OglTexture( $"{this.FullName}Color", TextureTarget.Texture2d, dimensions, InternalFormat.Rgba8, (TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear), (TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear) ) );
	}

	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType primitiveType ) {
		_framebufferStateService.BindFramebuffer( FramebufferTarget.Framebuffer, MultisampledFramebuffer );
		MultisampledFramebuffer.Clear( OpenGL.Buffer.Color, 0, [ 0 ] );
		Scene.Render( shaderIndex, dataBlocks, blendActivationFunction, primitiveType );
		_framebufferStateService.UnbindFramebuffer( FramebufferTarget.Framebuffer );
		_framebufferStateService.BlitToFrameBuffer( MultisampledFramebuffer, Framebuffer, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear );
	}
}
