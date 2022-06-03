using Engine.Data.Datatypes;
using OpenGL;

namespace Engine.Rendering.Lighting.Directional;

public class CascadeDepthBuffer : Framebuffer {

	public Texture? DepthTexture { get; private set; }

	public Texture? TransparencyColorTexture { get; private set; }

	public Texture? TransparencyRevealTexture { get; private set; }

	public CascadeDepthBuffer( Vector2i size ) : base( new StaticProportions( size ) ) { }

	public override void Clear() {
		Clear( OpenGL.Buffer.Color, 0, new float[] { 0, 0, 0, 0 } );
		Clear( OpenGL.Buffer.Color, 1, new float[] { 1 } );
		Clear( OpenGL.Buffer.Depth, 0, new float[] { 1 } );
	}

	protected override void Generate() {
		this.DepthTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.DepthComponent32f );

		this.TransparencyColorTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.Rgba16f );

		this.TransparencyRevealTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.R16f );

		AttachTexture( FramebufferAttachment.ColorAttachment0, this.TransparencyColorTexture );
		AttachTexture( FramebufferAttachment.ColorAttachment1, this.TransparencyRevealTexture );
		AttachTexture( FramebufferAttachment.DepthAttachment, this.DepthTexture );

		EnableCurrentColorAttachments();
	}
}
