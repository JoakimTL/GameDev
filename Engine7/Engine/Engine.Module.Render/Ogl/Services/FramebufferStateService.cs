﻿using Engine.Logging;
using Engine.Module.Render.Ogl.OOP;
using OpenGL;

namespace Engine.Module.Render.Ogl.Services;

public sealed class FramebufferStateService( ViewportStateService viewport ) : Identifiable {

	private readonly ViewportStateService _viewport = viewport;
	private uint _boundDrawBuffer = 0, _boundReadBuffer = 0;

	public OglFramebuffer CreateFramebuffer( Vector2<int> size ) {
		uint framebufferId = Gl.CreateFramebuffer();
		return new( framebufferId, size );
	}

	public OglFramebuffer CreateSurfaceDependentFramebuffer( IResizableSurface<int, float> resizableSurface, Vector2<float> scalingFactor ) {
		//uint framebufferId = Gl.CreateFramebuffer();
		//return new( framebufferId, size );
		throw new NotImplementedException();
	}

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
