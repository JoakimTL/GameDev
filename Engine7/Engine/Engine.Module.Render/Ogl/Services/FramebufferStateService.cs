using Engine.Logging;
using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.OOP.Framebuffers;
using Engine.Module.Render.Ogl.OOP.Textures;
using ImageMagick;
using OpenGL;

namespace Engine.Module.Render.Ogl.Services;

public sealed class FramebufferStateService( ViewportStateService viewport ) : Identifiable {

	private readonly ViewportStateService _viewport = viewport;
	private uint _boundDrawBuffer = 0, _boundReadBuffer = 0;

	public OglFramebuffer CreateFramebuffer( Vector2<int> size ) => new( size );
	public AutoscalingFramebuffer CreateAutoscalingFramebuffer( IResizableSurface<int> resizableSurface, float scalingFactor ) => new( resizableSurface, scalingFactor );

	public void BindFramebuffer( FramebufferTarget target, OglFramebuffer buffer ) {
		if (buffer.Disposed) {
			this.LogWarning( $"Trying to bind disposed framebuffer {buffer}" );
			return;
		}
		if (buffer.RequiresGeneration)
			buffer.Generate();
		switch (target) {
			case FramebufferTarget.DrawFramebuffer:
				if (buffer.FramebufferId == this._boundDrawBuffer)
					this.LogWarning( $"Framebuffer {buffer} already bound to draw buffer" );
				this._boundDrawBuffer = buffer.FramebufferId;
				Gl.BindFramebuffer( target, buffer.FramebufferId );
				this._viewport.Set( 0, buffer.Size );
				break;
			case FramebufferTarget.ReadFramebuffer:
				if (buffer.FramebufferId == this._boundReadBuffer)
					this.LogWarning( $"Framebuffer {buffer} already bound to read buffer" );
				this._boundReadBuffer = buffer.FramebufferId;
				Gl.BindFramebuffer( target, buffer.FramebufferId );
				break;
			case FramebufferTarget.Framebuffer:
				if (buffer.FramebufferId == this._boundDrawBuffer && buffer.FramebufferId == this._boundReadBuffer)
					this.LogWarning( $"Framebuffer {buffer} already bound to both draw and read buffer" );
				this._boundDrawBuffer = this._boundReadBuffer = buffer.FramebufferId;
				Gl.BindFramebuffer( target, buffer.FramebufferId );
				this._viewport.Set( 0, buffer.Size );
				break;
			default:
				throw new ArgumentOutOfRangeException( nameof( target ), target, null );
		}
	}

	public void UnbindFramebuffer( FramebufferTarget target ) {
		switch (target) {
			case FramebufferTarget.DrawFramebuffer:
				if (this._boundDrawBuffer == 0)
					this.LogWarning( "Trying to unbind framebuffer as draw buffer, but no framebuffer bound to draw buffer" );
				this._boundDrawBuffer = 0;
				Gl.BindFramebuffer( target, 0 );
				break;
			case FramebufferTarget.ReadFramebuffer:
				if (this._boundReadBuffer == 0)
					this.LogWarning( "Trying to unbind framebuffer as read buffer, but no framebuffer bound to read buffer" );
				this._boundReadBuffer = 0;
				Gl.BindFramebuffer( target, 0 );
				break;
			case FramebufferTarget.Framebuffer:
				if (this._boundDrawBuffer == 0 && this._boundReadBuffer == 0)
					this.LogWarning( "Trying to unbind framebuffer as both draw and read buffer, but no framebuffer bound to either buffer" );
				this._boundDrawBuffer = this._boundReadBuffer = 0;
				Gl.BindFramebuffer( target, 0 );
				break;
			default:
				throw new ArgumentOutOfRangeException( nameof( target ), target, null );
		}
	}

