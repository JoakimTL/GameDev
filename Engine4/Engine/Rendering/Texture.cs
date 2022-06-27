using Engine.Data.Datatypes;
using OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using System.Runtime.CompilerServices;
using Engine.Rendering.Disposal;
using Engine.Rendering.ResourceManagement;
using Engine.Time;

namespace Engine.Rendering;
public class Texture : DisposableIdentifiable {
	private readonly ulong _handle;
	private readonly Vector2i[] _sizes;
	public TextureTarget Target { get; }
	public Vector2i Level0 => this._sizes[ 0 ];
	public int Levels => this._sizes.Length;
	public uint TextureID { get; private set; }
	public bool Resident { get; private set; }
	public float LastBind { get; private set; }

	protected override string UniqueNameTag => $"{this.TextureID},{this._handle}";

	public Texture( string name, TextureTarget target, Vector2i[] sizes, InternalFormat format, params (TextureParameterName, int)[] parameters ) : base( name ) {
		if ( sizes.Length == 0 )
			throw new ArgumentOutOfRangeException( nameof( sizes ), "Must be greater than zero" );
		this.Target = target;
		this._sizes = sizes;
		this.Resident = false;
		this.LogLine( $"Created with format [{format}]!", Log.Level.LOW );

		this.TextureID = Gl.CreateTexture( this.Target );
		Gl.TextureStorage2D( this.TextureID, sizes.Length, format, sizes[ 0 ].X, sizes[ 0 ].Y );
		for ( int i = 0; i < parameters.Length; i++ )
			Gl.TextureParameter( this.TextureID, parameters[ i ].Item1, parameters[ i ].Item2 );
		this._handle = Gl.GetTextureHandleARB( this.TextureID );
	}

	public Texture( string name, TextureTarget target, Vector2i size, InternalFormat format, params (TextureParameterName, int)[] parameters ) : this( name, target, new Vector2i[] { size }, format, parameters ) { }

	public Vector2i GetMipmap( uint level ) {
		if ( level > this._sizes.Length )
			return 0;
		return this._sizes[ level ];
	}

	public void SetPixels( PixelFormat format, PixelType pixelType, IntPtr ptr, uint level = 0 ) {
		if ( level > this._sizes.Length ) {
			this.LogWarning( "Attempted to set pixels at a deeper mipmap level than present." );
			return;
		}
		Vector2i mm = this._sizes[ level ];
		Gl.TextureSubImage2D( this.TextureID, (int) level, 0, 0, mm.X, mm.Y, format, pixelType, ptr );
	}

	public void SetPixelsCompressed( PixelFormat format, int size, IntPtr ptr, uint level = 0 ) {
		if ( level > this._sizes.Length ) {
			this.LogWarning( "Attempted to set pixels at a deeper mipmap level than present." );
			return;
		}
		Vector2i mm = this._sizes[ level ];
		Gl.CompressedTextureSubImage2D( this.TextureID, (int) level, 0, 0, mm.X, mm.Y, format, size, ptr );
	}

	public ulong GetHandleDirect() {
		if ( !this.Resident )
			DirectBind();
		return this._handle;
	}

	/// <summary>
	/// Makes the texture resident.
	/// </summary>
	public void DirectBind() {
		if ( this.Resident ) {
			this.LastBind = Clock32.StartupTime;
			return;
		}

		this.LogLine( $"Made resident!", Log.Level.LOW );
		this.Resident = true;
		Gl.MakeTextureHandleResidentARB( this._handle );
	}

	/// <summary>
	/// Makes the texture non-resident.
	/// </summary>
	public void DirectUnbind() {
		if ( !this.Resident )
			return;

		this.LogLine( $"Made non-resident!", Log.Level.LOW );
		this.Resident = false;
		Gl.MakeTextureHandleNonResidentARB( this._handle );
	}

	protected override bool OnDispose() {
		if ( !Resources.Render.InThread ) {
			Resources.Render.ContextDiposer.Add( new TextureDisposal( this.FullName, this._handle, this.TextureID, this.Resident ) );
			return true;
		}
		DirectUnbind();
		Gl.DeleteTextures( new uint[] { this.TextureID } );
		return true;
	}

	public static bool LoadFile( TextureManager textureManager, string filepath, out Texture t, TextureMagFilter filter = TextureMagFilter.Linear ) {
		if ( !File.Exists( filepath ) ) {
			Log.Warning( $"Couldn't LOCATE {filepath}, using default 1x1 white texture!" );
			t = textureManager.White1x1;
			return false;
		}

		uint[] pixelData;
		using ( Image<Rgba32>? img = Image.Load<Rgba32>( filepath ) ) {
			IMemoryGroup<Rgba32>? pixels = img.GetPixelMemoryGroup();
			if ( pixels is null ) {
				Log.Warning( $"Couldn't LOAD {filepath}, using default 1x1 white texture!" );
				t = textureManager.White1x1;
				return false;
			}

			pixelData = new uint[ pixels.TotalLength ];

			unsafe {
				uint bytesCopied = 0;
				fixed ( uint* dst = pixelData ) {
					foreach ( Memory<Rgba32> memory in pixels ) {
						using ( System.Buffers.MemoryHandle memHandle = memory.Pin() ) {
							uint bytesToCopy = (uint) memory.Length * sizeof( uint );
							Unsafe.CopyBlock( &dst[ bytesCopied ], memHandle.Pointer, bytesToCopy );
							bytesCopied += bytesToCopy;
						}
					}
				}
			}

			t = new Texture( filepath,
				TextureTarget.Texture2d,
				(img.Width, img.Height),
				InternalFormat.Rgba8,
				(TextureParameterName.TextureMagFilter, (int) filter),
				(TextureParameterName.TextureMinFilter, (int) filter)
			);
		}

		unsafe {
			fixed ( uint* src = pixelData )
				t.SetPixels( PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr( src ) );
		}

		Log.Line( $"[{filepath}] loaded as GL Texture [{t}]!", Log.Level.NORMAL );
		return true;

	}
}