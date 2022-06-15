using Engine;
using Engine.Data.Datatypes;
using Engine.Time;
using TestPlatform.Voxels2.Data;

namespace TestPlatform.Voxels2.Render;

public class VoxelChunkFaceRenderer : Identifiable {

	private static float _drawTime = 0;
	private static int _drawCount = 0;
	public static float DrawTime => _drawTime;
	public static int DrawCount => _drawCount;

	public event Action<VoxelChunkFaceRenderer>? NeedRedraw;
	public event Action<VoxelChunkFaceRenderer, IReadOnlyList<VoxelFaceData[]>>? Redrawn;

	private readonly AutoResetEvent _lock;
	private readonly VoxelWorldRenderer _worldRenderer;
	private VoxelChunk _chunk;
	private readonly List<VoxelFaceData>[] _redrawData;
	private readonly VoxelChunk?[] _neighbourChunks;
	private float _lastDrawn;
	public bool FlaggedForRedraw { get; private set; }

	public VoxelChunkFaceRenderer( VoxelWorldRenderer worldRenderer, VoxelChunk chunk ) {
		this._worldRenderer = worldRenderer ?? throw new ArgumentNullException( nameof( worldRenderer ) );
		this._chunk = chunk;
		this._lock = new AutoResetEvent( true );
		this._redrawData = new List<VoxelFaceData>[ 6 ];
		this._neighbourChunks = new VoxelChunk[ 6 ];

		for ( int i = 0; i < this._redrawData.Length; i++ ) {
			this._redrawData[ i ] = new List<VoxelFaceData>();
		}
	}

	public void SetChunk( VoxelChunk chunk ) {
		this._chunk = chunk;
		FlagForRedraw();
	}

	private void FlagForRedraw() {
		if ( this.FlaggedForRedraw )
			return;
		NeedRedraw?.Invoke( this );
		this.FlaggedForRedraw = true;
	}

	public bool Redraw() {
		this._lastDrawn = Clock32.StartupTime;
		if ( this._chunk.IsValid() ) {
			if ( this._chunk.Data.AllAir )
				return true; //Might cause Multithread problems?
			bool valid = true;
			for ( int i = 0; i < 6; i++ ) {
				bool inbounds = this._worldRenderer.World.TryGetChunk( this._chunk.ChunkPosition + GetDirectionVector( (VoxelFaceDirection) i ), true, out VoxelChunk? neighbour );
				if ( inbounds && ( neighbour is null || !neighbour.IsValid() ) ) {
					valid = false;
					break;
				}
				this._neighbourChunks[ i ] = neighbour;
			}
			if ( valid ) {
				try {
					this._lock.WaitOne();
					for ( int i = 0; i < 6; i++ )
						this._redrawData[ i ].Clear();
					float startDrawTime = Clock32.StartupTime;
					Span<uint> chunkLayerIndexMapping = stackalloc uint[ (int) VoxelChunk.Area ];
					Span<ushort> examinedLayer = stackalloc ushort[ (int) VoxelChunk.Area ];
					Span<ushort> occlusionLayer = stackalloc ushort[ (int) VoxelChunk.Area ];
					Span<bool> shouldBeRendered = stackalloc bool[ (int) VoxelChunk.Area ];
					VoxelChunkData data = this._chunk.Data;
					data.AddUser();
					for ( int i = 0; i < 6; i++ ) {
						InternalRedraw( data, i, examinedLayer, occlusionLayer, shouldBeRendered, this._neighbourChunks[ i ] );
					}
					data.RemoveUser();
					_drawTime += Clock32.StartupTime - startDrawTime;
					_drawCount++;
					Redrawn?.Invoke( this, this._redrawData.Select( p => p.ToArray() ).ToList() );
					this.FlaggedForRedraw = false;
				} finally {
					this._lock.Set();
				}
				return true;
			}
		}
		return false;
	}

