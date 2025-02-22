using Engine.Module.Render.Ogl.Providers;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Sandbox.Logic.Old.OldWorld.Tiles;

namespace Sandbox.Render.Oldworld;

public sealed class WorldTileSceneInstance : SceneInstanceCollection<Vertex3, Entity2SceneData>.InstanceBase {

	private RenderedFoundationTile? _renderTile;

	public RenderedFoundationTile RenderTile => _renderTile ?? throw new InvalidOperationException( $"{nameof( RenderTile )} is not set." );

	public int CurrentLoD { get; private set; }

	internal void SetRenderTile( RenderedFoundationTile renderTile ) {
		if (_renderTile is not null)
			throw new InvalidOperationException( $"{nameof( RenderTile )} is already set." );
		_renderTile = renderTile;
		Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity, new( ushort.MaxValue, ushort.MaxValue, ushort.MaxValue, ushort.MaxValue ) ) );
	}

	public void Update( MeshProvider meshProvider ) {
		if (RenderTile.LevelOfDetail == CurrentLoD)
			return;
		CurrentLoD = RenderTile.LevelOfDetail;
		//Update the instance.
		UpdateMesh( meshProvider );
		Write( new Entity2SceneData( Matrix4x4<float>.MultiplicativeIdentity, new( ushort.MaxValue, ushort.MaxValue, ushort.MaxValue, ushort.MaxValue ) ) );
	}

	private void UpdateMesh( MeshProvider meshProvider ) {
		List<ITile> tiles = [];
		Mesh?.Dispose();
		AddTilesToList( RenderTile.Tile, tiles, CurrentLoD );
		IMesh mesh = CreateLoDMesh( tiles, meshProvider );
		SetMesh( mesh );
	}

	private void AddTilesToList( IContainingTile baseTile, List<ITile> trianglesInLoD, int seekingLayer ) {
		if (baseTile.Layer == seekingLayer || seekingLayer < baseTile.Layer) {
			trianglesInLoD.Add( baseTile );
			return;
		}

		foreach (ITile subTile in baseTile.SubTiles) {
			if (subTile is not IContainingTile container) {
				trianglesInLoD.Add( subTile );
				continue;
			}
			AddTilesToList( container, trianglesInLoD, seekingLayer );
		}
	}

	private IMesh CreateLoDMesh( List<ITile> tiles, MeshProvider meshProvider ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];
		for (int i = 0; i < tiles.Count; i++) {
			Vector3<float> a = tiles[ i ].VectorA;
			Vector3<float> b = tiles[ i ].VectorB;
			Vector3<float> c = tiles[ i ].VectorC;
			Vector4<byte> color = (tiles[ i ].Color * 255).Clamp<Vector4<float>, float>( 0, 255 ).CastSaturating<float, byte>();

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
