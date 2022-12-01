using Engine.Datatypes.Vectors;
using Engine.GlobalServices;
using Engine.Rendering.Objects;
using Engine.Structure.Interfaces;
using OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Engine.Rendering.Services;

public sealed class TextureService : IContextService, IUpdateable, IDisposable {

	private readonly Dictionary<string, Texture> _textures;
	private readonly ConcurrentQueue<string> _updatedTextures;
	private readonly FileWatchingService _fileWatchingService;

	public TextureService( FileWatchingService fileWatchingService ) {
		_textures = new();
		_updatedTextures = new();
		this._fileWatchingService = fileWatchingService;
	}

	public void Dispose() {
		foreach ( Texture t in this._textures.Values )
			t.Dispose();
	}

	public Texture? Get( string path ) {
		if ( _textures.TryGetValue( path, out var texture ) )
			return texture;
		if ( LoadFile( path, out texture ) )
			_textures.Add( path, texture );
		return texture;
	}

	private void FileChanged( string path ) => _updatedTextures.Enqueue( path );

	public void Update( float time, float deltaTime ) {
		while ( _updatedTextures.TryDequeue( out string? path ) )
			if ( _textures.TryGetValue( path, out var texture ) ) {
				if ( LoadRawData( path, out uint[]? pixelData, out Vector2i res ) )
					unsafe {
						fixed ( uint* src = pixelData )
							texture.SetPixels( PixelFormat.Rgba, PixelType.UnsignedByte, new nint( src ) );
					}
			}
	}

	private bool LoadFile( string filepath, [NotNullWhen( true )] out Texture? t, int samples = 0, TextureMagFilter filter = TextureMagFilter.Linear ) {
		t = null;
		if ( !File.Exists( filepath ) ) {
			Log.Warning( $"Failed to find texture {filepath}!" );
			return false;
		}

		if ( !LoadRawData( filepath, out uint[]? pixelData, out Vector2i res ) )
			return false;

		t = new( filepath, TextureTarget.Texture2d, res, InternalFormat.Rgba8, samples, (TextureParameterName.TextureMagFilter, (int) filter), (TextureParameterName.TextureMinFilter, (int) filter) );
		_fileWatchingService.Track( filepath, FileChanged );

		unsafe {
			fixed ( uint* src = pixelData )
				t.SetPixels( PixelFormat.Rgba, PixelType.UnsignedByte, new nint( src ) );
		}

		Log.Line( $"[{filepath}] loaded as GL Texture [{t}]!", Log.Level.NORMAL );
		return true;

	}

	private bool LoadRawData( string filepath, [NotNullWhen( true )] out uint[]? pixelData, out Vector2i resolution ) {
		pixelData = null;
		resolution = 0;
		if ( !File.Exists( filepath ) ) {
			Log.Warning( $"Failed to find texture {filepath}!" );
			return false;
		}

		using ( Image<Rgba32>? img = Image.Load<Rgba32>( filepath ) ) {
			resolution = (img.Width, img.Height);
			IMemoryGroup<Rgba32>? pixels = img.GetPixelMemoryGroup();
			if ( pixels is null ) {
				Log.Warning( $"Failed to load texture {filepath}!" );
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
		}
		return true;

	}
}
