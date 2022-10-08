using OpenGL;

namespace Engine.Rendering.Framebuffers;

public class ScreenFramebuffer : Framebuffer {

	public Texture? DiffuseTexture { get; private set; }

	public ScreenFramebuffer( Window window, float scale ) : base( new WindowProportions( window, scale ) ) { }

	public override void Clear() => Clear( OpenGL.Buffer.Color, 0, new float[] { 0, 0, 0, 0 } );

	protected override void Generate() {
		this.DiffuseTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.Rgba8,
				(TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear),
				(TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear) );

		AttachTexture( FramebufferAttachment.ColorAttachment0, this.DiffuseTexture );

		EnableCurrentColorAttachments();
	}
}