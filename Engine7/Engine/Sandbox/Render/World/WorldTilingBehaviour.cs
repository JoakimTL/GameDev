using Engine;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;
public sealed class WorldTilingBehaviour : SynchronizedRenderBehaviourBase<WorldArchetype> {

	private SceneInstanceCollection<Vertex3, Entity2SceneData>? _sceneInstanceCollection;
	//private OcTree<TileTriangle, float> _octree;
	private Vector3<float> _lastCameraTranslation;
	private readonly List<WorldTileSceneInstance> _instances = [];

	protected override void OnRenderEntitySet() {
		if (_sceneInstanceCollection is null)
			_sceneInstanceCollection = RenderEntity.RequestSceneInstanceCollection<Vertex3, Entity2SceneData, TestShaderBundle>( "test", 0 );
		var baseTiles = Archetype.WorldTilingComponent.Tiling.Tiles;
		for (int i = 0; i < baseTiles.Count; i++) {
			WorldTileSceneInstance instance = _sceneInstanceCollection.Create<WorldTileSceneInstance>();
			instance.SetBaseTile( baseTiles[ i ] );
			instance.Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity ) );
			_instances.Add( instance );
		}
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		Vector3<float> currentCameraTranslation = RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation.Round<Vector3<float>, float>( 2, MidpointRounding.ToEven );
		if (_lastCameraTranslation == currentCameraTranslation)
			return;
		_lastCameraTranslation = currentCameraTranslation;

		var vertices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.Vertices;
		for (int i = 0; i < _instances.Count; i++) {
			List<TileTriangle> trianglesInLoD = [];
			var instance = _instances[ i ];
			var center = Vector.Average<Vector3<double>, double>( [
				vertices[ instance.BaseTile.IndexA ],
				vertices[ instance.BaseTile.IndexB ],
				vertices[ instance.BaseTile.IndexC ] ] ).CastSaturating<double, float>();
			int seekingLayer = GetLoDLevel( (center - _lastCameraTranslation).Magnitude<Vector3<float>, float>(), instance.BaseTile.Layer, instance.BaseTile.Layer + instance.BaseTile.RemainingLayers );
			if (int.Clamp( seekingLayer, (int) instance.BaseTile.Layer, (int) (instance.BaseTile.Layer + instance.BaseTile.RemainingLayers) ) == instance.CurrentLoD)
				continue;
			instance.CurrentLoD = int.Clamp( seekingLayer, (int) instance.BaseTile.Layer, (int) (instance.BaseTile.Layer + instance.BaseTile.RemainingLayers) );
			instance.Mesh?.Dispose();
			AddTilesToList( instance.BaseTile, trianglesInLoD, seekingLayer );
			IMesh mesh = CreateLoDMesh( trianglesInLoD, vertices );
			instance.SetMesh( mesh );
			instance.Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity ) );
		}
	}

	private int GetLoDLevel( float distance, uint minLayer, uint maxLayer ) {
		//Min layer is the lowest level of detail, max layer is the highest level of detail.
		int layerDifference = (int) (maxLayer - minLayer);
		float distanceToLayer = distance / 2;
		float distancePart = distanceToLayer * layerDifference;
		float layer = maxLayer - distancePart;
		return int.Min( (int) float.Round( layer ), (int) maxLayer );
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		return false;
	}

	protected override void Synchronize() {
		//if (_sceneInstanceCollection is null)
		//	_sceneInstanceCollection = RenderEntity.RequestSceneInstanceCollection<Vertex3, Entity2SceneData, TestShaderBundle>( "test", 0 );
		//if (_octree is null)
		//	_octree = new OcTree<WorldTileTriangle, float>( 3 );

		//for (int i = 0; i < Archetype.WorldTilingComponent.Tiling.Tiles.Count; i++) {
		//	uint level = (uint) (Archetype.WorldTilingComponent.Tiling.Tiles.Count - 1 - i);
		//	if (level > 4)
		//		continue;
		//	foreach (Tile tile in Archetype.WorldTilingComponent.Tiling.Tiles[ i ])
		//		_octree.Add( new WorldTileTriangle( tile.IndexA, tile.IndexB, tile.IndexC, Archetype.WorldTilingComponent.Tiling.TileVectors ) );
		//}

		////TODO: find some way to do this shit.
		//List<List<AABB<Vector3<float>>>> boundsPerLevel = [];

		//for (int i = 0; i < Archetype.WorldTilingComponent.Tiling.Tiles.Count; i++) {
		//	_octree.GetBoundsAtLevel( out var boundAtLevel, (uint) i );
		//	boundsPerLevel.Add( boundAtLevel );

		//	//foreach (AABB<Vector3<float>> bounds in boundAtLevel) {
		//	//	WorldTileSceneInstance instance = _sceneInstanceCollection.Create<WorldTileSceneInstance>();
		//	//	List<WorldTileTriangle> trianglesInLoD = [];
		//	//	_octree.GetAll( bounds, trianglesInLoD, (uint) i );
		//	//	instance.SetMesh( CreateLoDMesh( trianglesInLoD, Archetype.WorldTilingComponent.Tiling.TileVectors ) );

		//	//	instance.Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity ) );
		//	//}
		//}

		//Assign LoD level based on distance from camera. If an LoD level changes update the mesh of the affected cell and neighboring cells.

		//uint currentLevel = 4;
		//foreach (AABB<Vector3<float>> bounds in boundsPerLevel[ (int) currentLevel ]) {
		//	WorldTileSceneInstance instance = _sceneInstanceCollection.Create<WorldTileSceneInstance>();
		//	List<WorldTileTriangle> trianglesInLoD = [];
		//	_octree.GetAll( bounds, trianglesInLoD );
		//	instance.SetMesh( CreateLoDMesh( trianglesInLoD, Archetype.WorldTilingComponent.Tiling.TileVectors ) );

		//	instance.Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity ) );
		//}

		//var vertices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.Vertices;
		//var indices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.GetIndices( 5 );
		//List<WorldTileTriangle> trianglesInLoD = [];
		//for (int i = 0; i < indices.Count; i += 3) {
		//	trianglesInLoD.Add( new( indices[ i ], indices[ i + 1 ], indices[ i + 2 ], vertices ) );
		//}
		//WorldTileSceneInstance instance = _sceneInstanceCollection.Create<WorldTileSceneInstance>();
		//instance.SetMesh( CreateLoDMesh( trianglesInLoD, vertices ) );
		//instance.Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity ) );

		//var vertices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.Vertices;
		//var baseTiles = Archetype.WorldTilingComponent.Tiling.Tiles;
		//Random r = new( 42 );
		//for (int i = 0; i < baseTiles.Count; i++) {
		//	List<WorldTileTriangle> trianglesInLoD = [];
		//	BaseTile baseTile = baseTiles[ i ];
		//	int seekingLayer = baseTile.Layer + baseTile.Sublayers;
		//	AddTilesToList( baseTile, trianglesInLoD, seekingLayer );
		//	Vector4<double> baseColor = (r.NextDouble() * 0.5 + 0.5, r.NextDouble() * 0.5 + 0.5, r.NextDouble() * 0.5 + 0.5, 1);
		//	WorldTileSceneInstance instance = _sceneInstanceCollection.Create<WorldTileSceneInstance>();
		//	instance.SetMesh( CreateLoDMesh( trianglesInLoD, vertices, baseColor, r ) );
		//	instance.Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity ) );
		//}
	}

	private void AddTilesToList( IContainingTile baseTile, List<TileTriangle> trianglesInLoD, int seekingLayer ) {
		if (baseTile.Layer == seekingLayer || seekingLayer < baseTile.Layer) {
			trianglesInLoD.Add( new( baseTile ) );
			return;
		}

		foreach (ITile subTile in baseTile.SubTiles) {
			if (subTile is not IContainingTile container) {
				trianglesInLoD.Add( new( subTile ) );
				continue;
			}
			AddTilesToList( container, trianglesInLoD, seekingLayer );
		}
	}

	private IMesh CreateLoDMesh( List<TileTriangle> trianglesInLoD, IReadOnlyList<Vector3<double>> tileVectors ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];
		for (int i = 0; i < trianglesInLoD.Count; i++) {
			int a = trianglesInLoD[ i ].A;
			int b = trianglesInLoD[ i ].B;
			int c = trianglesInLoD[ i ].C;
			Vector4<byte> color = (trianglesInLoD[ i ].Color * 255).Clamp<Vector4<double>, double>( 0, 255 ).CastSaturating<double, byte>();

			Vector3<float> aV = tileVectors[ a ].CastSaturating<double, float>();
			Vector3<float> bV = tileVectors[ b ].CastSaturating<double, float>();
			Vector3<float> cV = tileVectors[ c ].CastSaturating<double, float>();

			//Create mesh
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( aV, 0, 0, color ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( bV, 0, 0, color ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( cV, 0, 0, color ) );

		}

		return RenderEntity.ServiceAccess.MeshProvider.CreateMesh( vertices.ToArray(), indices.ToArray() );
	}

}

public sealed class TileTriangle( ITile tile ) {

	private readonly ITile _tile = tile;

	public int A => _tile.IndexA;
	public int B => _tile.IndexB;
	public int C => _tile.IndexC;
	public Vector4<double> Color => _tile.Color;
	public Vector3<double> VectorA => _tile.VectorA;
	public Vector3<double> VectorB => _tile.VectorB;
	public Vector3<double> VectorC => _tile.VectorC;
}

public sealed class WorldTileLoD {



}

public sealed class WorldTileSceneInstance : SceneInstanceCollection<Vertex3, Entity2SceneData>.InstanceBase {

	private CompositeTile? _baseTile;

	public CompositeTile BaseTile => _baseTile ?? throw new InvalidOperationException( "BaseTile is not set." );

	public int CurrentLoD { get; set; }

	public new void SetMesh( IMesh mesh ) => base.SetMesh( mesh );
	public new bool Write<T>( T data ) where T : unmanaged => base.Write( data );

	internal void SetBaseTile( CompositeTile baseTile ) => _baseTile = baseTile;
}
