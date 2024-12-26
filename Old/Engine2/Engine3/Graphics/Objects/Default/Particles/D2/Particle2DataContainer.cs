using Engine.LinearAlgebra;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Engine.Graphics.Objects.Default.Particles.D2 {
	unsafe public class Particle2DataContainer {
		private Particle2Data data;
		public Vector2 Translation {
			get => data.transform.XY;
			set { if( data.transform.XY == value ) return; data.transform.XY = value; Changed?.Invoke(); }
		}

		public float Scale {
			get => data.transform.Z;
			set { if( data.transform.Z == value ) return; data.transform.Z = value; Changed?.Invoke(); }
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

		public Vector4b Color {
			get => data.color;
			set { if( data.color == value ) return; data.color = value; Changed?.Invoke(); }
		}

		public float Blend {
			get => data.blend;
			set { if( data.blend == value ) return; data.blend = value; Changed?.Invoke(); }
		}

		public event Action Changed;

		public Particle2DataContainer( Vector2 initialTranslation, float scale, float rotation, Vector4b color, Vector2 t1, Vector2 t2, float blend ) {
			data = new Particle2Data( initialTranslation, scale, rotation, color, t1, t2, blend );
		}

		public Particle2DataContainer( Vector4b color ) {
			data = new Particle2Data() { color = color };
		}

		public Particle2DataContainer() {
			data = new Particle2Data();
		}

		public void FillByteData( byte[] byteData ) {
			unsafe {
				fixed( Particle2Data* pDataPtr = &data ) {
					Marshal.Copy( (IntPtr) pDataPtr, byteData, 0, 36 );
				}
				Vector2 vec = new Vector2( (float) Math.Cos( Rotation ), (float) Math.Sin( Rotation ) );
				Vector2* rotDataPtr = &vec;
				Marshal.Copy( (IntPtr) rotDataPtr, byteData, 36, 8 );
			}
		}
	}

	[StructLayout( LayoutKind.Explicit, Size = 40 )]
	public struct Particle2Data {
		[FieldOffset( 0 )]
		public Vector3 transform;
		[FieldOffset( 12 )]
		public Vector4 textureData;
		[FieldOffset( 28 )]
		public Vector4b color;
		[FieldOffset( 32 )]
		public float blend;
		[FieldOffset( 36 )]
		public float rotation;

		public Particle2Data( Vector2 initialTranslation, float scale, float rotation, Vector4b color, Vector2 t1, Vector2 t2, float blend ) : this() {
			this.transform = new Vector3( initialTranslation, scale );
			this.color = color;
			this.textureData = new Vector4( t1, t2 );
			this.blend = blend;
			this.rotation = rotation;
		}
	}
}
