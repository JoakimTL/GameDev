using Engine.Data.Datatypes;
using Engine.Rendering.Utilities;
using OpenGL;

namespace Engine.Rendering;
public abstract class Framebuffer : DisposableIdentifiable {
	private readonly uint _framebufferId;
	private readonly Dictionary<int, bool> _activeAttachments;
	private readonly List<uint> _renderBuffers;
	private readonly List<Texture> _textures;
	public Proportions Resolution { get; private set; }
	public Vector2i Size => this.Resolution?.Size ?? 1;

	public Framebuffer( Proportions proportions ) {
		if ( proportions is null )
			throw new ArgumentNullException( nameof( proportions ) );
		this._framebufferId = Gl.CreateFramebuffer();
		this._activeAttachments = new Dictionary<int, bool>();
		this._renderBuffers = new List<uint>();
		this._textures = new List<Texture>();
		this.Resolution = proportions;
		this.Resolution.Resized += ProportionsChanged;
		ProportionsChanged();
	}

	public Texture CreateTexture( TextureTarget target, InternalFormat internalFormat, params (TextureParameterName, int)[] parameters ) {
		Texture t = new( $"FBO#{this.Name}:{internalFormat}", target, this.Size, internalFormat, parameters );
		this.LogLine( $"Created new texture [{t}]!", Log.Level.LOW, ConsoleColor.Cyan );
		return t;
	}

	private void ProportionsChanged() {
		if ( this.Disposed )
			return;
		Wipe();
		Generate();
		Validate();
	}

	public void SetProportions( Proportions proportions ) {
		if ( this.Resolution is not null ) {
			this.Resolution.Resized -= ProportionsChanged;
			this.Resolution.Dispose();
		}
		if ( proportions is null )
			return;
		this.Resolution = proportions;
		this.Resolution.Resized += ProportionsChanged;
		ProportionsChanged();
	}

	public abstract void Clear();
	protected abstract void Generate();

	private void Wipe() {
		foreach ( KeyValuePair<int, bool> kvp in this._activeAttachments ) {
			if ( kvp.Value ) {
				DetachBuffer( (FramebufferAttachment) kvp.Key );
			} else {
				DetachTexture( (FramebufferAttachment) kvp.Key );
			}
		}

		if ( this._renderBuffers.Count > 0 ) {
			Gl.DeleteRenderbuffers( this._renderBuffers.ToArray() );
			this._renderBuffers.Clear();
		}

		if ( this._textures.Count > 0 ) {
			for ( int i = 0; i < this._textures.Count; i++ )
				this._textures[ i ].Dispose();
			this._textures.Clear();
		}
	}

	protected void Validate() {
		FramebufferStatus status = Gl.CheckNamedFramebufferStatus( this._framebufferId, FramebufferTarget.Framebuffer );
		if ( status != FramebufferStatus.FramebufferComplete ) {
			this.LogWarning( $"Framebuffer validation failed: {status}" );
			return;
		}
		this.LogLine( "Framebuffer validated!", Log.Level.HIGH, color: ConsoleColor.Green );
	}

	#region Attachment
	/// <summary>
	/// Attaches a texture to the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	/// <param name="tex">The texture id</param>
	/// <param name="level">The texture level, if the texture has several layers you must choose which layer to write to.</param>
	public void AttachTexture( FramebufferAttachment attachment, Texture tex, int level = 0 ) {
		if ( tex is null )
			return;
		if ( tex.TextureID == 0 )
			return;
		Gl.NamedFramebufferTexture( this._framebufferId, attachment, tex.TextureID, level );
		this._activeAttachments[ (int) attachment ] = false;
		this.LogLine( $"Attached texture [{tex}] to [{attachment}]!", Log.Level.NORMAL );
	}

