using Engine;
using Engine.Rendering.Standard.Scenes;

namespace TestPlatform.Voxels2.Render;

public class VoxelRenderDataManager : DisposableIdentifiable{

	private readonly VoxelFaceDirection _faceDirection;
	private readonly Scene _scene;
	private readonly List<VoxelSceneObject> _sceneObjects;
	//Contains render data from multiple chunks, the best candidate for maintaining storage location of chunk render data might be the chunks.

	public VoxelRenderDataManager( VoxelFaceDirection faceDirection, Scene scene ) {
		this._sceneObjects = new List<VoxelSceneObject>();
		this._faceDirection = faceDirection;
		this._scene = scene;
	}

	public void Clear( VoxelChunkFaceRenderer renderer ) {
		foreach ( VoxelSceneObject sceneObject in this._sceneObjects )
			sceneObject.Clear( renderer );
	}

	public void Fill( VoxelChunkFaceRenderer renderer, VoxelFaceData[] data ) {
		uint start = 0;
		foreach ( VoxelSceneObject sceneObject in this._sceneObjects ) {
			if ( sceneObject.HasCapacity )
				start += sceneObject.Add( renderer, start, data );
			if ( start == data.Length )
				return;
		}
		while ( start < data.Length )
			start += CreateSceneObject().Add( renderer, start, data );
	}


	private VoxelSceneObject CreateSceneObject() {
		VoxelSceneObject? so = new( this._faceDirection );
		this._sceneObjects.Add( so );
		this._scene.AddSceneObject( so );
		return so;
	}

	protected override bool OnDispose() {
		for ( int i = 0; i < this._sceneObjects.Count; i++ ) {
			this._sceneObjects[ i ].Dispose();
		}
		this._sceneObjects.Clear();
		return true;
	}
}