	public void BlitFramebuffer( Vector2<int> srcPoint1, Vector2<int> srcPoint2, Vector2<int> dstPoint1, Vector2<int> dstPoint2, ClearBufferMask mask, BlitFramebufferFilter filter ) {
		if (srcPoint1.X < 0 || srcPoint1.Y < 0)
			throw new OpenGlArgumentException( "Point cannot be negative", nameof( srcPoint1 ) );
		if (dstPoint1.X < 0 || dstPoint1.Y < 0)
			throw new OpenGlArgumentException( "Point cannot be negative", nameof( dstPoint1 ) );
		if (srcPoint2.X < srcPoint1.X || srcPoint2.Y < srcPoint1.Y)
			throw new OpenGlArgumentException( "Second point must be greater than first point", nameof( srcPoint2 ) );
		if (dstPoint2.X < dstPoint1.X || dstPoint2.Y < dstPoint1.Y)
			throw new OpenGlArgumentException( "Second point must be greater than first point", nameof( dstPoint2 ) );
		Gl.BlitFramebuffer(
			srcPoint1.X, srcPoint1.Y, srcPoint2.X, srcPoint2.Y,
			dstPoint1.X, dstPoint1.Y, dstPoint2.X, dstPoint2.Y,
			mask, filter );
	}

	public void BlitToFrameBuffer( OglFramebuffer source, OglFramebuffer destination, ClearBufferMask mask, BlitFramebufferFilter filter ) {
		BindFramebuffer( FramebufferTarget.ReadFramebuffer, source );
		BindFramebuffer( FramebufferTarget.DrawFramebuffer, destination );
		BlitFramebuffer( (0, 0), source.Size, (0, 0), destination.Size, mask, filter );
		UnbindFramebuffer( FramebufferTarget.ReadFramebuffer );
		UnbindFramebuffer( FramebufferTarget.DrawFramebuffer );
	}

	public void BlitToScreen( OglFramebuffer source, OglWindow window, ClearBufferMask mask, BlitFramebufferFilter filter ) {
		BindFramebuffer( FramebufferTarget.ReadFramebuffer, source );
		UnbindFramebuffer( FramebufferTarget.DrawFramebuffer );
		BlitFramebuffer( (0, 0), source.Size, (0, 0), window.Size, mask, filter );
		//DrawBuffer( DrawBufferMode.Back );
		UnbindFramebuffer( FramebufferTarget.ReadFramebuffer );
	}

}


public sealed class TextureAssetService {
	private readonly Dictionary<string, OglTexture> _textures = [];

	public event Action? TextureAdded;
	public event Action? TextureRemoved;

	public OglTexture Get( string path ) {
		if (_textures.TryGetValue( path, out OglTexture? texture ))
			return texture;
		if (!File.Exists( path ))
			throw new FileNotFoundException( "Texture file not found", path );
		if (Path.GetExtension( path ) != ".png")
			throw new ArgumentException( "Texture file must be a PNG file", nameof( path ) );

		byte[]? pixelData;
		Vector2<int> imageDimensions;
		using (MagickImage image = new( path )) {
			image.Alpha( AlphaOption.Set );

			uint bitDepth = image.Depth;

			if (bitDepth != 8)
				throw new InvalidOperationException( $"Unsupported bit depth: {bitDepth}. Only 8-bit and 16-bit are supported." );

			image.Format = MagickFormat.Rgba;

			imageDimensions = ((int) image.Width, (int) image.Height);
			pixelData = image.GetPixels().ToByteArray( PixelMapping.RGBA );
		}

		if (pixelData is null)
			throw new InvalidOperationException( "Failed to load image data" );

		texture = new OglTexture( path, TextureTarget.Texture2d, imageDimensions, InternalFormat.Rgba8 );
		unsafe {
			fixed (byte* pixelDataPtr = pixelData) {
				texture.SetPixels( PixelFormat.Rgba, PixelType.UnsignedByte, (nint) pixelDataPtr );
			}
		}
		_textures.Add( path, texture );
		return texture;
	}
}

//TODO: REMOVE and replace with a better system for asset handling
public sealed class TextureAssetProvider( TextureAssetService textureAssetService ) : IRenderServiceProvider {
	private readonly TextureAssetService _textureAssetService = textureAssetService;

	public OglTexture Get( string name ) => _textureAssetService.Get( Path.Combine( "assets\\textures", $"{name}.png" ));
}

public sealed class TextureShaderStorageService {

}