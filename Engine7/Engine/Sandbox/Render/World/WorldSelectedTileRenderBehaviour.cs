using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Ogl.Scenes;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

public sealed class WorldSelectedTileRenderBehaviour : SynchronizedRenderBehaviourBase<WorldSelectedTileArchetype> {

	private Tile? _desyncHoveringTile;
	private Tile? _currentHoveringTile;
	private SceneInstanceCollection<LineVertex, Line3SceneData>? _instanceCollection;
	private readonly List<Line3Instance> _instances = [];
	private IMesh? _lineInstanceMesh;

	private bool _changed;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
		_instanceCollection = RenderEntity.RequestSceneInstanceCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( "test", 0 );
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

		_changed = true;
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!_changed)
			return;
		_changed = false;
		if (_instanceCollection is null)
			return;
		//if (_currentHoveringTile is null) {
		//	_instanceCollection.Clear();
		//	return;
		//}

		if (_currentHoveringTile is not null) {
			var region = _currentHoveringTile.ContainingTile as Region;
			if (region is null)
				return;

			while (_instances.Count < 3) {
				_instances.Add( _instanceCollection.Create<Line3Instance>() );
				_instances[ ^1 ].SetMesh( _lineInstanceMesh );
			}


			var vA = region.VectorA;
			var vB = region.VectorB;
			var vC = region.VectorC;

			var cross = (vB - vA).Cross( vC - vA );
			var magnitude = cross.Magnitude<Vector3<float>, float>();
			var normal = cross.Normalize<Vector3<float>, float>();

			var lift = 1 + magnitude * 5;
			var width = magnitude * 3;
			_instances[ 0 ].Write( new Line3SceneData( vA * lift, width, vB * lift, width, normal, -1, 1, (0, 0, 0.5f), 0, (255, 0, 0, 255) ) );
			_instances[ 1 ].Write( new Line3SceneData( vB * lift, width, vC * lift, width, normal, -1, 1, (0, 0, 0.5f), 0, (0, 255, 0, 255) ) );
			_instances[ 2 ].Write( new Line3SceneData( vC * lift, width, vA * lift, width, normal, -1, 1, (0, 0, 0.5f), 0, (0, 0, 255, 255) ) );
		}
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		if (component is WorldSelectedTileComponent selectedTileComponent) {
			_desyncHoveringTile = selectedTileComponent.HoveringTile;
			return true;
		}
		return false;
	}

	protected override void Synchronize() {
		_currentHoveringTile = _desyncHoveringTile;
		_changed = true;
	}
}
