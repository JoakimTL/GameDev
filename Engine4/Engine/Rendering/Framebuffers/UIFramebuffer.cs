using OpenGL;

namespace Engine.Rendering.Framebuffers;

public class UIFramebuffer : Framebuffer {

	public Texture? DiffuseTexture { get; private set; }

	public UIFramebuffer( Window window, float scale ) : base( new WindowProportions( window, scale ) ) { }

	public override void Clear() => Clear( OpenGL.Buffer.Color, 0, new float[] { 0, 0, 0, 0 } );

	protected override void Generate() {
		uint diffuseBuffer = CreateRenderbuffer( InternalFormat.Rgba8, 4 );

		AttachBuffer( FramebufferAttachment.ColorAttachment0, diffuseBuffer );

		EnableCurrentColorAttachments();
	}
}
