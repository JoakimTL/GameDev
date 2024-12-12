using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Sandbox.Logic.World;

namespace Sandbox.Render.World;
public sealed class WorldTilingBehaviour : SynchronizedRenderBehaviourBase<WorldArchetype> {

	private SceneInstanceCollection<Vertex3, Entity2SceneData>? _sceneInstanceCollection;
	private OcTree<WorldTileTriangle, float> _octree;

	protected override bool PrepareSynchronization( ComponentBase component ) {
		return false;
	}

	protected override void Synchronize() {
		if (_sceneInstanceCollection is null)
			_sceneInstanceCollection = RenderEntity.RequestSceneInstanceCollection<Vertex3, Entity2SceneData, TestShaderBundle>( "test", 0 );
		if (_octree is null)
			_octree = new OcTree<WorldTileTriangle, float>( (uint) Archetype.WorldTilingComponent.Tiling.Tiles.Count );

		for (int i = 0; i < Archetype.WorldTilingComponent.Tiling.Tiles.Count; i++) {
			uint level = (uint) (Archetype.WorldTilingComponent.Tiling.Tiles.Count - 1 - i);
			foreach (Tile tile in Archetype.WorldTilingComponent.Tiling.Tiles[ i ])
				_octree.Add( new WorldTileTriangle( tile.IndexA, tile.IndexB, tile.IndexC, level, Archetype.WorldTilingComponent.Tiling.TileVectors ) );
		}

		//TODO: find some way to do this shit.
		List<List<AABB<Vector3<float>>>> boundsPerLevel = [];

		for (int i = Archetype.WorldTilingComponent.Tiling.Tiles.Count - 1; i >= 0; i--) {
			_octree.GetBoundsAtLevel( out var boundAtLevel, (uint) i );
			boundsPerLevel.Add( boundAtLevel );

			foreach (AABB<Vector3<float>> bounds in boundAtLevel) {
				WorldTileSceneInstance instance = _sceneInstanceCollection.Create<WorldTileSceneInstance>();
				List<WorldTileTriangle> trianglesInLoD = [];
				_octree.GetAll( bounds, trianglesInLoD, (uint) i );
				instance.SetMesh( CreateLoDMesh( trianglesInLoD, Archetype.WorldTilingComponent.Tiling.TileVectors ) );

				instance.Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity ) );
			}
		}


		//foreach (AABB<Vector3<float>> bounds in boundsPerLevel[ 0 ]) {
		//	WorldTileSceneInstance instance = _sceneInstanceCollection.Create<WorldTileSceneInstance>();
		//	List<WorldTileTriangle> trianglesIsLoD = [];
		//	_octree.GetAll( bounds, trianglesIsLoD );
		//	instance.SetMesh( CreateLoDMesh( trianglesIsLoD, Archetype.WorldTilingComponent.Tiling.TileVectors ) );

		//	instance.Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity ) );
		//}
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

	public uint Level { get; }

	public WorldTileTriangle( uint a, uint b, uint c, uint level, IReadOnlyList<Vector3<double>> tileVertices ) {
		_a = a;
		_b = b;
		_c = c;
		this.Level = level;
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