	/// <summary>
	/// Detaches a texture from the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	public void DetachTexture( FramebufferAttachment attachment ) {
		Gl.NamedFramebufferTexture( this._framebufferId, attachment, 0, 0 );
		this._activeAttachments.Remove( (int) attachment );
	}

	/// <summary>
	/// Attaches a buffer to the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	/// <param name="buf">The buffer id</param>
	public void AttachBuffer( FramebufferAttachment attachment, uint buf ) {
		if ( buf == 0 )
			return;
		Gl.NamedFramebufferRenderbuffer( this._framebufferId, attachment, RenderbufferTarget.Renderbuffer, buf );
		this._activeAttachments[ (int) attachment ] = true;
	}

	/// <summary>
	/// Detaches a buffer from the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	public void DetachBuffer( FramebufferAttachment attachment ) {
		Gl.NamedFramebufferRenderbuffer( this._framebufferId, attachment, RenderbufferTarget.Renderbuffer, 0 );
		this._activeAttachments.Remove( (int) attachment );
	}

	public void EnableCurrentColorAttachments() {
		List<int> attachments = new();
		int i = 0;
		foreach ( int attachment in this._activeAttachments.Keys )
			if ( attachment != (int) FramebufferAttachment.DepthAttachment && attachment != (int) FramebufferAttachment.DepthStencilAttachment ) {
				attachments.Add( attachment );
				i++;
			}
		attachments.Sort();
		Gl.NamedFramebufferDrawBuffers( this._framebufferId, i, attachments.ToArray() );
	}

	protected void Clear( OpenGL.Buffer bufferType, int buffer, uint[] values ) => Gl.ClearNamedFramebuffer( this._framebufferId, bufferType, buffer, values );

	protected void Clear( OpenGL.Buffer bufferType, int buffer, int[] values ) => Gl.ClearNamedFramebuffer( this._framebufferId, bufferType, buffer, values );

	protected void Clear( OpenGL.Buffer bufferType, int buffer, float[] values ) => Gl.ClearNamedFramebuffer( this._framebufferId, bufferType, buffer, values );

	protected void ClearDepthStencil( OpenGL.Buffer bufferType, int buffer, float depth, int stencil ) => Gl.ClearNamedFramebuffer( this._framebufferId, bufferType, buffer, depth, stencil );
	#endregion

	public void Bind() {
		Gl.BindFramebuffer( FramebufferTarget.DrawFramebuffer, this._framebufferId );
		Viewport.Set( 0, this.Size );
	}

	internal static void Unbind( FramebufferTarget target ) => Gl.BindFramebuffer( target, 0 );

	protected override bool OnDispose() {
		Wipe();
		Gl.DeleteFramebuffers( this._framebufferId );
		return true;
	}

	public abstract class Proportions : DisposableIdentifiable {
		public abstract Vector2i Size { get; }
		public abstract event Action Resized;
	}

	public sealed class ProportionalProportions : Proportions {

		public override Vector2i Size => Vector2i.Max( Vector2i.Ceiling( this._proportions.Size * this._scale ), 1 );
		public override event Action? Resized;

		private readonly Proportions _proportions;
		private readonly float _scale;

		public ProportionalProportions( Proportions proportions, float scale ) {
			this._proportions = proportions;
			this._scale = scale;
			this._proportions.Resized += Resized;
		}

		protected override bool OnDispose() {
			this._proportions.Resized -= Resized;
			return true;
		}
	}

	public sealed class WindowProportions : Proportions {

		public override Vector2i Size => Vector2i.Max( Vector2i.Ceiling( Resources.Render.Window.Size * this._scale ), 1 );
		public override event Action? Resized;

		private readonly float _scale;

		public WindowProportions( float scale ) {
			this._scale = scale;
			Resources.Render.Window.WindowEvents.Resized += WindowResized;
		}

		private void WindowResized( int width, int height ) => Resized?.Invoke();

		protected override bool OnDispose() {
			Resources.Render.Window.WindowEvents.Resized -= WindowResized;
			return true;
		}
	}

	public sealed class StaticProportions : Proportions {

		private Vector2i _size;
		public override Vector2i Size => this._size;
		public override event Action? Resized;

		public StaticProportions( Vector2i initialSize ) {
			if ( Vector2i.NegativeOrZero( initialSize ) || initialSize.X == 0 || initialSize.Y == 0 ) {
				this.LogWarning( "Size must be greater than zero." );
				throw new ArgumentException( nameof( initialSize ) );
			}
			this._size = initialSize;
		}

		public void Resize( Vector2i newSize ) {
			if ( Vector2i.NegativeOrZero( newSize ) || newSize.X == 0 || newSize.Y == 0 ) {
				this.LogWarning( "Size must be greater than zero." );
				return;
			}
			this._size = newSize;
			Resized?.Invoke();
		}

		protected override bool OnDispose() => true;
	}
}
