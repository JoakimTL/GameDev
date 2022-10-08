using Engine.Data.Datatypes;

namespace Engine.Rendering.Services;

[Structure.ProcessBefore( typeof( WindowService ), typeof( IDisposable ) )]
[Structure.ProcessAfter( typeof( WindowService ), typeof( IInitializable ) )]
public class TextureManager : ModuleService, IInitializable {

	private bool _initialized;
	private Texture? _white1x1;
	private Texture? _red1x1;
	private Texture? _green1x1;
	private Texture? _blue1x1;
	private Texture? _black1x1;
	public Texture White1x1 => this._white1x1 ?? throw new NullReferenceException( "Manager not initialized!" );
	public Texture Red1x1 => this._red1x1 ?? throw new NullReferenceException( "Manager not initialized!" );
	public Texture Green1x1 => this._green1x1 ?? throw new NullReferenceException( "Manager not initialized!" );
	public Texture Blue1x1 => this._blue1x1 ?? throw new NullReferenceException( "Manager not initialized!" );
	public Texture Black1x1 => this._black1x1 ?? throw new NullReferenceException( "Manager not initialized!" );

	private readonly Dictionary<string, Texture> _textures;

	public TextureManager() {
		this._textures = new Dictionary<string, Texture>();
	}

	public void Initialize() {
		if ( this._initialized ) {
			this.LogError( "Already initialized!" );
			return;
		}
		this._initialized = true;
		unsafe {
			this._white1x1 = new Texture( nameof( this.White1x1 ), OpenGL.TextureTarget.Texture2d, 1, OpenGL.InternalFormat.Rgba8 );
			this._red1x1 = new Texture( nameof( this.Red1x1 ), OpenGL.TextureTarget.Texture2d, 1, OpenGL.InternalFormat.Rgba8 );
			this._green1x1 = new Texture( nameof( this.Green1x1 ), OpenGL.TextureTarget.Texture2d, 1, OpenGL.InternalFormat.Rgba8 );
			this._blue1x1 = new Texture( nameof( this.Blue1x1 ), OpenGL.TextureTarget.Texture2d, 1, OpenGL.InternalFormat.Rgba8 );
			this._black1x1 = new Texture( nameof( this.Black1x1 ), OpenGL.TextureTarget.Texture2d, 1, OpenGL.InternalFormat.Rgba8 );
			Vector4b whiteColor = Vector4b.White;
			Vector4b redColor = Vector4b.Red;
			Vector4b greenColor = Vector4b.Green;
			Vector4b blueColor = Vector4b.Blue;
			Vector4b blackColor = Vector4b.Black;
			this.White1x1.SetPixels( OpenGL.PixelFormat.Rgba, OpenGL.PixelType.UnsignedByte, new IntPtr( &whiteColor ) );
			this.Red1x1.SetPixels( OpenGL.PixelFormat.Rgba, OpenGL.PixelType.UnsignedByte, new IntPtr( &redColor ) );
			this.Green1x1.SetPixels( OpenGL.PixelFormat.Rgba, OpenGL.PixelType.UnsignedByte, new IntPtr( &greenColor ) );
			this.Blue1x1.SetPixels( OpenGL.PixelFormat.Rgba, OpenGL.PixelType.UnsignedByte, new IntPtr( &blueColor ) );
			this.Black1x1.SetPixels( OpenGL.PixelFormat.Rgba, OpenGL.PixelType.UnsignedByte, new IntPtr( &blackColor ) );
		}
	}

	/// <summary>
	/// Will load a texture file from storage and store it.
	/// </summary>
	/// <param name="fileName">Name of the image file in the texture folder, does not include extensions. .png is the only supported extension.</param>
	public Texture Get( string fileName ) {
		if ( string.IsNullOrEmpty( fileName ) ) {
			this.LogWarning( "Must have a valid name for a texture!" );
			return this.White1x1;
		}
		if ( !this._textures.TryGetValue( fileName, out Texture? t ) )
			if ( Texture.LoadFile( this, $"res/textures/{fileName}.png", out t ) )
				this._textures.Add( fileName, t );
		return t;
	}

	/// <summary>
	/// Will load a texture file from storage and store it. Uses a direct path rather than an asset name
	/// </summary>
	public Texture GetFromPath( string filePath ) {
		if ( string.IsNullOrEmpty( filePath ) ) {
			this.LogWarning( "Must have a valid name for a texture!" );
			return this.White1x1;
		}
		if ( !this._textures.TryGetValue( filePath, out Texture? t ) )
			if ( Texture.LoadFile( this, filePath, out t ) )
				this._textures.Add( filePath, t );
		return t;
	}

	protected override bool OnDispose() {
		foreach ( Texture t in this._textures.Values )
			t.Dispose();
		return this._textures.Values.All( p => p.Disposed );
	}
}