	private static Vector3i GetDirectionVector( VoxelFaceDirection direction ) => direction switch {
		VoxelFaceDirection.NORTH => (0, 0, 1),
		VoxelFaceDirection.SOUTH => (0, 0, -1),
		VoxelFaceDirection.EAST => (1, 0, 0),
		VoxelFaceDirection.WEST => (-1, 0, 0),
		VoxelFaceDirection.UP => (0, 1, 0),
		VoxelFaceDirection.DOWN => (0, -1, 0),
		_ => 0,
	};

	private void InternalRedraw( VoxelChunkData data, int directionIndex, Span<ushort> examinedLayer, Span<ushort> occlusionLayer, Span<bool> shouldBeRendered, VoxelChunk? neighbour ) {
		List<VoxelFaceData> dataList = this._redrawData[ directionIndex ];
		switch ( (VoxelFaceDirection) directionIndex ) {
			case VoxelFaceDirection.NORTH:
				InternalRedrawZ( dataList, data, neighbour, 1, examinedLayer, occlusionLayer, shouldBeRendered );
				break;
			case VoxelFaceDirection.SOUTH:
				InternalRedrawZ( dataList, data, neighbour, -1, examinedLayer, occlusionLayer, shouldBeRendered );
				break;
			case VoxelFaceDirection.EAST:
				InternalRedrawX( dataList, data, neighbour, 1, examinedLayer, occlusionLayer, shouldBeRendered );
				break;
			case VoxelFaceDirection.WEST:
				InternalRedrawX( dataList, data, neighbour, -1, examinedLayer, occlusionLayer, shouldBeRendered );
				break;
			case VoxelFaceDirection.UP:
				InternalRedrawY( dataList, data, neighbour, 1, examinedLayer, occlusionLayer, shouldBeRendered );
				break;
			case VoxelFaceDirection.DOWN:
				InternalRedrawY( dataList, data, neighbour, -1, examinedLayer, occlusionLayer, shouldBeRendered );
				break;
		}
	}

	private void InternalRedrawX( List<VoxelFaceData> dataList, VoxelChunkData data, VoxelChunk? otherChunk, int direction, Span<ushort> examinedLayer, Span<ushort> occlusionLayer, Span<bool> shouldBeRendered ) {
		for ( int layer = 0; layer < VoxelChunk.Length; layer++ ) {
			Span<uint> chunkLayerIndexMapping = VoxelChunkLodIndexer.GetIndexesX( layer, data.LodLevel );
			{
				bool shouldRender = false;
				for ( int i = 0; i < VoxelChunk.Area; i++ )
					examinedLayer[ i ] = data.GetIdFromIndex( chunkLayerIndexMapping[ i ] );

				for ( int i = 0; i < VoxelChunk.Area; i++ )
					if ( examinedLayer[ i ] != 0 ) {
						shouldRender = true;
						break;
					}
				if ( !shouldRender )
					continue;
			}
			{
				if ( ( direction == -1 && layer == 0 ) || ( direction == 1 && layer == (int) VoxelChunk.Length - 1 ) ) {
					if ( otherChunk is not null ) {
						VoxelChunkData neighbourData = otherChunk.Data;
						neighbourData.AddUser();
						int occlusionDepth = direction == -1 ? (int) VoxelChunk.Length - 1 : 0;
						Span<uint> otherChunkIndexMapping = VoxelChunkLodIndexer.GetIndexesX( occlusionDepth, otherChunk.LodLevel );
						for ( int i = 0; i < VoxelChunk.Area; i++ )
							occlusionLayer[ i ] = neighbourData.GetIdFromIndex( otherChunkIndexMapping[ i ] );
						neighbourData.RemoveUser();
					} else {
						occlusionLayer.Fill( 0 );
					}
				} else {
					Span<uint> occlusionIndexMapping = VoxelChunkLodIndexer.GetIndexesX( layer + direction, data.LodLevel );
					for ( int i = 0; i < VoxelChunk.Area; i++ )
						occlusionLayer[ i ] = data.GetIdFromIndex( occlusionIndexMapping[ i ] );
				}
			}
			InternalLayerDrawEvaluation( dataList, this._chunk.VoxelPosition, layer, (0, 1, 0), (0, 0, 1), (1, 0, 0), (0, 1, 0), (0, 0, 1), examinedLayer, occlusionLayer, shouldBeRendered );
		}
	}

