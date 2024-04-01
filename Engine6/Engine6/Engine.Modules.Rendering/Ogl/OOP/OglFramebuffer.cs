using Engine.Math.NewFolder;
using OpenGL;

namespace Engine.Modules.Rendering.Ogl.OOP;

public sealed class OglFramebuffer : DisposableIdentifiable {

	public readonly uint FramebufferId;
	public Vector2<int> Size { get; private set; }
	private int[]? _currentActiveAttachments;

	protected override string ExtraInformation => $"FBO{FramebufferId}";

	public bool RequiresGeneration { get; private set; } = false;

	public event Action<OglFramebuffer>? OnFramebufferGeneration;

	private readonly Dictionary<int, FramebufferAttachmentType> _attachments;
	public IReadOnlyDictionary<int, FramebufferAttachmentType> Attachments => _attachments.AsReadOnly();

	private readonly List<uint> _currentRenderbuffers;
	private readonly List<OglTexture> _currentTextures;

	public OglFramebuffer( uint framebufferId, Vector2<int> size ) {
		if (size.X <= 0 || size.Y <= 0)
			throw new OpenGlArgumentException( $"{FullName} must have positive non-zero size on both axis", nameof( size ) );

		FramebufferId = framebufferId;
		Size = size;
		_attachments = [];
		_currentRenderbuffers = [];
		_currentTextures = [];
		RequiresGeneration = true;
	}

	internal void Generate() {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		if (!RequiresGeneration) {
			this.LogWarning( $"Already generated." );
			return;
		}
		Wipe();
		OnFramebufferGeneration?.Invoke( this );
		RequiresGeneration = false;
		Validate();
	}

	public void Resize( Vector2<int> newSize ) {
		if (newSize.X <= 0 || newSize.Y <= 0)
			throw new OpenGlArgumentException( $"{FullName} must have positive non-zero size on both axis", nameof( newSize ) );
		if (newSize == Size)
			this.LogWarning( $"Already has size {newSize}." );
		Size = newSize;
		RequiresGeneration = true;
	}

	public OglTexture CreateTexture( TextureTarget target, InternalFormat internalFormat, params (TextureParameterName, int)[] parameters ) {
		OglTexture t = new( $"FBO{FramebufferId}", target, Size, internalFormat, parameters );
		_currentTextures.Add( t );
		return t;
	}

	public uint CreateRenderbuffer( InternalFormat format, int samples ) {
		uint buffer = Gl.CreateRenderbuffer();
		if (samples <= 0)
			Gl.NamedRenderbufferStorage( buffer, format, Size.X, Size.Y );
		else
			Gl.NamedRenderbufferStorageMultisample( buffer, samples, format, Size.X, Size.Y );
		_currentRenderbuffers.Add( buffer );
		return buffer;
	}

	private void Validate() {
		FramebufferStatus status = Gl.CheckNamedFramebufferStatus( FramebufferId, FramebufferTarget.Framebuffer );
		if (status != FramebufferStatus.FramebufferComplete)
			this.LogWarning( $"Validation failed: {status}" );
	}

	/// <summary>
	/// Attaches a texture to the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	/// <param name="tex">The texture id</param>
	/// <param name="level">The texture level, if the texture has several layers you must choose which layer to write to.</param>
	public void AttachTexture( FramebufferAttachment attachment, OglTexture texture, int level = 0 ) {
		if (texture is null)
			throw new ArgumentNullException( nameof( texture ) );
		if (texture.TextureID == 0)
			throw new OpenGlArgumentException( $"{FullName} says texture {texture} has not been created yet", nameof( texture ) );
		if (!_attachments.TryAdd( (int) attachment, FramebufferAttachmentType.Texture )) {
			this.LogWarning( $"Already an attachment at {attachment}." );
			return;
		}

		Gl.NamedFramebufferTexture( FramebufferId, attachment, texture.TextureID, level );
	}

