using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Graphics.Disposables;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects {
	public abstract class Framebuffer : Cacheable {
		//Implement stencil buffering

		public uint ID { get; private set; }
		public uint DepthStencilBuffer { get; private set; }
		private readonly List<uint> textures;
		private int[] drawBuffers;

		private BufferScaler scaling;
		public Vector2i Size { get; private set; }

		public event Action Remade;

		public Framebuffer( string name ) : base( name ) {
			drawBuffers = new int[ 0 ];
			textures = new List<uint>();
			Size = 0;
		}

		public Framebuffer( string name, Vector2i size ) : base( name ) {
			drawBuffers = new int[ 0 ];
			textures = new List<uint>();
			Size = size;
			InitFramebuffer();
		}

		public Framebuffer( string name, Vector2i size, float scale ) : base( name ) {
			drawBuffers = new int[ 0 ];
			textures = new List<uint>();
			Size = ( size * scale ).IntFloored.Max( 1 );
			InitFramebuffer();
		}

		~Framebuffer() {
			new OGLFramebufferDisposer( Name, ID, DepthStencilBuffer, textures );
		}

		#region Generation
		private void InitFramebuffer() {
			if( ID != 0 )
				Gl.DeleteFramebuffers( ID );
			ID = 0;
			DeleteDepthStencilBuffer();
			DeleteTextures();
			if( Size.X * Size.Y <= 0 ) {
				Logging.Warning( $"[{Name}] Size is invalid!" );
				return;
			}
			InitBuffer();
			Bind( FramebufferTarget.Framebuffer );
			CreateFBO();
			Validate();
			Remade?.Invoke();
		}

		private void InitBuffer() {
			ID = Gl.GenFramebuffer();
			Logging.Routine( $"[{Name}]: Created buffer [{ID}]!", ConsoleColor.Cyan );
		}

		protected abstract void CreateFBO();

		protected uint CreateAndBindTexture( TextureTarget target = TextureTarget.Texture2d ) {
			uint tex = Gl.GenTexture();
			textures.Add( tex );
			Gl.BindTexture( target, tex );
			Logging.Routine( $"[{Name}]: Created texture [{tex}]!", ConsoleColor.Cyan );
			return tex;
		}

		protected uint CreateAndBindTexture( TextureTarget target, InternalFormat iFormat, PixelFormat pFormat, PixelType pType ) {
			uint tex = Gl.GenTexture();
			textures.Add( tex );
			Gl.BindTexture( target, tex );
			Gl.TexImage2D( target, 0, iFormat, Size.X, Size.Y, 0, pFormat, pType, (IntPtr) 0 );
			Logging.Routine( $"[{Name}]: Created texture [{tex}]!", ConsoleColor.Cyan );
			return tex;
		}

		protected void SetFilter( TextureTarget target = TextureTarget.Texture2d, int minMagFiler = (int) TextureMagFilter.Linear, int wrapMode = (int) TextureWrapMode.ClampToEdge ) {
			Gl.TexParameter( target, TextureParameterName.TextureMagFilter, minMagFiler );
			Gl.TexParameter( target, TextureParameterName.TextureMinFilter, minMagFiler );
			Gl.TexParameter( target, TextureParameterName.TextureWrapS, wrapMode );
			Gl.TexParameter( target, TextureParameterName.TextureWrapT, wrapMode );
		}

		protected void GenerateDepthStencilbuffer() {
			DeleteDepthStencilBuffer();
			DepthStencilBuffer = Gl.GenRenderbuffer();
		}

		protected void SetDrawbuffers( params int[] buffers ) {
			drawBuffers = buffers;
			Gl.DrawBuffers( buffers );
		}

		protected void AttachTexture( FramebufferAttachment attachment, uint texId ) {
			Gl.FramebufferTexture( FramebufferTarget.Framebuffer, attachment, texId, 0 );
		}

		private void Validate() {
			FramebufferStatus status = Gl.CheckFramebufferStatus( FramebufferTarget.Framebuffer );
			if( status != FramebufferStatus.FramebufferComplete ) {
				Logging.Warning( $"[{Name}]: [{status}]!" );
				return;
			}
			Logging.Routine( $"[{Name}]: Validated!", ConsoleColor.Green );
		}
		#endregion

		#region Binding
		public void BindDraw( TextureTarget target, uint texture, FramebufferAttachment drawAttachment ) {
			Gl.BindFramebuffer( FramebufferTarget.DrawFramebuffer, ID );
			Gl.FramebufferTexture2D( FramebufferTarget.DrawFramebuffer, drawAttachment, target, texture, 0 );
			Gl.DrawBuffers( (int) drawAttachment );
			Gl.Viewport( 0, 0, Size.X, Size.Y );
		}

		public void BindRead( TextureTarget target, uint texture, ReadBufferMode readAttachment ) {
			Gl.BindFramebuffer( FramebufferTarget.ReadFramebuffer, ID );
			Gl.FramebufferTexture2D( FramebufferTarget.ReadFramebuffer, FramebufferAttachment.ColorAttachment0, target, texture, 0 );
			Gl.ReadBuffer( readAttachment );
		}

		public void Bind( FramebufferTarget target ) {
			Gl.BindFramebuffer( target, ID );
			if( target != FramebufferTarget.ReadFramebuffer )
				Gl.Viewport( 0, 0, Size.X, Size.Y );
		}

		public void Clear( OpenGL.Buffer buffer, FramebufferAttachment bufferAttachment, uint[] values ) {
			Gl.ClearBuffer( buffer, (int) bufferAttachment, values );
		}

		public void Clear( OpenGL.Buffer buffer, FramebufferAttachment bufferAttachment, int[] values ) {
			Gl.ClearBuffer( buffer, (int) bufferAttachment, values );
		}

		public void Clear( OpenGL.Buffer buffer, FramebufferAttachment bufferAttachment, float[] values ) {
			Gl.ClearBuffer( buffer, (int) bufferAttachment, values );
		}

		public void Clear( OpenGL.Buffer buffer, FramebufferAttachment bufferAttachment, float depth, int stencil ) {
			Gl.ClearBuffer( buffer, (int) bufferAttachment, depth, stencil );
		}

		public static void Unbind( FramebufferTarget target ) {
			Gl.BindFramebuffer( target, 0 );
		}
		#endregion

		#region Scaling
		public void BindToWindow( GLWindow window, float scale = 1 ) {
			if( scaling is null )
				scaling = new WindowBind( this, window, scale );
		}

		public void BindToBuffer( Framebuffer buffer, float scale = 1 ) {
			if( scaling is null )
				scaling = new BufferBind( this, buffer, scale );
		}

		public void UnbindScaler() {
			if( scaling is null )
				return;
			scaling.Unbind();
			scaling = null;
		}

		public void SetSize( Vector2i size ) {
			Vector2i nSize = size;
			if( nSize != Size ) {
				Size = nSize;
				InitFramebuffer();
			}
		}
		#endregion

		#region Disposal
		private void DeleteTextures() {
			foreach( uint t in textures )
				Gl.DeleteTextures( t );
			textures.Clear();
		}

		private void DeleteDepthStencilBuffer() {
			if( DepthStencilBuffer != 0 )
				Gl.DeleteRenderbuffers( DepthStencilBuffer );
			DepthStencilBuffer = 0;
		}

		public override void Dispose() {
			if( ID != 0 )
				Gl.DeleteFramebuffers( ID );
			ID = 0;
			DeleteDepthStencilBuffer();
			DeleteTextures();
		}
		#endregion

		#region Blitting
		public static void BlitDepth( Framebuffer src, Framebuffer dst, bool unbind ) {
			Gl.BindFramebuffer( FramebufferTarget.ReadFramebuffer, src.ID );
			Gl.BindFramebuffer( FramebufferTarget.DrawFramebuffer, dst.ID );
			Gl.BlitFramebuffer( 0, 0, src.Size.X, src.Size.Y, 0, 0, dst.Size.X, dst.Size.Y, ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest );
			if( unbind )
				Gl.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );
		}

		public static void BlitStencil( Framebuffer src, Framebuffer dst ) {
			Gl.BindFramebuffer( FramebufferTarget.ReadFramebuffer, src.ID );
			Gl.BindFramebuffer( FramebufferTarget.DrawFramebuffer, dst.ID );
			Gl.BlitFramebuffer( 0, 0, src.Size.X, src.Size.Y, 0, 0, dst.Size.X, dst.Size.Y, ClearBufferMask.StencilBufferBit, BlitFramebufferFilter.Nearest );
			Gl.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );
		}

		public static void BlitColor( Framebuffer src, Framebuffer dst, ReadBufferMode readBuffer ) {
			Gl.BindFramebuffer( FramebufferTarget.ReadFramebuffer, src.ID );
			Gl.ReadBuffer( readBuffer );
			Gl.BindFramebuffer( FramebufferTarget.DrawFramebuffer, dst.ID );
			Gl.DrawBuffers( (int) readBuffer );
			Gl.BlitFramebuffer( 0, 0, src.Size.X, src.Size.Y, 0, 0, dst.Size.X, dst.Size.Y, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest );
			Gl.DrawBuffers( dst.drawBuffers );
			Gl.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );
		}
		#endregion

		private abstract class BufferScaler {
			private readonly Framebuffer thisBuffer;
			private float scale;
			public float Scale { get => scale; set => SetScaling( value ); }

			public BufferScaler( Framebuffer thisBuffer, float scale ) {
				this.thisBuffer = thisBuffer;
				this.scale = scale;
			}

			protected void ResizeBuffer( Vector2i size ) {
				thisBuffer.SetSize( ( size * scale ).IntFloored.Max( 1 ) );
			}

			public abstract void Unbind();

			private void SetScaling( float scaling ) {
				if( scaling <= 0 )
					return;
				float prev = scale;
				scale = scaling;
				float ratio = scale / prev;
				thisBuffer.SetSize( ( thisBuffer.Size * ratio ).IntFloored.Max( 1 ) );
			}
		}

		private class BufferBind : BufferScaler {
			protected readonly Framebuffer boundBuffer;

			public BufferBind( Framebuffer thisBuffer, Framebuffer boundBuffer, float scale ) : base( thisBuffer, scale ) {
				this.boundBuffer = boundBuffer;
				boundBuffer.Remade += BoundBufferRemade;
				ResizeBuffer( boundBuffer.Size );
			}

			private void BoundBufferRemade() {
				ResizeBuffer( boundBuffer.Size );
			}

			public override void Unbind() {
				boundBuffer.Remade -= BoundBufferRemade;
			}
		}

		private class WindowBind : BufferScaler, IWindowEventListener {

			private GLWindow window;

			public WindowBind( Framebuffer buffer, GLWindow window, float scale ) : base( buffer, scale ) {
				this.window = window;
				window.EventHandler.Window.Add( this );
				ResizeBuffer( window.Size );
			}

			public void WindowFocusHandler( IntPtr window, bool focused ) { }

			public void WindowResizeHandler( IntPtr window, int width, int height ) {
				ResizeBuffer( (width, height) );
			}

			public override void Unbind() {
				window.EventHandler.Window.Remove( this );
				window = null;
			}
		}
	}
}