	private void InternalRedrawY( List<VoxelFaceData> dataList, VoxelChunkData data, VoxelChunk? otherChunk, int direction, Span<ushort> examinedLayer, Span<ushort> occlusionLayer, Span<bool> shouldBeRendered ) {
		for ( int layer = 0; layer < VoxelChunk.Length; layer++ ) {
			Span<uint> chunkLayerIndexMapping = VoxelChunkLodIndexer.GetIndexesY( layer, data.LodLevel );
			{
				bool shouldRender = false;
				for ( int i = 0; i < VoxelChunk.Area; i++ )
					examinedLayer[ i ] = data.GetIdFromIndex( chunkLayerIndexMapping[ i ] );

				for ( int i = 0; i < VoxelChunk.Area; i++ )
					if ( examinedLayer[ i ] != 0 ) {
						shouldRender = true;
						break;
					}
				if ( !shouldRender )
					continue;
			}
			{
				if ( ( direction == -1 && layer == 0 ) || ( direction == 1 && layer == (int) VoxelChunk.Length - 1 ) ) {
					if ( otherChunk is not null ) {
						VoxelChunkData neighbourData = otherChunk.Data;
						neighbourData.AddUser();
						int occlusionDepth = direction == -1 ? (int) VoxelChunk.Length - 1 : 0;
						Span<uint> otherChunkIndexMapping = VoxelChunkLodIndexer.GetIndexesY( occlusionDepth, otherChunk.LodLevel );
						for ( int i = 0; i < VoxelChunk.Area; i++ )
							occlusionLayer[ i ] = neighbourData.GetIdFromIndex( otherChunkIndexMapping[ i ] );
						neighbourData.RemoveUser();
					} else {
						occlusionLayer.Fill( 0 );
					}
				} else {
					Span<uint> occlusionIndexMapping = VoxelChunkLodIndexer.GetIndexesY( layer + direction, data.LodLevel );
					for ( int i = 0; i < VoxelChunk.Area; i++ )
						occlusionLayer[ i ] = data.GetIdFromIndex( occlusionIndexMapping[ i ] );
				}
			}
			InternalLayerDrawEvaluation( dataList, this._chunk.VoxelPosition, layer, (1, 0, 0), (0, 0, 1), (0, 1, 0), (1, 0, 0), (0, 0, 1), examinedLayer, occlusionLayer, shouldBeRendered );
		}
	}

	private void InternalRedrawZ( List<VoxelFaceData> dataList, VoxelChunkData data, VoxelChunk? otherChunk, int direction, Span<ushort> examinedLayer, Span<ushort> occlusionLayer, Span<bool> shouldBeRendered ) {
		for ( int layer = 0; layer < VoxelChunk.Length; layer++ ) {
			Span<uint> chunkLayerIndexMapping = VoxelChunkLodIndexer.GetIndexesZ( layer, data.LodLevel );
			{
				bool shouldRender = false;
				for ( int i = 0; i < VoxelChunk.Area; i++ )
					examinedLayer[ i ] = data.GetIdFromIndex( chunkLayerIndexMapping[ i ] );

				for ( int i = 0; i < VoxelChunk.Area; i++ )
					if ( examinedLayer[ i ] != 0 ) {
						shouldRender = true;
						break;
					}
				if ( !shouldRender )
					continue;
			}
			{
				if ( ( direction == -1 && layer == 0 ) || ( direction == 1 && layer == (int) VoxelChunk.Length - 1 ) ) {
					if ( otherChunk is not null ) {
						VoxelChunkData neighbourData = otherChunk.Data;
						neighbourData.AddUser();
						int occlusionDepth = direction == -1 ? (int) VoxelChunk.Length - 1 : 0;
						Span<uint> otherChunkIndexMapping = VoxelChunkLodIndexer.GetIndexesZ( occlusionDepth, otherChunk.LodLevel );
						for ( int i = 0; i < VoxelChunk.Area; i++ )
							occlusionLayer[ i ] = neighbourData.GetIdFromIndex( otherChunkIndexMapping[ i ] );
						neighbourData.RemoveUser();
					} else {
						occlusionLayer.Fill( 0 );
					}
				} else {
					Span<uint> occlusionIndexMapping = VoxelChunkLodIndexer.GetIndexesZ( layer + direction, data.LodLevel );
					for ( int i = 0; i < VoxelChunk.Area; i++ )
						occlusionLayer[ i ] = data.GetIdFromIndex( occlusionIndexMapping[ i ] );
				}
			}
			InternalLayerDrawEvaluation( dataList, this._chunk.VoxelPosition, layer, (1, 0, 0), (0, 1, 0), (0, 0, 1), (1, 0, 0), (0, 1, 0), examinedLayer, occlusionLayer, shouldBeRendered );
		}
	}

