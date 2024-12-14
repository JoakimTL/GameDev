using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Sandbox.Logic.World;

namespace Sandbox.Render.World;
public sealed class WorldTilingBehaviour : SynchronizedRenderBehaviourBase<WorldArchetype> {

	private SceneInstanceCollection<Vertex3, Entity2SceneData>? _sceneInstanceCollection;
	private OcTree<WorldTileTriangle, float> _octree;

	protected override void OnUpdate( double time, double deltaTime ) {
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		return false;
	}

	protected override void Synchronize() {
		if (_sceneInstanceCollection is null)
			_sceneInstanceCollection = RenderEntity.RequestSceneInstanceCollection<Vertex3, Entity2SceneData, TestShaderBundle>( "test", 0 );
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

		var vertices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.Vertices;
		var indices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.GetIndices( 5 );
		List<WorldTileTriangle> trianglesInLoD = [];
		for (int i = 0; i < indices.Count; i += 3) {
			trianglesInLoD.Add( new( indices[ i ], indices[ i + 1 ], indices[ i + 2 ], vertices ) );
		}
		WorldTileSceneInstance instance = _sceneInstanceCollection.Create<WorldTileSceneInstance>();
		instance.SetMesh( CreateLoDMesh( trianglesInLoD, vertices ) );
		instance.Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity ) );
	}

	private IMesh CreateLoDMesh( List<WorldTileTriangle> trianglesIsLoD, IReadOnlyList<Vector3<double>> tileVectors ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];
		for (int i = 0; i < trianglesIsLoD.Count; i++) {
			int a = (int) trianglesIsLoD[ i ].A;
			int b = (int) trianglesIsLoD[ i ].B;
			int c = (int) trianglesIsLoD[ i ].C;

			Vector3<float> aV = tileVectors[ a ].CastSaturating<double, float>();
			Vector3<float> bV = tileVectors[ b ].CastSaturating<double, float>();
			Vector3<float> cV = tileVectors[ c ].CastSaturating<double, float>();

			//Create mesh
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( aV, 0, 0, (byte) ((i * 50 + 50) % 255) ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( bV, 0, 0, (byte) ((i * 50 + 50) % 255) ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( cV, 0, 0, (byte) ((i * 50 + 50) % 255) ) );

		}

		return RenderEntity.ServiceAccess.MeshProvider.CreateMesh( vertices.ToArray(), indices.ToArray() );
	}

}

public sealed class WorldTileTriangle : IOctreeLeaf<float> {
	public Vector3<float> Vector => _center;

	private IReadOnlyList<Vector3<double>> _tileVertices;
	private uint _a;
	private uint _b;
	private uint _c;
	private Vector3<float> _center;

	public uint A => _a;
	public uint B => _b;
	public uint C => _c;

	public WorldTileTriangle( uint a, uint b, uint c, IReadOnlyList<Vector3<double>> tileVertices ) {
		_a = a;
		_b = b;
		_c = c;
		_tileVertices = tileVertices;
		_center = Engine.Vector.Average<Vector3<double>, double>( [ tileVertices[ (int) a ], tileVertices[ (int) b ], tileVertices[ (int) c ] ] ).CastSaturating<double, float>();
	}

}

public sealed class WorldTileLoD {



}

public sealed class WorldTileSceneInstance : SceneInstanceCollection<Vertex3, Entity2SceneData>.InstanceBase {
	public new void SetMesh( IMesh mesh ) => base.SetMesh( mesh );
	public new bool Write<T>( T data ) where T : unmanaged => base.Write( data );
}