using OpenGL;

namespace Engine.Rendering.Framebuffers;
public class GeometryBuffer : Framebuffer {

	public Texture? DiffuseTexture { get; private set; }

	public Texture? NormalTexture { get; private set; }

	public Texture? LightPropertiesTexture { get; private set; }

	public Texture? GlowTexture { get; private set; }

	public Texture? TransparencyColorTexture { get; private set; }

	public Texture? TransparencyRevealTexture { get; private set; }

	public Texture? DepthTexture { get; private set; }

	public const uint TransparencyColorTextureTarget = 4;
	public const uint TransparencyRevealTextureTarget = 5;

	public GeometryBuffer( float scale = 1 ) : base( new WindowProportions( scale ) ) {	}

	protected override void Generate() {
		this.DiffuseTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.Rgba16 );

		this.NormalTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.Rgb16,
			(TextureParameterName.TextureMinFilter, (int) TextureMagFilter.Nearest),
			(TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest) );

		this.LightPropertiesTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.Rgb16 );

		this.GlowTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.Rgba16 );

		this.TransparencyColorTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.Rgba16f );

		this.TransparencyRevealTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.R16f );

		this.DepthTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.DepthComponent32f );

		AttachTexture( FramebufferAttachment.ColorAttachment0, this.DiffuseTexture );
		AttachTexture( FramebufferAttachment.ColorAttachment1, this.NormalTexture );
		AttachTexture( FramebufferAttachment.ColorAttachment2, this.LightPropertiesTexture );
		AttachTexture( FramebufferAttachment.ColorAttachment3, this.GlowTexture );
		AttachTexture( FramebufferAttachment.ColorAttachment4, this.TransparencyColorTexture );
		AttachTexture( FramebufferAttachment.ColorAttachment5, this.TransparencyRevealTexture );
		AttachTexture( FramebufferAttachment.DepthAttachment, this.DepthTexture );

		EnableCurrentColorAttachments();
	}

	public override void Clear() {
		Clear( OpenGL.Buffer.Color, 0, new float[] { 0, 0, 0, 1 } );
		Clear( OpenGL.Buffer.Color, 1, new float[] { 0, 0, 0 } );
		Clear( OpenGL.Buffer.Color, 2, new float[] { 0, 0, 0 } );
		Clear( OpenGL.Buffer.Color, 3, new float[] { 0, 0, 0, 0 } );
		Clear( OpenGL.Buffer.Color, 4, new float[] { 0, 0, 0, 0 } );
		Clear( OpenGL.Buffer.Color, 5, new float[] { 1 } );
		Clear( OpenGL.Buffer.Depth, 0, new float[] { 1 } );
	}
}
