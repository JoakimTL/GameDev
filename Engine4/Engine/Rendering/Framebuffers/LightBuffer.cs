using OpenGL;

namespace Engine.Rendering.Framebuffers;

public class LightBuffer : Framebuffer {

	public Texture? DiffuseTexture { get; private set; }

	public LightBuffer( float scale = 1 ) : base( new WindowProportions( scale ) ) { }

	public override void Clear() => Clear( OpenGL.Buffer.Color, 0, new float[] { 0, 0, 0, 1 } );

	protected override void Generate() {
		this.DiffuseTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.Rgba16 );

		AttachTexture( FramebufferAttachment.ColorAttachment0, this.DiffuseTexture );

		EnableCurrentColorAttachments();
	}
}