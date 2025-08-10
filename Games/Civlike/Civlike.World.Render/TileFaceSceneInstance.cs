using Civlike.World.Geometry;
using Civlike.World.State;
using Civlike.World.State.States;
using Engine;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Providers;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;

namespace Civlike.World.Render;

public sealed class TileFaceSceneInstance() : SceneInstanceBase( typeof( Entity3SceneData ) ) {
	public void UpdateMesh( BoundedTileCluster cluster, MeshProvider meshProvider, Func<Tile, Vector4<float>> colorSelector ) {
		this.Mesh?.Dispose();
		SetMesh( CreateMesh( cluster, meshProvider, colorSelector ) );
	}

	private static IMesh CreateMesh( BoundedTileCluster cluster, MeshProvider meshProvider, Func<Tile, Vector4<float>> colorSelector ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];
		for (int i = 0; i < cluster.Tiles.Count; i++) {
			Tile tile = cluster.Tiles[ i ];
			ReadOnlyFace face = tile.Face;
			Node nodeA = tile.Nodes[ 0 ];
			Node nodeB = tile.Nodes[ 1 ];
			Node nodeC = tile.Nodes[ 2 ];
			ReadOnlyVertex vertexA = nodeA.Vertex;
			ReadOnlyVertex vertexB = nodeB.Vertex;
			ReadOnlyVertex vertexC = nodeC.Vertex;
			Vector3<float> a = vertexA.Vector * nodeA.GetState<NodeLandmassState>().HeightFactor;
			Vector3<float> b = vertexB.Vector * nodeB.GetState<NodeLandmassState>().HeightFactor;
			Vector3<float> c = vertexC.Vector * nodeC.GetState<NodeLandmassState>().HeightFactor;
			Vector4<byte> color = (colorSelector( tile ) * 255)
				.Clamp<Vector4<float>, float>( 0, 255 )
				.CastSaturating<float, byte>();

			//Create mesh
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( a, 0, 0, color ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( b, 0, 0, color ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( c, 0, 0, color ) );

		}

		return meshProvider.CreateMesh( vertices.ToArray(), [ .. indices ] );
	}
	public new void SetAllocated( bool allocated ) => base.SetAllocated( allocated );
	public new void SetVertexArrayObject( OglVertexArrayObjectBase? vertexArrayObject ) => base.SetVertexArrayObject( vertexArrayObject );
	public new void SetShaderBundle( ShaderBundleBase? shaderBundle ) => base.SetShaderBundle( shaderBundle );

	public bool Write( Entity3SceneData data ) => Write<Entity3SceneData>( data );
	public bool TryRead( out Entity3SceneData data ) => TryRead<Entity3SceneData>( out data );

	protected override void Initialize() {

	}
}