using Engine.Data.Datatypes;
using OpenGL;

namespace Engine.Rendering.Lighting.Directional;

public class CascadeDepthBlurBuffer : Framebuffer {

	public Texture? RedTexture { get; private set; }

	public CascadeDepthBlurBuffer( Vector2i size ) : base( new StaticProportions( size ) ) { }

	public override void Clear() => Clear( OpenGL.Buffer.Color, 0, new float[] { 1 } );

	protected override void Generate() {
		this.RedTexture = CreateTexture( TextureTarget.Texture2d, InternalFormat.R32f );

		AttachTexture( FramebufferAttachment.ColorAttachment0, this.RedTexture );

		EnableCurrentColorAttachments();
	}
}