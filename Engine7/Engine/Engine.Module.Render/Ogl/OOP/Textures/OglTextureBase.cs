using Engine.Logging;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Textures;

public abstract class OglTextureBase<T> : DisposableIdentifiable where T : struct {
	public readonly uint TextureID;
	public readonly TextureTarget Target;
	public readonly T Metadata;
	protected readonly ulong _handle;
	protected readonly IReadOnlyList<Vector2<int>> _levels;
	private uint _referenceCount;

	/// <summary>
	/// Level 0 is guaranteed to exist, with a size of 1x1 or greater.
	/// </summary>
	public Vector2<int> Level0 => this._levels[ 0 ];
	public bool Resident { get; private set; }

	public OglTextureBase( string name, TextureTarget target, Vector2<int> level0, T metadata, params (TextureParameterName, int)[] parameters ) {
		if (level0.X <= 0 || level0.Y <= 0)
			throw new OpenGlArgumentException( "Texture size must be greater than zero on both axis", nameof( level0 ) );
		this.Target = target;
		this._levels = GetLevels( level0, metadata );
		this.Resident = false;

		this.TextureID = Gl.CreateTexture( this.Target );
		var error = Gl.GetError();
		if (error != ErrorCode.NoError)
			this.LogWarning( $"Error creating texture 1: {error}" );
		GenerateTexture( metadata );
		error = Gl.GetError();
		if (error != ErrorCode.NoError)
			this.LogWarning( $"Error creating texture 2: {error}" );
		for (int i = 0; i < parameters.Length; i++) {
			Gl.TextureParameter( this.TextureID, parameters[ i ].Item1, parameters[ i ].Item2 );
			error = Gl.GetError();
			if (error != ErrorCode.NoError)
				this.LogWarning( $"Error creating texture 3 {i}: {error}" );
		}
		error = Gl.GetError();
		if (error != ErrorCode.NoError)
			this.LogWarning( $"Error creating texture 4: {error}" );
		this._handle = Gl.GetTextureHandleARB( this.TextureID );
		this.Nickname = $"TEX{this.TextureID} {name}";
	}

	protected abstract void GenerateTexture( T metadata );

	protected abstract IReadOnlyList<Vector2<int>> GetLevels( Vector2<int> level0, T metadata );

	internal ulong Handle => this._handle;

	public Vector2<int> GetLevel( uint level ) {
		if (level > this._levels.Count) {
			this.LogWarning( $"Has no level {level}" );
			return (0, 0);
		}
		return this._levels[ (int) level ];
	}

	public TextureReference GetTextureReference() {
		if (this.Disposed)
			throw new ObjectDisposedException( this.FullName );
		TextureReference reference = new( this );
		reference.OnDestruction += OnReferenceDestruction;
		if (this._referenceCount == 0)
			MakeResident();
		this._referenceCount++;
		return reference;
	}

	private void OnReferenceDestruction() {
		this._referenceCount--;
		if (this._referenceCount == 0 && !Disposed)
			MakeNonResident();
	}

	//How? How to handle it being non-resident, but still referenced? Is this even relevant? Should a texture be non-resident but still referencable?
	internal void MakeResident() {
		if (this.Disposed)
			throw new ObjectDisposedException( this.FullName );
		if (this.Resident)
			return;
		Gl.MakeTextureHandleResidentARB( this._handle );
	}

	internal void MakeNonResident() {
		if (this.Disposed)
			throw new ObjectDisposedException( this.FullName );
		if (!this.Resident)
			return;
		Gl.MakeTextureHandleNonResidentARB( this._handle );
	}

	public void SetPixels( PixelFormat format, PixelType pixelType, nint ptr, uint level = 0 ) {
		if (level > this._levels.Count) {
			this.LogWarning( "Attempted to set pixels at a deeper mipmap level than present." );
			return;
		}
		Vector2<int> mm = this._levels[ (int) level ];
		Gl.TextureSubImage2D( this.TextureID, (int) level, 0, 0, mm.X, mm.Y, format, pixelType, ptr );
	}

	public void SetPixelsCompressed( PixelFormat format, int size, nint ptr, uint level = 0 ) {
		if (level > this._levels.Count) {
			this.LogWarning( "Attempted to set pixels at a deeper mipmap level than present." );
			return;
		}
		Vector2<int> mm = this._levels[ (int) level ];
		Gl.CompressedTextureSubImage2D( this.TextureID, (int) level, 0, 0, mm.X, mm.Y, format, size, ptr );
	}

	protected override bool InternalDispose() {
		Gl.DeleteTextures( [ this.TextureID ] );
		return true;
	}

	public sealed class TextureReference {
		private readonly OglTextureBase<T> _texture;

		internal event Action? OnDestruction;

		public TextureReference( OglTextureBase<T> texture ) {
			this._texture = texture;
		}

		~TextureReference() {
			OnDestruction?.Invoke();
		}

		public ulong GetHandle() => this._texture.Handle;
	}
}