using System.Runtime.InteropServices;
using Engine.Data.Datatypes;
using TestPlatform.Voxels2.Data;

namespace TestPlatform.Voxels2.Render;
public static unsafe class VoxelChunkLodIndexer {

	private static readonly List<IntPtr[]> _indexes;

	static VoxelChunkLodIndexer() {
		_indexes = new List<IntPtr[]>();

		uint voxelSize = 0;
		uint lodLevel = 0;
		while ( voxelSize < VoxelChunk.Length ) {
			voxelSize = GetVoxelSizeAtLodLevel( lodLevel );
			_indexes.Add( CreateLodLevel( voxelSize ) );
			++lodLevel;
		}
	}

	public static uint GetVoxelSizeAtLodLevel( uint lodLevel ) => Math.Min( 1u << (int) lodLevel, VoxelChunk.Length );

	private static IntPtr[] CreateLodLevel( uint voxelSize ) {
		Span<(Vector3i xAxis, Vector3i yAxis, Vector3i zAxis)> axis = stackalloc (Vector3i, Vector3i, Vector3i)[ 3 ];
		axis[ 0 ] = ((0, 1, 0), (0, 0, 1), (1, 0, 0));
		axis[ 1 ] = ((1, 0, 0), (0, 0, 1), (0, 1, 0));
		axis[ 2 ] = ((1, 0, 0), (0, 1, 0), (0, 0, 1));

		uint* xAxis = (uint*) NativeMemory.AllocZeroed( VoxelChunk.ChunkVolume * sizeof( uint ) );
		uint* yAxis = (uint*) NativeMemory.AllocZeroed( VoxelChunk.ChunkVolume * sizeof( uint ) );
		uint* zAxis = (uint*) NativeMemory.AllocZeroed( VoxelChunk.ChunkVolume * sizeof( uint ) );

		for ( int i = 0; i < VoxelChunk.Length; i++ ) {
			GiveLayerIndexes( xAxis, (int) VoxelChunk.Area * i, axis[ 0 ].xAxis, axis[ 0 ].yAxis, axis[ 0 ].zAxis, voxelSize, i );
			GiveLayerIndexes( yAxis, (int) VoxelChunk.Area * i, axis[ 1 ].xAxis, axis[ 1 ].yAxis, axis[ 1 ].zAxis, voxelSize, i );
			GiveLayerIndexes( zAxis, (int) VoxelChunk.Area * i, axis[ 2 ].xAxis, axis[ 2 ].yAxis, axis[ 2 ].zAxis, voxelSize, i );
		}

		return new IntPtr[] { new IntPtr( xAxis ), new IntPtr( yAxis ), new IntPtr( zAxis ) };
	}

	internal static void GiveLayerIndexes( uint* data, int index, Vector3i xAxis, Vector3i yAxis, Vector3i zAxis, uint voxelSize, int layer ) {
		for ( int b = 0; b < VoxelChunk.Length; b++ )
			for ( int a = 0; a < VoxelChunk.Length; a++ )
				data[ index++ ] = GetLocalVoxelIndex( ( a * xAxis ) + ( b * yAxis ) + (layer * zAxis), voxelSize );
	}

	public static uint GetLocalVoxelIndex( Vector3i localVoxelPosition, uint voxelSize ) {
		uint actualLength = VoxelChunk.Length / voxelSize;
		uint actualArea = actualLength * actualLength;
		Vector3i internalPosition = Vector3i.Floor( localVoxelPosition / voxelSize );
		return (uint) ( ( internalPosition.Y * (int) actualArea ) + ( internalPosition.Z * (int) actualLength ) + internalPosition.X );
	}

	public static Span<uint> GetIndexesX( int layer, uint lodLevel ) {
		if ( lodLevel >= _indexes.Count )
			lodLevel = (uint) ( _indexes.Count - 1 );
		return new( ( (uint*) _indexes[ (int) lodLevel ][ 0 ].ToPointer() ) + ( VoxelChunk.Area * layer ), (int) VoxelChunk.Area );
	}

	public static Span<uint> GetIndexesY( int layer, uint lodLevel ) {
		if ( lodLevel >= _indexes.Count )
			lodLevel = (uint) ( _indexes.Count - 1 );
		return new( ( (uint*) _indexes[ (int) lodLevel ][ 1 ].ToPointer() ) + ( VoxelChunk.Area * layer ), (int) VoxelChunk.Area );
	}

	public static Span<uint> GetIndexesZ( int layer, uint lodLevel ) {
		if ( lodLevel >= _indexes.Count )
			lodLevel = (uint) ( _indexes.Count - 1 );
		return new( ( (uint*) _indexes[ (int) lodLevel ][ 2 ].ToPointer() ) + ( VoxelChunk.Area * layer ), (int) VoxelChunk.Area );
	}
}