	private static void InternalLayerDrawEvaluation(
		List<VoxelFaceData> dataList, Vector3i baseTranslation, int layer,
		Vector3i xTAxis, Vector3i yTAxis, Vector3i zTAxis, Vector3i xSAxis, Vector3i ySAxis,
		Span<ushort> examinedLayer, Span<ushort> occlusionLayer, Span<bool> shouldBeRendered ) {

		static int GetIndex( int x, int y ) => ( y * (int) VoxelChunk.Length ) + x;
		for ( int i = 0; i < VoxelChunk.Area; i++ )
			shouldBeRendered[ i ] = examinedLayer[ i ] != 0 && occlusionLayer[ i ] == 0;

		for ( int sY = 0; sY < VoxelChunk.Length; sY++ ) {
			for ( int sX = 0; sX < VoxelChunk.Length; sX++ ) {
				int index = GetIndex( sX, sY );
				if ( !shouldBeRendered[ index ] )
					continue;

				ushort voxelId = examinedLayer[ index ];

				int eX = sX + 1;
				for ( ; eX < VoxelChunk.Length; eX++ ) {
					int indexEx = GetIndex( eX, sY );
					if ( !shouldBeRendered[ indexEx ] )
						break;
					if ( examinedLayer[ indexEx ] != voxelId )
						break;
				}

				int eY = sY + 1;
				for ( ; eY < VoxelChunk.Length; eY++ ) {
					for ( int x = sX; x < eX; x++ ) {
						int indexEx = GetIndex( x, eY );
						if ( !shouldBeRendered[ indexEx ] )
							goto endSearch;
						if ( examinedLayer[ indexEx ] != voxelId )
							goto endSearch;
					}
				}
			endSearch:
				for ( int y = sY; y < eY; y++ )
					for ( int x = sX; x < eX; x++ )
						shouldBeRendered[ GetIndex( x, y ) ] = false;

				int scaleX = eX - sX - 1;
				int scaleY = eY - sY - 1;

				Vector3i translation = baseTranslation;
				translation.X += ( xTAxis.X * sX ) + ( yTAxis.X * sY ) + ( zTAxis.X * layer );
				translation.Y += ( xTAxis.Y * sX ) + ( yTAxis.Y * sY ) + ( zTAxis.Y * layer );
				translation.Z += ( xTAxis.Z * sX ) + ( yTAxis.Z * sY ) + ( zTAxis.Z * layer );
				Vector3i scale = 0;
				scale.X = ( xSAxis.X * scaleX ) + ( ySAxis.X * scaleY );
				scale.Y = ( xSAxis.Y * scaleX ) + ( ySAxis.Y * scaleY );
				scale.Z = ( xSAxis.Z * scaleX ) + ( ySAxis.Z * scaleY );
				VoxelFaceData face = new( translation, scale, voxelId );

				dataList.Add( face );
			}
		}
	}

	public bool IsValid() => this._chunk.IsValid();
}
