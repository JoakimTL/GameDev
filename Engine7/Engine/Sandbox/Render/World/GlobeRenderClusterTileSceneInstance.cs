using Engine.Module.Render.Ogl.Providers;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Sandbox.Logic.World.Tiles;
using Sandbox.Render.Oldworld;

namespace Sandbox.Render.World;

public sealed class GlobeRenderClusterTileSceneInstance : SceneInstanceCollection<Vertex3, Entity3SceneData>.InstanceBase {

	private RenderCluster? _renderCluster;
	public RenderCluster RenderCluster => _renderCluster ?? throw new InvalidOperationException( $"{nameof( RenderCluster )} is not set." );

	private bool _needsMeshUpdate = false;
	private bool _visibilityChanged = false;

	internal void SetTileRenderCluster( RenderCluster cluster ) {
		if (_renderCluster is not null)
			throw new InvalidOperationException( $"{nameof( RenderCluster )} is already set." );
		_renderCluster = cluster;
		_needsMeshUpdate = true;
		_renderCluster.VisibilityChanged += OnVisibilityChanged;
	}

	private void OnVisibilityChanged() {
		_visibilityChanged = true;
	}

	public void Update( MeshProvider meshProvider ) {
		if (!(_visibilityChanged || _needsMeshUpdate) || _renderCluster is null)
			return;
		//Update the instance.
		SetActive( _renderCluster.IsVisible );
		_visibilityChanged = false;
		if (Active && _needsMeshUpdate) {
			UpdateMesh( meshProvider );
			Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity, new( ushort.MaxValue, ushort.MaxValue, ushort.MaxValue, ushort.MaxValue ) ) );
			_needsMeshUpdate = false;
		}
	}

	private void UpdateMesh( MeshProvider meshProvider ) {
		Mesh?.Dispose();
		SetMesh( CreateMesh( RenderCluster.Tiles, meshProvider ) );
	}

	private IMesh CreateMesh( IReadOnlyList<Tile> tiles, MeshProvider meshProvider ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];
		for (int i = 0; i < tiles.Count; i++) {
			TileRenderModel renderModel = tiles[ i ].RenderModel;
			Vector3<float> a = renderModel.VectorA;
			Vector3<float> b = renderModel.VectorB;
			Vector3<float> c = renderModel.VectorC;
			Vector4<byte> color = (renderModel.Color * 255).Clamp<Vector4<float>, float>( 0, 255 ).CastSaturating<float, byte>();

			//Create mesh
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( a, 0, 0, color ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( b, 0, 0, color ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( c, 0, 0, color ) );

		}

		return meshProvider.CreateMesh( vertices.ToArray(), indices.ToArray() );
	}
}