using Engine.Datatypes.Vectors;
using OpenGL;

namespace Engine.Rendering.Objects;

public class Texture : Identifiable, IDisposable {
	private readonly ulong _handle;
	private readonly Vector2i[] _sizes;
	public TextureTarget Target { get; }
	public Vector2i Level0 => _sizes[ 0 ];
	public int Levels => _sizes.Length;
	public uint TextureID { get; private set; }
	public bool Resident { get; private set; }

	protected override string UniqueNameTag => $"{TextureID},{_handle}";

	public Texture( string name, TextureTarget target, Vector2i[] sizes, InternalFormat format, int samples = 0, params (TextureParameterName, int)[] parameters ) : base( name ) {
		if ( sizes.Length == 0 )
			throw new ArgumentOutOfRangeException( nameof( sizes ), "Must be greater than zero" );
		Target = target;
		_sizes = sizes;
		Resident = false;
		this.LogLine( $"Created with format [{format}]!", Log.Level.LOW );

		TextureID = Gl.CreateTexture( Target );
		bool mipmaps = sizes.Length > 1 || samples == 0;
		if ( samples != 0 && sizes.Length > 1 )
			this.LogWarning( "Can't have mipmaps and multisampling in the same texture! Using mipmaps!" );
		if ( mipmaps )
			Gl.TextureStorage2D( TextureID, sizes.Length, format, sizes[ 0 ].X, sizes[ 0 ].Y );
		else
			Gl.TextureStorage2DMultisample( TextureID, samples, format, sizes[ 0 ].X, sizes[ 0 ].Y, false );

		for ( int i = 0; i < parameters.Length; i++ )
			Gl.TextureParameter( TextureID, parameters[ i ].Item1, parameters[ i ].Item2 );
		_handle = Gl.GetTextureHandleARB( TextureID );
	}

	public Texture( string name, TextureTarget target, Vector2i size, InternalFormat format, int samples = 0, params (TextureParameterName, int)[] parameters ) : this( name, target, new Vector2i[] { size }, format, samples, parameters ) { }

#if DEBUG
	~Texture() {
		System.Diagnostics.Debug.Fail( "Texture was not disposed!" );
	}
#endif

	public Vector2i GetMipmap( uint level ) {
		if ( level > _sizes.Length )
			return 0;
		return _sizes[ level ];
	}

	public void SetPixels( PixelFormat format, PixelType pixelType, nint ptr, uint level = 0 ) {
		if ( level > _sizes.Length ) {
			this.LogWarning( "Attempted to set pixels at a deeper mipmap level than present." );
			return;
		}
		Vector2i mm = _sizes[ level ];
		Gl.TextureSubImage2D( TextureID, (int) level, 0, 0, mm.X, mm.Y, format, pixelType, ptr );
	}

	public void SetPixelsCompressed( PixelFormat format, int size, nint ptr, uint level = 0 ) {
		if ( level > _sizes.Length ) {
			this.LogWarning( "Attempted to set pixels at a deeper mipmap level than present." );
			return;
		}
		Vector2i mm = _sizes[ level ];
		Gl.CompressedTextureSubImage2D( TextureID, (int) level, 0, 0, mm.X, mm.Y, format, size, ptr );
	}

	public ulong GetHandle() {
		if ( !Resident )
			Bind();
		return _handle;
	}

	/// <summary>
	/// Makes the texture resident.
	/// </summary>
	public void Bind() {
		if ( Resident )
			return;
		this.LogLine( $"Made resident!", Log.Level.LOW );
		Resident = true;
		Gl.MakeTextureHandleResidentARB( _handle );
	}

	/// <summary>
	/// Makes the texture non-resident.
	/// </summary>
	public void Unbind() {
		if ( !Resident )
			return;

		this.LogLine( $"Made non-resident!", Log.Level.LOW );
		Resident = false;
		Gl.MakeTextureHandleNonResidentARB( _handle );
	}

	public void Dispose() {
		Unbind();
		Gl.DeleteTextures( new uint[] { TextureID } );
		GC.SuppressFinalize( this );
	}
}
