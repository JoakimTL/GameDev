using Engine;
using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VoxDevClient.Rendering.Voxels {
	public class VoxelChunkMesh : InstancedMeshVBOArray {

		public const int INSTANCESIZE = 8; // 3 bytes for translation, 3 byte for scaling, 2 for UV

		public enum Direction {
			XPLUS, ZPLUS, XNEG, ZNEG, YPLUS, YNEG
		}

		public static readonly IReadOnlyList<Vector3i> DirectionVectors = new List<Vector3i>() { (1, 0, 0), (0, 0, 1), (-1, 0, 0), (0, 0, -1), (0, 1, 0), (0, -1, 0) };

		private byte[] clearData = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

		public Direction NormalDirection { get; private set; }
		private int min = 0;
		private int currentSize;
		private int maxFaces;
		private bool[] filled;

		public VoxelChunkMesh( string name, Direction direction, int maxFaces ) : base( name, ( 4 * ( 3 + 3 + 2 ) ), maxFaces * INSTANCESIZE / 256, BufferUsage.DynamicDraw, PrimitiveType.TriangleStrip ) {
			this.NormalDirection = direction;
			currentSize = maxFaces / 256;
			this.maxFaces = maxFaces;
			LoadMesh( GenerateVBOMesh(), 8 );
			filled = new bool[ currentSize ];
			Setup( VAO );
			SetActiveInstances( 0 );
		}

		public void Clear( int index ) {
			if( index < 0 )
				return;
			if( index < min )
				min = index;
			if( index == ActiveInstances - 1 )
				SetActiveInstances( index );
			filled[ index ] = false;
			BufferData( index * INSTANCESIZE, clearData );
		}

		public int FillSingle( Vector3b translation, Vector3b scale, Vector2b UV ) {
			if( min == -1 )
				if( !Expand() )
					return -1;
			int index = min;
			filled[ index ] = true;
			min = FindNextClear( min );
			if( index >= ActiveInstances )
				SetActiveInstances( index + 1 );
			BufferData( index * INSTANCESIZE, new byte[] { translation.X, translation.Y, translation.Z, scale.X, scale.Y, scale.Z, UV.X, UV.Y } );
			return index;
		}

		public int FillMultiple( params VoxFace[] faces ) {
			int index = FindRangeStart( faces.Length );
			if( index == -1 )
				return -1;
			for( int i = 0; i < faces.Length; i++ ) {
				filled[ index + i ] = true;
			}
			min = FindNextClear( 0 );
			byte[] arr = new byte[ faces.Length * INSTANCESIZE ];
			unsafe {
				fixed( VoxFace* p = faces ) {
					byte* bp = (byte*) p;
					for( int i = 0; i < faces.Length * INSTANCESIZE; i++ ) {
						arr[ i ] = bp[ i ];
					}
				}
			}
			if( index + faces.Length > ActiveInstances )
				SetActiveInstances( index + faces.Length );
			BufferData( index * INSTANCESIZE, arr );
			return index;
		}

		internal int FindRangeStart( int length ) {
			while( min == -1 || min + length > currentSize ) {
				if( !Expand() )
					return -1;
			}
			int i = min;
			for( ; i < currentSize - length; i++ ) {
				int j = 0;
				for( ; j < length; j++ ) {
					if( filled[ i + j ] ) {
						i = i + j + 1;
						break;
					}
				}
				if( j == length )
					break;
			}
			if( i >= currentSize - length )
				return -1;
			return i;
		}

		private int FindNextClear( int from ) {
			for( int i = from; i < currentSize; i++ )
				if( !filled[ i ] )
					return i;
			return -1;
		}

		private bool Expand() {
			if( currentSize >= maxFaces )
				return false;
			currentSize *= 2;
			Array.Resize( ref filled, currentSize );
			min = FindNextClear( 0 );
			Buffers[ DBO ].Resize( (uint) ( currentSize * INSTANCESIZE ) );
			return true;
		}

		public override void Setup( uint VAO ) {
			if( VAO > 0 )
				Gl.BindVertexArray( VAO );
			Gl.EnableVertexAttribArray( 0 );
			Gl.EnableVertexAttribArray( 1 );
			Gl.EnableVertexAttribArray( 2 );
			Gl.EnableVertexAttribArray( 3 );
			Gl.EnableVertexAttribArray( 4 );
			Gl.EnableVertexAttribArray( 5 );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			Gl.VertexAttribPointer( 0, 3, VertexAttribType.UnsignedByte, false, 8, (IntPtr) 0 );
			Gl.VertexAttribPointer( 1, 3, VertexAttribType.Byte, false, 8, (IntPtr) 3 );
			Gl.VertexAttribPointer( 2, 2, VertexAttribType.UnsignedByte, false, 8, (IntPtr) 6 );
			Gl.BindBuffer( BufferTarget.ArrayBuffer, DBO );
			Gl.VertexAttribPointer( 3, 3, VertexAttribType.UnsignedByte, false, INSTANCESIZE, (IntPtr) 0 );
			Gl.VertexAttribPointer( 4, 3, VertexAttribType.UnsignedByte, false, INSTANCESIZE, (IntPtr) 3 );
			Gl.VertexAttribPointer( 5, 2, VertexAttribType.UnsignedByte, true, INSTANCESIZE, (IntPtr) 6 );
			Gl.VertexAttribDivisor( 3, 1 );
			Gl.VertexAttribDivisor( 4, 1 );
			Gl.VertexAttribDivisor( 5, 1 );
			if( VAO > 0 )
				Gl.BindVertexArray( 0 );
		}

		private byte[] GenerateVBOMesh() {
			Vector3b[] translationData = new Vector3b[ 4 ];
			Vector2b[] UVData = new Vector2b[ 4 ];
			Vector3b normal = DirectionVectors[ (int) NormalDirection ].AsByteUnchecked;

			switch( NormalDirection ) {
				case Direction.XPLUS:
					translationData[ 0 ] = new Vector3b( 0, 0, 1 );
					translationData[ 1 ] = new Vector3b( 0, 0, 0 );
					translationData[ 2 ] = new Vector3b( 0, 1, 1 );
					translationData[ 3 ] = new Vector3b( 0, 1, 0 );
					UVData[ 0 ] = new Vector2b( 0, 1 );
					UVData[ 1 ] = new Vector2b( 0, 0 );
					UVData[ 2 ] = new Vector2b( 1, 1 );
					UVData[ 3 ] = new Vector2b( 1, 0 );
					break;
				case Direction.ZPLUS:
					translationData[ 0 ] = new Vector3b( 0, 1, 0 );
					translationData[ 1 ] = new Vector3b( 0, 0, 0 );
					translationData[ 2 ] = new Vector3b( 1, 1, 0 );
					translationData[ 3 ] = new Vector3b( 1, 0, 0 );
					UVData[ 0 ] = new Vector2b( 0, 1 );
					UVData[ 1 ] = new Vector2b( 0, 0 );
					UVData[ 2 ] = new Vector2b( 1, 1 );
					UVData[ 3 ] = new Vector2b( 1, 0 );
					break;
				case Direction.XNEG:
					translationData[ 0 ] = new Vector3b( 0, 1, 0 );
					translationData[ 1 ] = new Vector3b( 0, 0, 0 );
					translationData[ 2 ] = new Vector3b( 0, 1, 1 );
					translationData[ 3 ] = new Vector3b( 0, 0, 1 );
					UVData[ 0 ] = new Vector2b( 1, 0 );
					UVData[ 1 ] = new Vector2b( 0, 0 );
					UVData[ 2 ] = new Vector2b( 1, 1 );
					UVData[ 3 ] = new Vector2b( 0, 1 );
					break;
				case Direction.ZNEG:
					translationData[ 0 ] = new Vector3b( 1, 0, 0 );
					translationData[ 1 ] = new Vector3b( 0, 0, 0 );
					translationData[ 2 ] = new Vector3b( 1, 1, 0 );
					translationData[ 3 ] = new Vector3b( 0, 1, 0 );
					UVData[ 0 ] = new Vector2b( 1, 0 );
					UVData[ 1 ] = new Vector2b( 0, 0 );
					UVData[ 2 ] = new Vector2b( 1, 1 );
					UVData[ 3 ] = new Vector2b( 0, 1 );
					break;
				case Direction.YPLUS:
					translationData[ 0 ] = new Vector3b( 1, 0, 0 );
					translationData[ 1 ] = new Vector3b( 0, 0, 0 );
					translationData[ 2 ] = new Vector3b( 1, 0, 1 );
					translationData[ 3 ] = new Vector3b( 0, 0, 1 );
					UVData[ 0 ] = new Vector2b( 1, 0 );
					UVData[ 1 ] = new Vector2b( 0, 0 );
					UVData[ 2 ] = new Vector2b( 1, 1 );
					UVData[ 3 ] = new Vector2b( 0, 1 );
					break;
				case Direction.YNEG:
					translationData[ 0 ] = new Vector3b( 0, 0, 1 );
					translationData[ 1 ] = new Vector3b( 0, 0, 0 );
					translationData[ 2 ] = new Vector3b( 1, 0, 1 );
					translationData[ 3 ] = new Vector3b( 1, 0, 0 );
					UVData[ 0 ] = new Vector2b( 0, 1 );
					UVData[ 1 ] = new Vector2b( 0, 0 );
					UVData[ 2 ] = new Vector2b( 1, 1 );
					UVData[ 3 ] = new Vector2b( 1, 0 );
					break;
			}
			byte[] arr = new byte[ 32 ];
			for( int i = 0; i < 4; i++ ) {
				arr[ i * 8 + 0 ] = translationData[ i ].X;
				arr[ i * 8 + 1 ] = translationData[ i ].Y;
				arr[ i * 8 + 2 ] = translationData[ i ].Z;
				arr[ i * 8 + 3 ] = normal.X;
				arr[ i * 8 + 4 ] = normal.Y;
				arr[ i * 8 + 5 ] = normal.Z;
				arr[ i * 8 + 6 ] = UVData[ i ].X;
				arr[ i * 8 + 7 ] = UVData[ i ].Y;
			}
			return arr;
		}
	}

	[StructLayout( LayoutKind.Explicit )]
	public struct VoxFace {
		[FieldOffset( 0 )]
		public Vector3b translation;
		[FieldOffset( 3 )]
		public Vector3b scale;
		[FieldOffset( 6 )]
		public Vector2b uv;
	}
}
