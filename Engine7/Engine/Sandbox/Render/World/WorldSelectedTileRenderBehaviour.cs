using Engine.Module.Entities.Container;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

public sealed class WorldSelectedTileRenderBehaviour : DependentRenderBehaviourBase<WorldSelectedTileArchetype> {

	private Tile? _currentHoveringTile;
	private Tile? _currentSelectedTile;
	private SceneInstanceCollection<LineVertex, Line3SceneData>? _instanceCollection;
	private readonly List<Line3Instance> _instances = [];
	private IMesh _lineInstanceMesh = null!;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
		_instanceCollection = RenderEntity.RequestSceneInstanceCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( "grid", 1 );
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
	}

	public override void Update( double time, double deltaTime ) {
		if (_instanceCollection is null)
			return;

		Tile? hoveringTile = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Tile>( "hoveringTile" );
		if (_currentHoveringTile != hoveringTile) {
			_currentHoveringTile = hoveringTile;
		}

		Tile? selectedTile = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Tile>( "selectedTile" );
		if (_currentSelectedTile != selectedTile) {
			_currentSelectedTile = selectedTile;
		}

		while (_instances.Count < 9) {
			_instances.Add( _instanceCollection.Create<Line3Instance>() );
			_instances[ ^1 ].SetMesh( _lineInstanceMesh );
		}

		SetHoveringTriangle();
		SetSelectedRegion( time );
		SetSelectedTile( time );
	}

	private void SetHoveringTriangle() {
		if (_currentHoveringTile is null) {
			_instances[ 0 ].SetActive( false );
			_instances[ 1 ].SetActive( false );
			_instances[ 2 ].SetActive( false );
			return;
		}

		Region? region = _currentHoveringTile.ContainingTile as Region;
		if (region is null)
			return;

		Vector3<float> vA = region.VectorA;
		Vector3<float> vB = region.VectorB;
		Vector3<float> vC = region.VectorC;

		Vector3<float> cross = (vB - vA).Cross( vC - vA );
		float magnitude = cross.Magnitude<Vector3<float>, float>();
		Vector3<float> normal = cross.Normalize<Vector3<float>, float>();

		float width = magnitude * 1.75f;
		_instances[ 0 ].Write( new Line3SceneData( vA, width, vB, width, normal, 0, 1, (-1, 0, 1), 0, (120, 120, 120, 255) ) );
		_instances[ 1 ].Write( new Line3SceneData( vB, width, vC, width, normal, 0, 1, (-1, 0, 1), 0, (120, 120, 120, 255) ) );
		_instances[ 2 ].Write( new Line3SceneData( vC, width, vA, width, normal, 0, 1, (-1, 0, 1), 0, (120, 120, 120, 255) ) );
		_instances[ 0 ].SetActive( true );
		_instances[ 1 ].SetActive( true );
		_instances[ 2 ].SetActive( true );
	}

	private void SetSelectedRegion( double time ) {
		if (_currentSelectedTile is null) {
			_instances[ 6 ].SetActive( false );
			_instances[ 7 ].SetActive( false );
			_instances[ 8 ].SetActive( false );
			return;
		}

		Region? region = _currentSelectedTile.ContainingTile as Region;
		if (region is null)
			return;

		Vector4<double> col1 = (.5 + double.Sin( time ) * .5, .5 + double.Sin( time + double.Pi / 3 ) * .5, .5 + double.Sin( time + double.Pi / 3 * 2 ) * .5, 1);
		Vector4<double> col2 = (.5 + double.Sin( time + double.Pi / 3 ) * .5, .5 + double.Sin( time + double.Pi / 3 * 2 ) * .5, .5 + double.Sin( time ) * .5, 1);
		Vector4<double> col3 = (.5 + double.Sin( time + double.Pi / 3 * 2 ) * .5, .5 + double.Sin( time ) * .5, .5 + double.Sin( time + double.Pi / 3 ) * .5, 1);
		Vector4<byte> col1B = (col1 * 255).Clamp<Vector4<double>, double>( 0, 255 ).CastSaturating<double, byte>();
		Vector4<byte> col2B = (col2 * 255).Clamp<Vector4<double>, double>( 0, 255 ).CastSaturating<double, byte>();
		Vector4<byte> col3B = (col3 * 255).Clamp<Vector4<double>, double>( 0, 255 ).CastSaturating<double, byte>();

		Vector3<float> vA = region.VectorA;
		Vector3<float> vB = region.VectorB;
		Vector3<float> vC = region.VectorC;
		Vector3<float> cross = (vB - vA).Cross( vC - vA );
		float magnitude = cross.Magnitude<Vector3<float>, float>();
		Vector3<float> normal = cross.Normalize<Vector3<float>, float>();
		float width = magnitude * 2;
		_instances[ 6 ].Write( new Line3SceneData( vA, width, vB, width, normal, 0, 1, (-1, 0, 1), 0, col1B, col2B ) );
		_instances[ 7 ].Write( new Line3SceneData( vB, width, vC, width, normal, 0, 1, (-1, 0, 1), 0, col2B, col3B ) );
		_instances[ 8 ].Write( new Line3SceneData( vC, width, vA, width, normal, 0, 1, (-1, 0, 1), 0, col3B, col1B ) );
		_instances[ 6 ].SetActive( true );
		_instances[ 7 ].SetActive( true );
		_instances[ 8 ].SetActive( true );
	}

	private void SetSelectedTile( double time ) {
		if (_currentSelectedTile is null) {
			_instances[ 3 ].SetActive( false );
			_instances[ 4 ].SetActive( false );
			_instances[ 5 ].SetActive( false );
			return;
		}

		Vector4<double> col1 = (.5 + double.Sin( time ) * .5, .5 + double.Sin( time + double.Pi / 3 ) * .5, .5 + double.Sin( time + double.Pi / 3 * 2 ) * .5, 1);
		Vector4<double> col2 = (.5 + double.Sin( time + double.Pi / 3 ) * .5, .5 + double.Sin( time + double.Pi / 3 * 2 ) * .5, .5 + double.Sin( time ) * .5, 1);
		Vector4<double> col3 = (.5 + double.Sin( time + double.Pi / 3 * 2 ) * .5, .5 + double.Sin( time ) * .5, .5 + double.Sin( time + double.Pi / 3 ) * .5, 1);
		Vector4<byte> col1B = (col1 * 255).Clamp<Vector4<double>, double>( 0, 255 ).CastSaturating<double, byte>();
		Vector4<byte> col2B = (col2 * 255).Clamp<Vector4<double>, double>( 0, 255 ).CastSaturating<double, byte>();
		Vector4<byte> col3B = (col3 * 255).Clamp<Vector4<double>, double>( 0, 255 ).CastSaturating<double, byte>();

		Vector3<float> vA = _currentSelectedTile.VectorA;
		Vector3<float> vB = _currentSelectedTile.VectorB;
		Vector3<float> vC = _currentSelectedTile.VectorC;
		Vector3<float> cross = (vB - vA).Cross( vC - vA );
		float magnitude = cross.Magnitude<Vector3<float>, float>();
		Vector3<float> normal = cross.Normalize<Vector3<float>, float>();
		float width = magnitude * 3;
		_instances[ 3 ].Write( new Line3SceneData( vA, width, vB, width, normal, 0, 1, (-1, 0, 1), 0, col1B, col2B ) );
		_instances[ 4 ].Write( new Line3SceneData( vB, width, vC, width, normal, 0, 1, (-1, 0, 1), 0, col2B, col3B ) );
		_instances[ 5 ].Write( new Line3SceneData( vC, width, vA, width, normal, 0, 1, (-1, 0, 1), 0, col3B, col1B ) );
		_instances[ 3 ].SetActive( true );
		_instances[ 4 ].SetActive( true );
		_instances[ 5 ].SetActive( true );
	}

	protected override bool InternalDispose() {
		_instanceCollection?.Clear();
		return true;
	}
}
