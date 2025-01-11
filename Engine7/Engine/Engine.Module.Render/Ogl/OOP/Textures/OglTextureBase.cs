using Engine.Logging;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Textures;

public abstract class OglTextureBase : DisposableIdentifiable, IOglTexture {

	private bool _generated;

	private uint _referenceCount;
	private uint _textureID;
	private bool _resident;
	private ulong _handle;
	private readonly TextureTarget _target;
	private readonly List<Vector2<int>> _levels;
	private readonly (TextureParameterName name, int value)[] _parameters;

	public uint TextureID => GetTextureId();
	public TextureTarget Target => this._target;
	public bool Resident => this._resident;
	public Vector2<int> Level0 => this._levels[ 0 ];
	internal ulong Handle => GetTextureHandle();

	protected OglTextureBase( string name, TextureTarget target, Vector2<int> level0, params Span<(TextureParameterName, int)> parameters ) {
		if (level0.X <= 0 || level0.Y <= 0)
			throw new OpenGlArgumentException( "Texture size must be greater than zero on both axis", nameof( level0 ) );
		_textureID = 0;
		_target = target;
		_levels = [ level0 ];
		_parameters = [ .. parameters ];
		_resident = false;
		_handle = 0;
		_generated = false;
		Nickname = name;
	}

	private uint GetTextureId() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Accessed();
		return this._textureID;
	}

	private ulong GetTextureHandle() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Accessed();
		return this._handle;
	}

	private void Accessed() {
		if (_generated)
			return;
		Generate();
		_generated = true;
	}

	protected void Generate() {
		if (_generated)
			throw new InvalidOperationException( "Texture already generated." );
		AddLevels( _levels );
		_textureID = Gl.CreateTexture( _target );
		GenerateTexture( _textureID );
		for (int i = 0; i < _parameters.Length; i++)
			Gl.TextureParameter( _textureID, _parameters[ i ].name, _parameters[ i ].value );
		this._handle = Gl.GetTextureHandleARB( _textureID );
	}

	protected abstract void AddLevels( List<Vector2<int>> levelsList );
	protected abstract void GenerateTexture( uint textureId );

	public Vector2<int> GetLevel( uint level ) {
		if (level >= this._levels.Count)
			return this.LogWarningThenReturn( $"Has no level {level}", 0 );
		return this._levels[ (int) level ];
	}

	public TextureReference GetTextureReference() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Accessed();
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

	internal void MakeResident() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Accessed();
		if (this.Resident)
			return;
		_resident = true;
		Gl.MakeTextureHandleResidentARB( this._handle );
	}

	internal void MakeNonResident() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Accessed();
		if (!this.Resident)
			return;
		_resident = false;
		Gl.MakeTextureHandleNonResidentARB( this._handle );
	}

	public void SetPixels( PixelFormat format, PixelType pixelType, nint ptr, uint level = 0 ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Accessed();
		if (level > this._levels.Count) {
			this.LogWarning( "Attempted to set pixels at a deeper mipmap level than present." );
			return;
		}
		Vector2<int> mm = this._levels[ (int) level ];
		Gl.TextureSubImage2D( this.TextureID, (int) level, 0, 0, mm.X, mm.Y, format, pixelType, ptr );
	}

	public void SetPixelsCompressed( PixelFormat format, int size, nint ptr, uint level = 0 ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Accessed();
		if (level > this._levels.Count) {
			this.LogWarning( "Attempted to set pixels at a deeper mipmap level than present." );
			return;
		}
		Vector2<int> mm = this._levels[ (int) level ];
		Gl.CompressedTextureSubImage2D( this.TextureID, (int) level, 0, 0, mm.X, mm.Y, format, size, ptr );
	}

	protected override bool InternalDispose() {
		if (!_generated)
			return true;
		Gl.DeleteTextures( [ this.TextureID ] );
		return true;
	}
}

public abstract class OglTextureBase<T> : OglTextureBase where T : struct {
	public readonly T Metadata;

	public OglTextureBase( string name, TextureTarget target, Vector2<int> level0, T metadata, params Span<(TextureParameterName, int)> parameters ) : base( name, target, level0, parameters ) {
		Metadata = metadata;
	}
}