	/// <summary>
	/// Detaches a texture from the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	public void DetachTexture( FramebufferAttachment attachment ) {
		if (!_attachments.TryGetValue( (int) attachment, out FramebufferAttachmentType type ) || type != FramebufferAttachmentType.Texture) {
			this.LogWarning( $"didn't have any attached texture at {attachment}." );
			return;
		}

		_attachments.Remove( (int) attachment );
		Gl.NamedFramebufferTexture( FramebufferId, attachment, 0, 0 );
	}


	/// <summary>
	/// Attaches a buffer to the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	/// <param name="buf">The buffer id</param>
	public void AttachRenderbuffer( FramebufferAttachment attachment, uint buffer ) {
		if (buffer == 0)
			throw new OpenGlArgumentException( $"Buffer id must be greater than zero", nameof( buffer ) );
		if (!_attachments.TryAdd( (int) attachment, FramebufferAttachmentType.Renderbuffer )) {
			this.LogWarning( $"Already an attachment at {attachment}." );
			return;
		}

		Gl.NamedFramebufferRenderbuffer( FramebufferId, attachment, RenderbufferTarget.Renderbuffer, buffer );
	}

	/// <summary>
	/// Detaches a buffer from the framebuffer at the specified attachment point.
	/// </summary>
	/// <param name="attachment">The attachment point</param>
	public void DetachRenderbuffer( FramebufferAttachment attachment ) {
		if (!_attachments.TryGetValue( (int) attachment, out FramebufferAttachmentType type ) || type != FramebufferAttachmentType.Renderbuffer) {
			this.LogWarning( $"Didn't have any attached renderbuffer at {attachment}." );
			return;
		}

		_attachments.Remove( (int) attachment );
		Gl.NamedFramebufferRenderbuffer( FramebufferId, attachment, RenderbufferTarget.Renderbuffer, 0 );
	}

	public void EnableCurrentColorAttachments() {
		int[] attachments = [ .. _attachments.Keys.Where( p => p != (int) FramebufferAttachment.DepthAttachment && p != (int) FramebufferAttachment.DepthStencilAttachment ) ];
		Array.Sort( attachments );
		if (_currentActiveAttachments is not null && _currentActiveAttachments.SequenceEqual( attachments )) {
			this.LogWarning( $"Already has the correct color attachments enabled." );
			return;
		}
		_currentActiveAttachments = attachments;
		Gl.NamedFramebufferDrawBuffers( FramebufferId, attachments.Length, attachments );
	}

	public void Clear( OpenGL.Buffer bufferType, int buffer, uint[] values ) => Gl.ClearNamedFramebuffer( FramebufferId, bufferType, buffer, values );

	public void Clear( OpenGL.Buffer bufferType, int buffer, int[] values ) => Gl.ClearNamedFramebuffer( FramebufferId, bufferType, buffer, values );

	public void Clear( OpenGL.Buffer bufferType, int buffer, float[] values ) => Gl.ClearNamedFramebuffer( FramebufferId, bufferType, buffer, values );

	public void ClearDepthStencil( OpenGL.Buffer bufferType, int buffer, float depth, int stencil ) => Gl.ClearNamedFramebuffer( FramebufferId, bufferType, buffer, depth, stencil );

	protected override bool InternalDispose() {
		Wipe();
		Gl.DeleteFramebuffers( FramebufferId );
		return true;
	}

	private void Wipe() {
		foreach (KeyValuePair<int, FramebufferAttachmentType> kvp in _attachments)
			switch (kvp.Value) {
				case FramebufferAttachmentType.Texture:
					DetachTexture( (FramebufferAttachment) kvp.Key );
					break;
				case FramebufferAttachmentType.Renderbuffer:
					DetachRenderbuffer( (FramebufferAttachment) kvp.Key );
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof( kvp.Value ), kvp.Value, null );
			}

		if (_currentRenderbuffers.Count > 0) {
			Gl.DeleteRenderbuffers( [ .. _currentRenderbuffers ] );
			_currentRenderbuffers.Clear();
		}

		for (int i = 0; i < _currentTextures.Count; i++)
			_currentTextures[ i ].Dispose();
		_currentTextures.Clear();
	}
}
