using Engine.Logging;
using Engine.Module.Render.Ogl.OOP.Textures;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP;

public sealed class OglFramebuffer : DisposableIdentifiable {

	public readonly uint FramebufferId;
	public Vector2<int> Size { get; private set; }
	private int[]? _currentActiveAttachments;

	public bool RequiresGeneration { get; private set; } = false;

	public event Action<OglFramebuffer>? OnFramebufferGeneration;

	private readonly Dictionary<int, OglFramebufferAttachmentType> _attachments;
	public IReadOnlyDictionary<int, OglFramebufferAttachmentType> Attachments => this._attachments.AsReadOnly();

	private readonly List<uint> _currentRenderbuffers;
	private readonly List<OglTexture> _currentTextures;

	public OglFramebuffer( uint framebufferId, Vector2<int> size ) {
		if (size.X <= 0 || size.Y <= 0)
			throw new OpenGlArgumentException( $"{this.FullName} must have positive non-zero size on both axis", nameof( size ) );

		this.FramebufferId = framebufferId;
		this.Size = size;
		this._attachments = [];
		this._currentRenderbuffers = [];
		this._currentTextures = [];
		this.RequiresGeneration = true;
		this.Nickname = $"FBO{this.FramebufferId}";
	}

	internal void Generate() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		if (!this.RequiresGeneration) {
			this.LogWarning( $"Already generated." );
			return;
		}
		Wipe();
		OnFramebufferGeneration?.Invoke( this );
		this.RequiresGeneration = false;
		Validate();
	}

	public void Resize( Vector2<int> newSize ) {
		if (newSize.X <= 0 || newSize.Y <= 0)
			throw new OpenGlArgumentException( $"{this.FullName} must have positive non-zero size on both axis", nameof( newSize ) );
		if (newSize == this.Size)
			this.LogWarning( $"Already has size {newSize}." );
		this.Size = newSize;
		this.RequiresGeneration = true;
	}

	public OglTexture CreateTexture( TextureTarget target, InternalFormat internalFormat, params (TextureParameterName, int)[] parameters ) {
		OglTexture t = new( $"FBO{this.FramebufferId}", target, this.Size, internalFormat, parameters );
		this._currentTextures.Add( t );
		return t;
	}

	public uint CreateRenderbuffer( InternalFormat format, int samples ) {
		uint buffer = Gl.CreateRenderbuffer();
		if (samples <= 0)
			Gl.NamedRenderbufferStorage( buffer, format, this.Size.X, this.Size.Y );
		else
			Gl.NamedRenderbufferStorageMultisample( buffer, samples, format, this.Size.X, this.Size.Y );
		this._currentRenderbuffers.Add( buffer );
		return buffer;
	}

	private void Validate() {
		FramebufferStatus status = Gl.CheckNamedFramebufferStatus( this.FramebufferId, FramebufferTarget.Framebuffer );
		if (status != FramebufferStatus.FramebufferComplete)
			this.LogWarning( $"Validation failed: {status}" );
	}

	/// <summary>
	/// Attaches a texture to the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	/// <param name="textureId">The texture id</param>
	/// <param name="level">The texture level, if the texture has several layers you must choose which layer to write to.</param>
	public void AttachTexture( FramebufferAttachment attachment, uint textureId, int level = 0 ) {
		if (textureId == 0)
			throw new OpenGlArgumentException( $"Texture id must refer to a real texture. Dumbass.", nameof( textureId ) );
		if (!this._attachments.TryAdd( (int) attachment, OglFramebufferAttachmentType.Texture )) {
			this.LogWarning( $"Already an attachment at {attachment}." );
			return;
		}

		Gl.NamedFramebufferTexture( this.FramebufferId, attachment, textureId, level );
	}

	/// <summary>
	/// Detaches a texture from the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	public void DetachTexture( FramebufferAttachment attachment ) {
		if (!this._attachments.TryGetValue( (int) attachment, out OglFramebufferAttachmentType type ) || type != OglFramebufferAttachmentType.Texture) {
			this.LogWarning( $"didn't have any attached texture at {attachment}." );
			return;
		}

		this._attachments.Remove( (int) attachment );
		Gl.NamedFramebufferTexture( this.FramebufferId, attachment, 0, 0 );
	}


	/// <summary>
	/// Attaches a buffer to the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	/// <param name="buf">The buffer id</param>
	public void AttachRenderbuffer( FramebufferAttachment attachment, uint buffer ) {
		if (buffer == 0)
			throw new OpenGlArgumentException( $"Buffer id must be greater than zero", nameof( buffer ) );
		if (!this._attachments.TryAdd( (int) attachment, OglFramebufferAttachmentType.Renderbuffer )) {
			this.LogWarning( $"Already an attachment at {attachment}." );
			return;
		}

		Gl.NamedFramebufferRenderbuffer( this.FramebufferId, attachment, RenderbufferTarget.Renderbuffer, buffer );
	}

	/// <summary>
	/// Detaches a buffer from the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	public void DetachRenderbuffer( FramebufferAttachment attachment ) {
		if (!this._attachments.TryGetValue( (int) attachment, out OglFramebufferAttachmentType type ) || type != OglFramebufferAttachmentType.Renderbuffer) {
			this.LogWarning( $"Didn't have any attached renderbuffer at {attachment}." );
			return;
		}

		this._attachments.Remove( (int) attachment );
		Gl.NamedFramebufferRenderbuffer( this.FramebufferId, attachment, RenderbufferTarget.Renderbuffer, 0 );
	}

	public void EnableCurrentColorAttachments() {
		int[] attachments = [ .. this._attachments.Keys.Where( p => p != (int) FramebufferAttachment.DepthAttachment && p != (int) FramebufferAttachment.DepthStencilAttachment ) ];
		Array.Sort( attachments );
		if (this._currentActiveAttachments is not null && this._currentActiveAttachments.SequenceEqual( attachments )) {
			this.LogWarning( $"Already has the correct color attachments enabled." );
			return;
		}
		this._currentActiveAttachments = attachments;
		Gl.NamedFramebufferDrawBuffers( this.FramebufferId, attachments.Length, attachments );
	}

	public void Clear( OpenGL.Buffer bufferType, int buffer, uint[] values ) => Gl.ClearNamedFramebuffer( this.FramebufferId, bufferType, buffer, values );

	public void Clear( OpenGL.Buffer bufferType, int buffer, int[] values ) => Gl.ClearNamedFramebuffer( this.FramebufferId, bufferType, buffer, values );

	public void Clear( OpenGL.Buffer bufferType, int buffer, float[] values ) => Gl.ClearNamedFramebuffer( this.FramebufferId, bufferType, buffer, values );

	public void ClearDepthStencil( OpenGL.Buffer bufferType, int buffer, float depth, int stencil ) => Gl.ClearNamedFramebuffer( this.FramebufferId, bufferType, buffer, depth, stencil );

	protected override bool InternalDispose() {
		Wipe();
		Gl.DeleteFramebuffers( this.FramebufferId );
		return true;
	}

	private void Wipe() {
		foreach (KeyValuePair<int, OglFramebufferAttachmentType> kvp in this._attachments)
			switch (kvp.Value) {
				case OglFramebufferAttachmentType.Texture:
					DetachTexture( (FramebufferAttachment) kvp.Key );
					break;
				case OglFramebufferAttachmentType.Renderbuffer:
					DetachRenderbuffer( (FramebufferAttachment) kvp.Key );
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof( kvp.Value ), kvp.Value, null );
			}

		if (this._currentRenderbuffers.Count > 0) {
			Gl.DeleteRenderbuffers( [ .. this._currentRenderbuffers ] );
			this._currentRenderbuffers.Clear();
		}

		for (int i = 0; i < this._currentTextures.Count; i++)
			this._currentTextures[ i ].Dispose();
		this._currentTextures.Clear();
	}
}
