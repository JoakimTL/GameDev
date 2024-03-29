﻿using Engine.Data;
using Engine.OpenGL;
using OpenGL;

namespace Engine.OpenGL.OOP;

public abstract class OglTextureBase<T> : OglObjectBase where T : struct {
	public readonly uint TextureID;
	public readonly TextureTarget Target;
	public readonly T Metadata;
	protected readonly ulong _handle;
	protected readonly IReadOnlyList<Vector2i> _levels;
	protected readonly ContextWarningLog _warningLog;
	private uint _referenceCount;

	/// <summary>
	/// Level 0 is guaranteed to exist, with a size of 1x1 or greater.
	/// </summary>
	public Vector2i Level0 => _levels[ 0 ];
	public bool Resident { get; private set; }

	protected override string DisplayName => $"TEX#{(Nickname.Length > 0 ? $"{Nickname}:" : "")}{TextureID}";

	public OglTextureBase( ContextWarningLog warningLog, TextureTarget target, Vector2i level0, T metadata, params (TextureParameterName, int)[] parameters ) {
		if (level0.X <= 0 || level0.Y <= 0)
			throw new OpenGlArgumentException( "Texture size must be greater than zero on both axis", nameof( level0 ) );
		this._warningLog = warningLog;
		this.Target = target;
		this._levels = GetLevels( level0, metadata );
		Resident = false;

		TextureID = Gl.CreateTexture( Target );
		GenerateTexture( metadata );
		for (int i = 0; i < parameters.Length; i++)
			Gl.TextureParameter( TextureID, parameters[ i ].Item1, parameters[ i ].Item2 );
		_handle = Gl.GetTextureHandleARB( TextureID );
	}

	protected abstract void GenerateTexture( T metadata );

	protected abstract IReadOnlyList<Vector2i> GetLevels( Vector2i level0, T metadata );

	internal ulong Handle => _handle;

	public Vector2i GetLevel( uint level ) {
		if (level > _levels.Count) {
			_warningLog.LogWarning( $"{DisplayName} has no level {level}" );
			return (0, 0);
		}
		return _levels[ (int) level ];
	}

	public TextureReference GetTextureReference() {
		if (Disposed)
			throw new ObjectDisposedException( DisplayName );
		TextureReference reference = new( this );
		reference.OnDestruction += OnReferenceDestruction;
		if (_referenceCount == 0)
			MakeResident();
		_referenceCount++;
		return reference;
	}

	private void OnReferenceDestruction() {
		_referenceCount--;
		if (_referenceCount == 0)
			MakeNonResident();
	}

	//How? How to handle it being non-resident, but still referenced? Is this even relevant? Should a texture be non-resident but still referencable?
	internal void MakeResident() {
		if (Disposed)
			throw new ObjectDisposedException( DisplayName );
		if (Resident)
			return;
		Gl.MakeTextureHandleResidentARB( _handle );
	}

	internal void MakeNonResident() {
		if (Disposed)
			throw new ObjectDisposedException( DisplayName );
		if (!Resident)
			return;
		Gl.MakeTextureHandleNonResidentARB( _handle );
	}

	public void SetPixels( PixelFormat format, PixelType pixelType, nint ptr, uint level = 0 ) {
		if (level > _levels.Count) {
			_warningLog.LogWarning( "Attempted to set pixels at a deeper mipmap level than present." );
			return;
		}
		Vector2i mm = _levels[ (int) level ];
		Gl.TextureSubImage2D( TextureID, (int) level, 0, 0, mm.X, mm.Y, format, pixelType, ptr );
	}

	public void SetPixelsCompressed( PixelFormat format, int size, nint ptr, uint level = 0 ) {
		if (level > _levels.Count) {
			_warningLog.LogWarning( "Attempted to set pixels at a deeper mipmap level than present." );
			return;
		}
		Vector2i mm = _levels[ (int) level ];
		Gl.CompressedTextureSubImage2D( TextureID, (int) level, 0, 0, mm.X, mm.Y, format, size, ptr );
	}

	protected override bool InternalDispose() {
		Gl.DeleteTextures( [ TextureID ] );
		return true;
	}

	public sealed class TextureReference {
		private readonly OglTextureBase<T> _texture;

		internal event Action? OnDestruction;

		public TextureReference( OglTextureBase<T> texture ) {
			_texture = texture;
		}

		~TextureReference() {
			OnDestruction?.Invoke();
		}

		internal ulong GetHandle() => _texture.Handle;
	}
}