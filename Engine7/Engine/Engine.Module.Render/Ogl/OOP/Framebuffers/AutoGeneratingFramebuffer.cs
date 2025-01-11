using Engine.Module.Render.Ogl.OOP.Textures;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Framebuffers;

public sealed class AutoGeneratingFramebuffer : DisposableIdentifiable {

	private readonly OglFramebuffer _framebuffer;
	private readonly Dictionary<FramebufferAttachment, IFramebufferAttachmentGenerator> _attachmentGenerators;
	private readonly List<FramebufferScaledTextureGenerator> _textureGenerators;
	private readonly List<FramebufferScaledRenderbufferGenerator> _renderBufferGenerators;

	public AutoGeneratingFramebuffer( OglFramebuffer framebuffer ) {
		this._framebuffer = framebuffer;
		this._attachmentGenerators = [];
		this._textureGenerators = [];
		this._renderBufferGenerators = [];
		this._framebuffer.OnFramebufferGeneration += GenerateFramebuffer;
	}

	public FramebufferScaledTextureGenerator AddTexture( FramebufferAttachment attachment, Func<Vector2<int>, OglTextureBase> textureGenerator, float scale = 1 ) {
		if (_attachmentGenerators.ContainsKey( attachment ))
			throw new OpenGlArgumentException( $"Attachment {attachment} already has a generator" );
		FramebufferScaledTextureGenerator generator = new( textureGenerator, scale );
		this._attachmentGenerators.Add( attachment, generator );
		this._textureGenerators.Add( generator );
		return generator;
	}

	public FramebufferScaledRenderbufferGenerator AddRenderBuffer( FramebufferAttachment attachment, InternalFormat format, uint samples ) {
		if (_attachmentGenerators.ContainsKey( attachment ))
			throw new OpenGlArgumentException( $"Attachment {attachment} already has a generator" );
		FramebufferScaledRenderbufferGenerator generator = new( format, samples );
		this._attachmentGenerators.Add( attachment, generator );
		this._renderBufferGenerators.Add( generator );
		return generator;
	}

	private void GenerateFramebuffer( OglFramebuffer framebuffer ) {
		foreach (FramebufferScaledTextureGenerator generator in this._textureGenerators)
			generator.Invalidate( _framebuffer );
		foreach (FramebufferScaledRenderbufferGenerator generator in this._renderBufferGenerators)
			generator.Invalidate( _framebuffer );
		foreach (KeyValuePair<FramebufferAttachment, IFramebufferAttachmentGenerator> kvp in this._attachmentGenerators)
			kvp.Value.Attach( _framebuffer, kvp.Key );
		framebuffer.EnableCurrentColorAttachments();
	}

	protected override bool InternalDispose() {
		foreach (FramebufferScaledTextureGenerator generator in this._textureGenerators)
			generator.Dispose();
		foreach (FramebufferScaledRenderbufferGenerator generator in this._renderBufferGenerators)
			generator.Dispose();
		return true;
	}
}
