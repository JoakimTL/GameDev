using Engine.Module.Render.Entities;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

public sealed class WorldTileLoDBehaviour : DependentRenderBehaviourBase<WorldArchetype> {

	private Vector3<float> _lastCameraTranslation;
	private readonly List<RenderedFoundationTile> _tiles = [];
	public IReadOnlyList<RenderedFoundationTile> Tiles => _tiles;

	protected override void OnRenderEntitySet() {
		IReadOnlyList<CompositeTile> baseTiles = Archetype.WorldTilingComponent.Tiling.Tiles;
		for (int i = 0; i < baseTiles.Count; i++)
			_tiles.Add( new( baseTiles[ i ] ) );
	}

	public override void Update( double time, double deltaTime ) {
		Vector3<float> currentCameraTranslation = RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation.Round<Vector3<float>, float>( 2, MidpointRounding.ToEven );
		if (_lastCameraTranslation == currentCameraTranslation)
			return;
		_lastCameraTranslation = currentCameraTranslation;

		Span<Vector3<float>> tileVectors = stackalloc Vector3<float>[ 3 ];
		for (int i = 0; i < _tiles.Count; i++) {
			RenderedFoundationTile renderedTile = _tiles[ i ];
			ITile tile = renderedTile.Tile;
			tile.FillSpan( tileVectors );
			Vector3<float> center = Vector.Average<Vector3<float>, float>( tileVectors );
			int seekingLayer = GetExpectedLevelOfDetailForDistance( (center - _lastCameraTranslation).Magnitude<Vector3<float>, float>(), tile.Layer, tile.Layer + tile.RemainingLayers );
			renderedTile.SetLevelOfDetail( int.Clamp( seekingLayer, (int) tile.Layer, (int) (tile.Layer + tile.RemainingLayers) ) );
		}
	}

	private int GetExpectedLevelOfDetailForDistance( float distance, uint minLayer, uint maxLayer ) {
		//Min layer is the lowest level of detail, max layer is the highest level of detail.
		int layerDifference = (int) (maxLayer - minLayer);
		float distanceToLayer = distance * 0.75f;
		float distancePart = distanceToLayer * layerDifference;
		float layer = maxLayer - distancePart;
		return int.Min( (int) float.Round( layer ), (int) maxLayer );
	}

	protected override bool InternalDispose() {
		return true;
	}
}
