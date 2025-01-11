using Engine.Module.Render.Ogl.OOP.Textures;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Framebuffers;
public sealed class FramebufferScaledTextureGenerator( Func<Vector2<int>, OglTextureBase> textureGenerator, float scale = 1 ) : DisposableIdentifiable, IFramebufferAttachmentGenerator {
	private readonly Func<Vector2<int>, OglTextureBase> _textureGenerator = textureGenerator;
	private readonly float _scale = scale;
	private OglTextureBase? _texture;
	private TextureReference? _reference;
	public TextureReference TextureReference => GetReference();

	public void Invalidate( ISurface<int> surface ) {
		_reference = Generate( surface.Size );
	}

	private TextureReference Generate( Vector2<int> surfaceDimensions ) {
		_texture?.Dispose();
		_texture = _textureGenerator.Invoke( (surfaceDimensions.CastSaturating<int, float>() * _scale)
			.Round<Vector2<float>, float>( 0, MidpointRounding.AwayFromZero )
			.CastSaturating<float, int>()
			.Max( 1 ) );
		return _texture.GetTextureReference();
	}

	public void Attach( OglFramebuffer framebuffer, FramebufferAttachment attachment ) {
		if (_texture == null)
			throw new InvalidOperationException( "Texture not generated yet" );
		framebuffer.AttachTexture( attachment, _texture.TextureID, 0 );
	}

	private TextureReference GetReference()
		=> _reference ?? throw new InvalidOperationException( "Texture not generated yet" );

	protected override bool InternalDispose() {
		_texture?.Dispose();
		return true;
	}
}
