using Civlike.World.State;
using Civlike.World.State.States;
using Engine;
using Engine.Module.Render.Ogl.Providers;
using Engine.Standard.Render;

namespace Civlike.World.Render;

public sealed class BoundedTileClusterRenderer : DisposableIdentifiable {

	private bool _changed;

	public BoundedTileClusterRenderer( BoundedTileCluster cluster, TileFaceSceneInstance sceneInstance ) {
		this.Cluster = cluster;
		this.SceneInstance = sceneInstance;
		Cluster.StateChanged += OnClusterStateChanged;
		_changed = true;
	}

	private void OnClusterStateChanged( BoundedTileCluster cluster ) {
		//TODO
	}

	public BoundedTileCluster Cluster { get; }
	public TileFaceSceneInstance SceneInstance { get; }

	public void UpdateVisibility( Vector3<float> normalizedTranslation, Matrix4x4<float> viewProjectionMatrix ) {

		bool shouldBeVisible = false;

		AABB<Vector3<float>> bounds = Cluster.RenderCluster.Bounds;
		Vector3<float> min = bounds.Minima;
		Vector3<float> max = bounds.Maxima;

		Span<Vector3<float>> boundsCorners =
		[
			(min.X, min.Y, min.Z),
			(min.X, min.Y, max.Z),
			(min.X, max.Y, max.Z),
			(min.X, max.Y, min.Z),
			(max.X, min.Y, min.Z),
			(max.X, min.Y, max.Z),
			(max.X, max.Y, max.Z),
			(max.X, max.Y, min.Z)
		];

		for (int i = 0; i < boundsCorners.Length; i++)
			if (normalizedTranslation.Dot( boundsCorners[ i ].Normalize<Vector3<float>, float>() ) >= 0 /*0.20629947401*/) {
				shouldBeVisible = true;
				break;
			}

		if (!shouldBeVisible)
			SetVisibility( false );

		shouldBeVisible = false;

		for (int i = 0; i < boundsCorners.Length; i++) {
			Vector3<float>? transformedCorner = boundsCorners[ i ].TransformWorld( viewProjectionMatrix );
			if (!transformedCorner.HasValue)
				continue;
			Vector3<float> transformed = transformedCorner.Value;
			//Check if the transformed corner is within the view frustum
			if (transformed.X <= 1 && transformed.X >= -1 && transformed.Y <= 1 && transformed.Y >= -1) {
				shouldBeVisible = true;
				break;
			}
		}

		SetVisibility( shouldBeVisible );
	}

	public void SetVisibility( bool visible ) => SceneInstance.SetAllocated( visible );

	public void UpdateMesh( MeshProvider meshProvider ) {
		if (!this.SceneInstance.Allocated)
			return;
		SceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
		if (!_changed)
			return;
		_changed = false;
		SceneInstance.UpdateMesh( Cluster, meshProvider, SelectColorForTile );
	}

	private Vector4<float> SelectColorForTile( Tile tile ) {
		return tile.GetState<TileColorState>().Color;
	}

	protected override bool InternalDispose() {
		return true;
	}
}
