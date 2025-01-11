using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Framebuffers;
public sealed class FramebufferScaledRenderbufferGenerator( InternalFormat format, uint samples ) : DisposableIdentifiable, IFramebufferAttachmentGenerator {

	public uint RenderBufferId { get; private set; }
	public InternalFormat Format { get; } = format;
	public uint Samples { get; } = samples;

	public void Invalidate( ISurface<int> surface ) {
		if (RenderBufferId > 0)
			Gl.DeleteRenderbuffers( RenderBufferId );
		RenderBufferId = Gl.CreateRenderbuffer();
		if (Samples == 0) {
			Gl.NamedRenderbufferStorage( RenderBufferId, Format, surface.Size.X, surface.Size.Y );
			return;
		}
		Gl.NamedRenderbufferStorageMultisample( RenderBufferId, (int) Samples, Format, surface.Size.X, surface.Size.Y );
	}
	public void Attach( OglFramebuffer framebuffer, FramebufferAttachment attachment ) {
		if (RenderBufferId == 0)
			throw new InvalidOperationException( "Renderbuffer not generated yet" );
		framebuffer.AttachRenderbuffer( attachment, RenderBufferId );
	}

	protected override bool InternalDispose() {
		if (RenderBufferId > 0)
			Gl.DeleteRenderbuffers( RenderBufferId );
		return true;
	}
}
