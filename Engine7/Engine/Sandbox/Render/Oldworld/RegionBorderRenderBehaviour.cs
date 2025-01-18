using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Sandbox.Logic.OldWorld;

namespace Sandbox.Render.Oldworld;

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
				0, 5, 4,
				0, 4, 3
			] );
		foreach (RenderedFoundationTile tile in lodBehaviour.Tiles)
			_regionBorderContainers.Add( new( tile, RenderEntity.RequestSceneInstanceCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( "grid", 0 ), _lineInstanceMesh ) );
	}

	public override void Update( double time, double deltaTime ) {
		foreach (TileRegionBorderContainer container in _regionBorderContainers)
			container.Update();
	}

	protected override bool InternalDispose() {
		return true;
	}
}
