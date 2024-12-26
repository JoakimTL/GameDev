using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Graphics.Disposables;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.Graphics.Objects {
	public class Texture : Cacheable {

		public uint TextureID { get; private set; }

		public Vector2i Size { get; private set; }

		public TextureTarget TextureTarget { get; private set; }

		public Texture( string name, uint textureID, TextureTarget textureTarget, Vector2i size ) : base( name ) {
			this.TextureID = textureID;
			this.TextureTarget = textureTarget;
			this.Size = size;
		}

		public Texture( string name, uint textureID, TextureTarget textureTarget ) : base( name ) {
			this.TextureID = textureID;
			this.TextureTarget = textureTarget;
			this.Size = Vector2i.Zero;
		}

		public Texture( string name, Bitmap bitmap, TextureMagFilter filter = TextureMagFilter.Linear, bool flipY = true ) : base( name ) {
			TextureLoader.LoadBitmap( bitmap, out uint textureID, out TextureTarget textureTarget, out Vector2i size, filter, flipY );
			Gl.BindTexture( textureTarget, 0 );
			this.TextureID = textureID;
			this.TextureTarget = textureTarget;
			this.Size = size;

			Mem.Logs.MemoryLogger.WriteLine( $"Loaded bitmap [{name}] into GL texture [{textureID},{size}]!" );
		}

#if DEBUG
		~Texture() {
			if( TextureID != 0 )
				new OGLTextureDisposer( Name, TextureID );
		}
#endif

		public void SetPixel( Vector2 point, Vector4b color ) {
			if( point.X < 0 || point.Y < 0 || point.X > 1 || point.Y > 1 )
				return;

			unsafe {
				Gl.BindTexture( TextureTarget, TextureID );
				Vector4b* colorPtr = &color;
				int xoffset = (int) Math.Min( Math.Floor( point.X * Size.X ), Size.X - 1 );
				int yoffset = (int) Math.Min( Math.Floor( point.Y * Size.Y ), Size.Y - 1 );
				Gl.TexSubImage2D( TextureTarget, 0, xoffset, yoffset, 1, 1, OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr) colorPtr );
			}
		}

		public void SetPixels( Vector2 point, Vector4b[] colorData, Vector2i pixelArea ) {
			if( point.X < 0 || point.Y < 0 || point.X > 1 || point.Y > 1 )
				return;

			int xoffset = (int) Math.Min( Math.Floor( point.X * Size.X ), Size.X - 1 );
			int yoffset = (int) Math.Min( Math.Floor( point.Y * Size.Y ), Size.Y - 1 );

			if( xoffset + pixelArea.X > Size.X - 1 || yoffset + pixelArea.Y > Size.Y )
				return;

			unsafe {
				fixed( Vector4b* colorPtr = colorData ) {
					Gl.BindTexture( TextureTarget, TextureID );
					Gl.TexSubImage2D( TextureTarget, 0, xoffset, yoffset, pixelArea.X, pixelArea.Y, OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr) colorPtr );
				}
			}
		}

		public static Texture CreateNew( string name, Vector2i size, TextureMagFilter filter = TextureMagFilter.Linear ) {

			uint textureID = Gl.GenTexture();
			TextureTarget textureTarget = TextureTarget.Texture2d;

			Gl.PixelStore( PixelStoreParameter.UnpackAlignment, 1 );
			Gl.BindTexture( textureTarget, textureID );

			Gl.TexImage2D( textureTarget, 0, InternalFormat.Rgba8, size.X, size.Y, 0, OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr) 0 );
			Gl.TexParameter( textureTarget, TextureParameterName.TextureMagFilter, (int) filter );
			Gl.TexParameter( textureTarget, TextureParameterName.TextureMinFilter, (int) filter );

			return new Texture( name, textureID, textureTarget, size );

		}

		public static bool FromFile( string filepath, out Texture t, TextureMagFilter filter = TextureMagFilter.Linear ) {
			if( !File.Exists( filepath ) ) {
				Logging.Error( $"Couldn't locate {filepath}, using default texture!" );
				t = Mem.Textures.BlankWhite;
				return false;
			}

			uint textureID;
			TextureTarget textureTarget;
			Vector2i size;

			switch( new FileInfo( filepath ).Extension.ToLower() ) {
				case ".dds":
					TextureLoader.LoadDDS( filepath, out textureID, out textureTarget, out size );
					break;
				default:
					TextureLoader.LoadBitmap( (Bitmap) Bitmap.FromFile( filepath ), out textureID, out textureTarget, out size, filter );
					break;
			}

			Gl.BindTexture( textureTarget, 0 );

			Logging.Routine( $"[TEXTURE] Loaded texture file [{filepath}] into GL texture [{textureID},{size}]!" );

			t = new Texture( filepath, textureID, textureTarget, size );
			return true;

		}

		public override void Dispose() {
			if( TextureID != 0 ) {
				Gl.DeleteTextures( new uint[] { TextureID } );
				Mem.Logs.MemoryLogger.WriteLine( $"[{Name}]: Disposed [{TextureID}]!" );
				TextureID = 0;
			}
		}

	}

	internal static class TextureLoader {
		public static void LoadBitmap( Bitmap BitmapImage, out uint textureID, out TextureTarget textureTarget, out Vector2i size, TextureMagFilter filter = TextureMagFilter.Linear, bool FlipY = true ) {
			/* .net library has methods for converting many image formats so I exploit that by using 
			 * .net to convert any filetype to a bitmap.  Then the bitmap is locked into memory so
			 * that the garbage collector doesn't touch it, and it is read via OpenGL glTexImage2D. */
			if( FlipY )
				BitmapImage.RotateFlip( RotateFlipType.RotateNoneFlipY );     // bitmaps read from bottom up, so flip it
			size = new Vector2i( BitmapImage.Size.Width, BitmapImage.Size.Height );

			// must be Format32bppArgb file format, so convert it if it isn't in that format
			BitmapData bitmapData = BitmapImage.LockBits( new System.Drawing.Rectangle( 0, 0, BitmapImage.Width, BitmapImage.Height ), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb );

			// set the texture target and then generate the texture ID
			textureTarget = TextureTarget.Texture2d;
			textureID = Gl.GenTexture();

			Gl.PixelStore( PixelStoreParameter.UnpackAlignment, 1 ); // set pixel alignment
			Gl.BindTexture( textureTarget, textureID );     // bind the texture to memory in OpenGL

			//GL.TexParameteri(TextureTarget, TextureParameterName.GenerateMipmap, 0);
			Gl.TexImage2D( textureTarget, 0, InternalFormat.Rgba8, BitmapImage.Width, BitmapImage.Height, 0, OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0 );
			Gl.TexParameter( textureTarget, TextureParameterName.TextureMagFilter, (int) filter );
			Gl.TexParameter( textureTarget, TextureParameterName.TextureMinFilter, (int) filter );

			BitmapImage.UnlockBits( bitmapData );
			BitmapImage.Dispose();
		}

		/// <summary>
		/// Loads a compressed DDS file into an OpenGL texture.
		/// </summary>
		/// <param name="ResourceFile">The path to the DDS file.</param>
		public static void LoadDDS( string ResourceFile, out uint textureID, out TextureTarget textureTarget, out Vector2i size ) {
			using BinaryReader stream = new BinaryReader( new FileStream( ResourceFile, FileMode.Open ) );
			string filecode = new string( stream.ReadChars( 4 ) );
			if( filecode != "DDS " )                                 // first 4 chars should be "DDS "
				throw new Exception( "File was not a DDS file format." );

			DDS.DDSURFACEDESC2 imageData = DDS.DDSURFACEDESC2.FromBinaryReader( stream );//new DDS.DDSURFACEDESC2(stream);  // read the DirectDraw surface descriptor
			size = new Vector2i( imageData.Width, imageData.Height );

			if( imageData.LinearSize == 0 )
				throw new Exception( "The linear scan line size was zero." );

			bool compressed = true;
			int factor = 0, buffersize = 0, blocksize = 0;
			InternalFormat format;
			switch( imageData.PixelFormat.FourCC )       // check the compression type
			{
				case "DXT1":    // DXT1 compression ratio is 8:1
					format = InternalFormat.CompressedRgbaS3tcDxt1Ext;
					factor = 2;
					blocksize = 8;
					break;
				case "DXT3":    // DXT3 compression ratio is 4:1
					format = InternalFormat.CompressedRgbaS3tcDxt3Ext;
					factor = 4;
					blocksize = 16;
					break;
				case "DXT5":    // DXT5 compression ratio is 4:1
					format = InternalFormat.CompressedRgbaS3tcDxt5Ext;
					factor = 4;
					blocksize = 16;
					break;
				default:
					compressed = false;
					if( imageData.PixelFormat.ABitMask == 0xf000 && imageData.PixelFormat.RBitMask == 0x0f00 &&
						imageData.PixelFormat.GBitMask == 0x00f0 && imageData.PixelFormat.BBitMask == 0x000f &&
						imageData.PixelFormat.RGBBitCount == 16 )
						format = InternalFormat.Rgba;
					throw new Exception( string.Format( "File compression \"{0}\" is not supported.", imageData.PixelFormat.FourCC ) );
			}

			if( imageData.LinearSize != 0 )
				buffersize = (int) ( ( imageData.MipmapCount > 1 ) ? imageData.LinearSize * factor : imageData.LinearSize );
			else
				buffersize = (int) ( stream.BaseStream.Length - stream.BaseStream.Position );

			// read the pixel data and then pin it to memory so that the garbage collector
			// doesn't shuffle the data around while OpenGL is decompressing it
			byte[] pixels = stream.ReadBytes( buffersize );
			GCHandle pinned = GCHandle.Alloc( pixels, GCHandleType.Pinned );

			try {
				textureTarget = ( imageData.Height == 1 || imageData.Width == 1 ) ? TextureTarget.Texture1d : TextureTarget.Texture2d;
				textureID = Gl.GenTexture();
				Gl.BindTexture( textureTarget, textureID );
				Gl.TexParameter( textureTarget, TextureParameterName.TextureMinFilter, (int) TextureMagFilter.Nearest );
				Gl.TexParameter( textureTarget, TextureParameterName.TextureMagFilter, (int) TextureMinFilter.Nearest );

				int nOffset = 0, nWidth = (int) imageData.Width, nHeight = (int) imageData.Height;

				for( int i = 0; i < ( imageData.MipmapCount == 0 ? 1 : imageData.MipmapCount ); ++i ) {
					if( nWidth == 0 )
						nWidth = 1;        // smallest mipmap is 1x1 pixels
					if( nHeight == 0 )
						nHeight = 1;
					int nSize = 0;

					if( compressed ) {
						nSize = ( ( nWidth + 3 ) / 4 ) * ( ( nHeight + 3 ) / 4 ) * blocksize;
						Gl.CompressedTexImage2D( textureTarget, i, format, nWidth, nHeight, 0, nSize, (IntPtr) ( pinned.AddrOfPinnedObject().ToInt64() + nOffset ) );
					} else {
						nSize = nWidth * nHeight * 4 * ( (int) imageData.PixelFormat.RGBBitCount / 8 );
						Gl.TexImage2D( textureTarget, i, format, nWidth, nHeight, 0, OpenGL.PixelFormat.Bgra, PixelType.UnsignedShort4444, (IntPtr) ( pinned.AddrOfPinnedObject().ToInt64() + nOffset ) );
					}

					nOffset += nSize;
					nWidth /= 2;
					nHeight /= 2;
				}
			} catch( Exception ) {   // There was some sort of Dll related error, or the target GPU does not support glCompressedTexImage2DARB
				throw;
			} finally {
				pinned.Free();
			}
		}
	}

	internal class DDS {
		#region DirectDraw Surface
		/// <summary>The DirectDraw Surface pixel format.</summary>
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public struct DDS_PIXEL_FORMAT {
			/// <summary>Size of the DDS_PIXEL_FORMAT structure.</summary>
			public int Size;
			/// <summary>Pixel format flags.</summary>
			public int Flags;
			/// <summary>The FourCC code for compression identification.</summary>
			public string FourCC { get { return string.Format( "{0}{1}{2}{3}", fourCC0, fourCC1, fourCC2, fourCC3 ); } }
			public char fourCC0;
			public char fourCC1;
			public char fourCC2;
			public char fourCC3;
			/// <summary>The number of bits per pixel.</summary>
			public int RGBBitCount;
			/// <summary>Red bit mask.</summary>
			public int RBitMask;
			/// <summary>Green bit mask.</summary>
			public int GBitMask;
			/// <summary>Blue bit mask.</summary>
			public int BBitMask;
			/// <summary>Alpha bit mask.</summary>
			public int ABitMask;
		}

		/// <summary>The DirectDraw Surface descriptor.</summary>
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public struct DDSURFACEDESC2 {
			/// <summary>The size of the DDSURFACEDESC2 structure.</summary>
			public int Size;
			/// <summary>Flags to determine which fields are valid.</summary>
			public int Flags;
			/// <summary>The height (in pixels) of the surface.</summary>
			public int Height;
			/// <summary>The width (in pixels) of the surface.</summary>
			public int Width;
			/// <summary>The scan line size of the surface.</summary>
			public int LinearSize;
			/// <summary>The depth (if applicable).</summary>
			public int Depth;
			/// <summary>The number of mip map levels in this surface.</summary>
			public int MipmapCount;
			private readonly int Reserved0;
			private readonly int Reserved1;
			private readonly int Reserved2;
			private readonly int Reserved3;
			private readonly int Reserved4;
			private readonly int Reserved5;
			private readonly int Reserved6;
			private readonly int Reserved7;
			private readonly int Reserved8;
			private readonly int Reserved9;
			private readonly int Reserved10;
			/// <summary>A pixel format describing the surface.</summary>
			public DDS_PIXEL_FORMAT PixelFormat;
			/// <summary>DDS surface flags.</summary>
			public int SurfaceFlags;
			/// <summary>DDS cubemap flags.</summary>
			public int CubemapFlags;
			private readonly int Reserved11;
			private readonly int Reserved12;
			private readonly int Reserved13;

			public static DDSURFACEDESC2 FromBinaryReader( BinaryReader stream ) {
				byte[] data = stream.ReadBytes( 124 );    // Marshal.SizeOf(typeof(DDSURFACEDESC2)));
				GCHandle handle = GCHandle.Alloc( data, GCHandleType.Pinned );
				DDSURFACEDESC2 desc = (DDSURFACEDESC2) Marshal.PtrToStructure( handle.AddrOfPinnedObject(), typeof( DDSURFACEDESC2 ) );
				handle.Free();
				return desc;
			}
		}
		#endregion
	}
}
