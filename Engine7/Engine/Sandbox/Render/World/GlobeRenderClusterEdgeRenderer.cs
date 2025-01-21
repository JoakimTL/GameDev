using Engine.Module.Render.Ogl.Scenes;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

public sealed class GlobeRenderClusterEdgeRenderer {
	private readonly RenderCluster _cluster;
	private readonly SceneObjectFixedCollection<LineVertex, Line3SceneData> _lineCollection;

	public GlobeRenderClusterEdgeRenderer( RenderCluster cluster, SceneObjectFixedCollection<LineVertex, Line3SceneData> lineCollection ) {
		Line3SceneData[] data = new Line3SceneData[ cluster.Edges.Count ];
		int i = 0;
		foreach (Edge edge in cluster.Edges) {
			var a = edge.VectorA;
			var b = edge.VectorB;
			var length = (b - a).Magnitude<Vector3<float>, float>();
			var thickness = length * 0.01f;
			var normal = edge.Normal;
			data[ i++ ] = new Line3SceneData( a, thickness, b, thickness, normal, 0, 1, (-1, 0, 1), 0.5f, 0.75f, 0.5f, (90, 90, 90, 255) );
		}
		lineCollection.WriteRange( 0, data );
		cluster.VisibilityChanged += OnClusterVisibilityChanged;
		this._cluster = cluster;
		this._lineCollection = lineCollection;
	}

	private void OnClusterVisibilityChanged() {
		_lineCollection.SetActiveElements( _cluster.IsVisible ? _lineCollection.MaxElements : 0 );
	}
}
