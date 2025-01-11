using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Framebuffers;

public interface IFramebufferAttachmentGenerator {
	void Invalidate( ISurface<int> surface );
	void Attach( OglFramebuffer framebuffer, FramebufferAttachment attachment );
}
