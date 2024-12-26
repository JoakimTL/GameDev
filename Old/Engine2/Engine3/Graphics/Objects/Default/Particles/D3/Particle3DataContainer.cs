using Engine.LinearAlgebra;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Engine.Graphics.Objects.Default.Particles.D3 {
	unsafe public class Particle3DataContainer {
		private Particle3Data data;
		public Vector3 Translation {
			get => data.transform.XYZ;
			set { if( data.transform.XYZ == value ) return; data.transform.XYZ = value; Changed?.Invoke(); }
		}

		public float Scale {
			get => data.transform.W;
			set { if( data.transform.W == value ) return; data.transform.W = value; Changed?.Invoke(); }
		}

		public float Rotation {
			get => data.rotation;
			set { if( data.rotation == value ) return; data.rotation = value; Changed?.Invoke(); }
		}

		public Vector2 TextureOffset1 {
			get => data.textureData.XY;
			set { if( data.textureData.XY == value ) return; data.textureData.XY = value; Changed?.Invoke(); }
		}

		public Vector2 TextureOffset2 {
			get => data.textureData.ZW;
			set { if( data.textureData.ZW == value ) return; data.textureData.ZW = value; Changed?.Invoke(); }
		}

		public Vector4b Diffuse {
			get => data.diffuse;
			set { if( data.diffuse == value ) return; data.diffuse = value; Changed?.Invoke(); }
		}

		public Vector4b Glow {
			get => data.glow;
			set { if( data.glow == value ) return; data.glow = value; Changed?.Invoke(); }
		}

		public float Blend {
			get => data.blend;
			set { if( data.blend == value ) return; data.blend = value; Changed?.Invoke(); }
		}

		public event Action Changed;

		public Particle3DataContainer( Vector3 initialTranslation, float scale, float rotation, Vector4b diffuse, Vector4b glow, Vector2 t1, Vector2 t2, float blend ) {
			data = new Particle3Data( initialTranslation, scale, rotation, diffuse, glow, t1, t2, blend );
		}

		public Particle3DataContainer( Vector4b color ) {
			data = new Particle3Data() { diffuse = color };
		}

		public Particle3DataContainer() {
			data = new Particle3Data();
		}

		public void FillByteData( float lifetime,  byte[] byteData ) {
			unsafe {
				fixed( Particle3Data* pDataPtr = &data ) {
					Marshal.Copy( (IntPtr) pDataPtr, byteData, 0, 44 );
				}
				Vector2 vec = new Vector2( (float) Math.Cos( Rotation ), (float) Math.Sin( Rotation ) );
				Vector2* rotDataPtr = &vec;
				Marshal.Copy( (IntPtr) rotDataPtr, byteData, 44, 8 );
			}
		}
	}

	[StructLayout( LayoutKind.Explicit, Size = 48 )]
	public struct Particle3Data {
		[FieldOffset( 0 )]
		public Vector4 transform;
		[FieldOffset( 16 )]
		public Vector4 textureData;
		[FieldOffset( 32 )]
		public Vector4b diffuse;
		[FieldOffset( 36 )]
		public Vector4b glow;
		[FieldOffset( 40 )]
		public float blend;
		[FieldOffset( 44 )]
		public float rotation;

		public Particle3Data( Vector3 initialTranslation, float scale, float rotation, Vector4b diffuse, Vector4b glow, Vector2 t1, Vector2 t2, float blend ) : this() {
			this.transform = new Vector4( initialTranslation, scale );
			this.diffuse = diffuse;
			this.glow = glow;
			this.textureData = new Vector4( t1, t2 );
			this.blend = blend;
			this.rotation = rotation;
		}
	}
}
