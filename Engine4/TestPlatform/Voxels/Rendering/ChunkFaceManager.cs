using Engine;
using Engine.Rendering.Standard;
using TestPlatform.Voxels.World;

namespace TestPlatform.Voxels.Rendering;

public class ChunkFaceManager : Identifiable {

	/*private uint _oldFaceCount;
	private readonly Box<uint>?[] _faceIndexes;
	private readonly List<Box<uint>>[] _facesByLayer;*/
	private uint _faceCount;
	//private readonly uint[] _indices;
	private readonly List<LayerIndexer> _layers;
	private readonly uint?[] _layerIndex;
	public readonly SceneInstanceData<VoxelFaceData> VoxelData;

	public uint FaceCount => this._faceCount;
	public bool HasSpace => this.FaceCount < ChunkRender.InstanceCount;

	public ChunkFaceManager( string name ) : base( name ) {
		//this._faceIndexes = new Box<uint>[ ChunkRender.InstanceCount ];
		//this._facesByLayer = new List<Box<uint>>[ VoxelChunk.SideLength ];
		this._faceCount = 0;
		//this._indices = new uint[ ChunkRender.InstanceCount ];
		this._layers = new List<LayerIndexer>();
		this._layerIndex = new uint?[ ChunkRender.InstanceCount ];
		//for ( int i = 0; i < VoxelChunk.SideLength; i++ )
		//	this._facesByLayer[ i ] = new List<Box<uint>>();
		this.VoxelData = new SceneInstanceData<VoxelFaceData>( ChunkRender.InstanceCount, 0 );
	}

	public void SetActiveInstances() => this.VoxelData.SetActiveInstances( this._faceCount );

	public void Add( VoxelFaceData face, uint layer ) {
		if ( this._faceCount >= ChunkRender.InstanceCount ) {
			this.LogWarning( "Exceeded instance capacity!" );
			return;
		}
		if ( layer > VoxelChunk.SideLength ) {
			this.LogWarning( $"Layer {layer} is out of bounds." );
			return;
		}
		uint index = this._faceCount;
		this.VoxelData.SetInstance( index, face );

		if ( !this._layerIndex[ layer ].HasValue ) {
			this._layerIndex[ layer ] = (uint) this._layers.Count;
			this._layers.Add( new( layer, index ) );
		}

		this._faceCount++;
		SetActiveInstances();
	}


	public void Clear( uint layer ) {
		if ( layer > VoxelChunk.SideLength ) {
			this.LogWarning( $"Layer {layer} is out of bounds." );
			return;
		}
		uint? layerIndex = this._layerIndex[ layer ];
		if ( !layerIndex.HasValue )
			return;
		LayerIndexer startLayer = this._layers[ (int) layerIndex.Value ];
		LayerIndexer endLayer = (int) layerIndex.Value == this._layers.Count - 1 ? new LayerIndexer( VoxelChunk.SideLength, this._faceCount ) : this._layers[ (int) layerIndex.Value + 1 ];
		if ( endLayer.StartIndex == startLayer.StartIndex ) {
			this.LogWarning( $"Layer {layer} has no faces, but still exists." );
			return;
		}
		uint length = endLayer.StartIndex - startLayer.StartIndex;
		this.VoxelData.Remove( startLayer.StartIndex, length );

		this._layers.RemoveAt( (int) layerIndex.Value );
		this._layerIndex[ layer ] = null;
		for ( uint i = layerIndex.Value; i < this._layers.Count; i++ ) {
			LayerIndexer otherLayer = this._layers[ (int) i ];
			this._layers[ (int) i ] = new LayerIndexer( otherLayer.Layer, otherLayer.StartIndex - length );
		}
		for ( uint i = 0; i < VoxelChunk.SideLength; i++ ) {
			uint? otherLayerIndex = this._layerIndex[ i ];
			if ( otherLayerIndex.HasValue && otherLayerIndex.Value > layerIndex.Value )
				this._layerIndex[ i ] -= 1;
		}

		this._faceCount -= length;
		SetActiveInstances();
	}

	public void AddSimple( VoxelFaceData face ) {
		if ( this._faceCount >= ChunkRender.InstanceCount ) {
			this.LogWarning( "Exceeded instance capacity!" );
			return;
		}
		this.VoxelData.SetInstance( this._faceCount, face );
		this._faceCount++;
		SetActiveInstances();
	}

	public void ClearAll() {
		this._faceCount = 0;
		SetActiveInstances();
	}

	private struct LayerIndexer {
		public readonly uint Layer;
		public readonly uint StartIndex;

		public LayerIndexer( uint layer, uint startIndex ) {
			this.Layer = layer;
			this.StartIndex = startIndex;
		}

		public LayerIndexer SetIndex( uint index ) => new LayerIndexer( this.Layer, index );
	}

	/*public void AddOld( VoxelFaceData face, uint layer ) {
		if ( this._oldFaceCount >= ChunkRender.InstanceCount ) {
			this.LogWarning( "Exceeded instance capacity!" );
			return;
		}
		List<Box<uint>>? faces = this._facesByLayer[ layer ];
		if ( faces is null ) {
			this.LogWarning( $"Couldn't find layer {layer}." );
			return;
		}
		Box<uint> index = new( this._oldFaceCount );
		this.VoxelData.SetInstance( index.Value, face );
		faces.Add( index );
		this._faceIndexes[ this._oldFaceCount ] = index;
		this._oldFaceCount++;
		SetActiveInstances();
	}

	public void ClearOld( uint layer ) {
		List<Box<uint>>? faces = this._facesByLayer[ layer ];
		if ( faces is null ) {
			this.LogWarning( $"Couldn't find layer {layer}." );
			return;
		}
		if ( faces.Count == 0 )
			return;
		if ( IsContiguous( faces ) ) {
			this.VoxelData.Remove( faces[ 0 ].Value, (uint) faces.Count );
			for ( uint j = faces[ 0 ].Value; j < this._oldFaceCount; j++ ) {
				Box<uint>? forward = this._faceIndexes[ j + faces.Count ];
				if ( forward is null ) {
					this._faceIndexes[ j ] = null;
					continue;
				}
				this._faceIndexes[ j ] = forward;
				forward.Value = j;
			}
			this._oldFaceCount -= (uint) faces.Count;
		} else {
			for ( int i = faces.Count - 1; i >= 0; i-- ) {
				this.VoxelData.Remove( faces[ i ].Value, 1 );
				for ( uint j = faces[ i ].Value; j < this._oldFaceCount; j++ ) {
					Box<uint>? forward = this._faceIndexes[ j + 1 ];
					if ( forward is null ) {
						this._faceIndexes[ j ] = null;
						continue;
					}
					this._faceIndexes[ j ] = forward;
					forward.Value = j;
				}
				this._oldFaceCount--;
			}
		}
		SetActiveInstances();
		faces.Clear();
	}

	public static bool IsContiguous( List<Box<uint>> faces ) {
		for ( int i = 1; i < faces.Count; i++ )
			if ( faces[ i ].Value - faces[ i - 1 ].Value != 1 )
				return false;
		return true;
	}
	*/
}
