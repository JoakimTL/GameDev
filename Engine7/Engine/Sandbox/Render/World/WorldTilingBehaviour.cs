using Engine.Module.Entities.Render;
using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

public sealed class WorldTilingBehaviour : DependentRenderBehaviourBase<WorldArchetype>, IInitializable {

	private SceneInstanceCollection<Vertex3, Entity2SceneData>? _sceneInstanceCollection;
	private readonly List<WorldTileSceneInstance> _instances = [];

	public void Initialize() {
		if (!RenderEntity.TryGetBehaviour( out WorldTileLoDBehaviour? lodBehaviour ))
			throw new Exception( $"Needs {nameof( WorldTileLoDBehaviour )} to function!" );
		_sceneInstanceCollection = RenderEntity.RequestSceneInstanceCollection<Vertex3, Entity2SceneData, TestShaderBundle>( "test", 0 );
		foreach (var tile in lodBehaviour.Tiles) {
			WorldTileSceneInstance instance = _sceneInstanceCollection.Create<WorldTileSceneInstance>();
			instance.SetRenderTile( tile );
			_instances.Add( instance );
		}
	}

	public override void Update( double time, double deltaTime ) {
		foreach (var instance in _instances)
			instance.Update( RenderEntity.ServiceAccess.MeshProvider );
	}

	protected override bool InternalDispose() {
		return true;
	}
}

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
		if (_currentLoD < _tile.Tile.Layer + _tile.Tile.RemainingLayers - 1) {
			foreach (var instance in _instances)
				instance.SetActive( false );
			return;
		}
		//Update the instance.
		UpdateInstances();
	}

	private void UpdateInstances() {
		_renderedEdges.Clear();
		int instanceIndex = 0;
		var regions = _tile.Tile.GetAllRegions();
		for (int j = 0; j < regions.Count; j++) {
			var region = regions[ j ];

			//var regionCenter = region.GetCenter();
			//if ((regionCenter - _lastCameraTranslation).Magnitude<Vector3<float>, float>() > 0.1f)
			//	continue;

			var vectorSpan = new Vector3<float>[ 3 ];
			var indexSpan = new int[ 3 ];
			region.FillSpan( vectorSpan );
			region.FillSpan( indexSpan );

			var vA = vectorSpan[ 0 ];
			var vB = vectorSpan[ 1 ];
			var vC = vectorSpan[ 2 ];

			var cross = (vB - vA).Cross( vC - vA );
			var magnitude = cross.Magnitude<Vector3<float>, float>();
			var normal = cross.Normalize<Vector3<float>, float>();

			for (int l = 0; l < 3; l++) {
				var edgeIndices = GetEdge( indexSpan[ l ], indexSpan[ (l + 1) % 3 ] );
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

				instance.Write( new Line3SceneData( vectorSpan[ l ] * (1 + magnitude), magnitude, vectorSpan[ (l + 1) % 3 ] * (1 + magnitude), magnitude, normal, 0, 1, (-1, 0, 1), 0, 255 ) );
				instanceIndex++;
			}
		}
	}

	private (int, int) GetEdge( int indexA, int indexB ) => indexA < indexB
		? (indexA, indexB)
		: (indexB, indexA);
}

public sealed class RegionBorderRenderBehaviour : DependentRenderBehaviourBase<WorldArchetype>, IInitializable {

	private readonly List<TileRegionBorderContainer> _regionBorderContainers = [];
	private IMesh? _lineInstanceMesh;

	public void Initialize() {
		if (!RenderEntity.TryGetBehaviour( out WorldTileLoDBehaviour? lodBehaviour ))
			throw new Exception( $"Needs {nameof( WorldTileLoDBehaviour )} to function!" );
		_lineInstanceMesh = RenderEntity.ServiceAccess.MeshProvider.CreateMesh(
			[
				new LineVertex( (0, 1), (0, 1), 255 ),
				new LineVertex( (1, 1), (1, 1), 255 ),
				new LineVertex( (1, 0), (1, 0),  255 ),
				new LineVertex( (0, 0), (0, 0), 255 ),
				new LineVertex( (-1, 0), (1, 0), 255 ),
				new LineVertex( (-1, 1), (1, 1), 255 )
			], [
				0, 2, 1,
				0, 3, 2,
				0, 4, 5,
				0, 3, 4
			] );
		foreach (var tile in lodBehaviour.Tiles) {
			_regionBorderContainers.Add( new( tile, RenderEntity.RequestSceneInstanceCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( "test", 0 ), _lineInstanceMesh ) );
		}
	}

	public override void Update( double time, double deltaTime ) {
		foreach (var container in _regionBorderContainers)
			container.Update();
	}

	protected override bool InternalDispose() {
		return true;
	}
}

public sealed class WorldTileLoD {



}