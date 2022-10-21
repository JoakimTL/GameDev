using Engine.Datatypes;
using Engine.Rendering.Objects;
using OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.CompilerServices;

namespace Engine.Rendering.Services;

public sealed class TextureService : IContextService, IInitializable, IDisposable {

	private Texture _white1x1 = null!;
	private Texture _red1x1 = null!;
	private Texture _green1x1 = null!;
	private Texture _blue1x1 = null!;
	private Texture _black1x1 = null!;
	public Texture White1x1 => this._white1x1 ?? throw new NullReferenceException( "Manager not initialized!" );
	public Texture Red1x1 => this._red1x1 ?? throw new NullReferenceException( "Manager not initialized!" );
	public Texture Green1x1 => this._green1x1 ?? throw new NullReferenceException( "Manager not initialized!" );
	public Texture Blue1x1 => this._blue1x1 ?? throw new NullReferenceException( "Manager not initialized!" );
	public Texture Black1x1 => this._black1x1 ?? throw new NullReferenceException( "Manager not initialized!" );

	private readonly Dictionary<string, Texture> _textures;

	public TextureService() {
		_textures = new();
	}

	public void Initialize() {
		unsafe {
			this._white1x1 = new Texture( nameof( this.White1x1 ), TextureTarget.Texture2d, 1, InternalFormat.Rgba8 );
			this._red1x1 = new Texture( nameof( this.Red1x1 ), TextureTarget.Texture2d, 1, InternalFormat.Rgba8 );
			this._green1x1 = new Texture( nameof( this.Green1x1 ), TextureTarget.Texture2d, 1, InternalFormat.Rgba8 );
			this._blue1x1 = new Texture( nameof( this.Blue1x1 ), TextureTarget.Texture2d, 1, InternalFormat.Rgba8 );
			this._black1x1 = new Texture( nameof( this.Black1x1 ), TextureTarget.Texture2d, 1, InternalFormat.Rgba8 );
			Vector4b whiteColor = Vector4b.White;
			Vector4b redColor = Vector4b.Red;
			Vector4b greenColor = Vector4b.Green;
			Vector4b blueColor = Vector4b.Blue;
			Vector4b blackColor = Vector4b.Black;
			this.White1x1.SetPixels( PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr( &whiteColor ) );
			this.Red1x1.SetPixels( PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr( &redColor ) );
			this.Green1x1.SetPixels( PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr( &greenColor ) );
			this.Blue1x1.SetPixels( PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr( &blueColor ) );
			this.Black1x1.SetPixels( PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr( &blackColor ) );
		}
	}

	public void Dispose() {
		this.White1x1.Dispose();
		this.Red1x1.Dispose();
		this.Green1x1.Dispose();
		this.Blue1x1.Dispose();
		this.Black1x1.Dispose();
		foreach ( Texture t in this._textures.Values )
			t.Dispose();
	}

	public Texture Get( string path ) {
		if ( _textures.TryGetValue( path, out var texture ) )
			return texture;
		if ( LoadFile( path, out texture ) )
			_textures.Add( path, texture );
		return texture;
		//TODO add system that automatically disposes unused textures after a while. (60sec?)
	}

	private bool LoadFile( string filepath, out Texture t, int samples = 0, TextureMagFilter filter = TextureMagFilter.Linear ) {
		if ( !File.Exists( filepath ) ) {
			Log.Warning( $"Failed to find {filepath}, using default 1x1 white texture!" );
			t = this.White1x1;
			return false;
		}

		uint[] pixelData;
		using ( Image<Rgba32>? img = Image.Load<Rgba32>( filepath ) ) {
			IMemoryGroup<Rgba32>? pixels = img.GetPixelMemoryGroup();
			if ( pixels is null ) {
				Log.Warning( $"Failed to load {filepath}, using default 1x1 white texture!" );
				t = this.White1x1;
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
				samples,
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
