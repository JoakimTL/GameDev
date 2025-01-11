using Engine.Module.Render.Ogl.Scenes;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

public sealed class TileRegionBorderContainer( RenderedFoundationTile tile, SceneInstanceCollection<LineVertex, Line3SceneData> sceneInstanceCollection, IMesh lineMesh ) {
	private readonly RenderedFoundationTile _tile = tile;
	private readonly SceneInstanceCollection<LineVertex, Line3SceneData> _sceneInstanceCollection = sceneInstanceCollection;
	private readonly IMesh _lineMesh = lineMesh;
	private readonly HashSet<(int, int)> _renderedEdges = []; //TODO: Causes double "exposure" for edges on the edge of the tile if the lines are ever transparent.
	private readonly List<Line3Instance> _instances = [];
	private int _currentLoD = -1;

	public void Update() {
		if (_tile.LevelOfDetail == _currentLoD)
			return;
		_currentLoD = _tile.LevelOfDetail;
		if (_currentLoD < _tile.Tile.Layer + _tile.Tile.RemainingLayers) {
			foreach (Line3Instance instance in _instances)
				instance.SetActive( false );
			return;
		}
		//Update the instance.
		UpdateInstances();
	}

	private void UpdateInstances() {
		_renderedEdges.Clear();
		int instanceIndex = 0;
		IReadOnlyList<Region> regions = _tile.Tile.GetAllRegions();
		for (int j = 0; j < regions.Count; j++) {
			Region region = regions[ j ];

			//var regionCenter = region.GetCenter();
			//if ((regionCenter - _lastCameraTranslation).Magnitude<Vector3<float>, float>() > 0.1f)
			//	continue;

			Vector3<float>[] vectorSpan = new Vector3<float>[ 3 ];
			int[] indexSpan = new int[ 3 ];
			region.FillSpan( vectorSpan );
			region.FillSpan( indexSpan );

			Vector3<float> vA = vectorSpan[ 0 ];
			Vector3<float> vB = vectorSpan[ 1 ];
			Vector3<float> vC = vectorSpan[ 2 ];

			Vector3<float> cross = (vB - vA).Cross( vC - vA );
			float magnitude = cross.Magnitude<Vector3<float>, float>();
			Vector3<float> normal = cross.Normalize<Vector3<float>, float>();

			for (int l = 0; l < 3; l++) {
				(int, int) edgeIndices = GetEdge( indexSpan[ l ], indexSpan[ (l + 1) % 3 ] );
				if (!_renderedEdges.Add( edgeIndices ))
					continue;

				Line3Instance instance;
				if (_instances.Count > instanceIndex) {
					instance = _instances[ instanceIndex ];
					instance.SetActive( true );
				} else {
					instance = _sceneInstanceCollection.Create<Line3Instance>();
					instance.SetMesh( _lineMesh );
					_instances.Add( instance );
				}

				instance.Write( new Line3SceneData( vectorSpan[ l ], magnitude * 1.5f, vectorSpan[ (l + 1) % 3 ], magnitude * 1.5f, normal, 0, 1, (-1, 0, 1), 0, 255 ) );
				instanceIndex++;
			}
		}
	}

	private (int, int) GetEdge( int indexA, int indexB ) => indexA < indexB
		? (indexA, indexB)
		: (indexB, indexA);
}
