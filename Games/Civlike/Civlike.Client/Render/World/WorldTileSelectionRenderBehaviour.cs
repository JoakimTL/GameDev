using Civlike.Logic.World;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Standard.Render.Input.Services;
using Engine.Standard;
using Engine.Transforms;
using Civlike.World.GameplayState;

namespace Civlike.Client.Render.World;

public sealed class WorldTileSelectionRenderBehaviour : DependentRenderBehaviourBase<WorldArchetype> {

	private bool _changed = true;

	protected override void OnRenderEntitySet() {
		this.RenderEntity.ServiceAccess.Input.OnMouseMoved += OnMouseMoved;
		this.RenderEntity.ServiceAccess.Input.OnMouseButton += OnMouseButton;
		this.RenderEntity.ServiceAccess.CameraProvider.Main.Camera3.OnMatrixChanged += OnCameraMatrixChanged;

	}

	private void OnCameraMatrixChanged( IMatrixProvider<float> provider ) {
		this._changed = true;
	}

	private void OnMouseMoved( MouseMoveEvent @event ) {
		this._changed = true;
	}

	private void OnMouseButton( MouseButtonEvent @event ) {
		if (@event.Button != MouseButton.Left || @event.InputType != TactileInputType.Press)
			return;

		this.RenderEntity.ServiceAccess.Get<GameStateProvider>().SetNewState( "selectedTile", this.RenderEntity.ServiceAccess.Get<InternalStateProvider>().Get<Face>( "hoveringTile" ) );
	}

	public override void Update( double time, double deltaTime ) {
		if (!this._changed)
			return;
		this._changed = false;
		Globe globe = this.Archetype.GlobeComponent.Globe;
		Engine.Transforms.Camera.Perspective.Dynamic projection = this.RenderEntity.ServiceAccess.CameraProvider.Main.Projection3;
		Engine.Transforms.Camera.View3 view = this.RenderEntity.ServiceAccess.CameraProvider.Main.View3;
		Vector2<float> ndc = this.RenderEntity.ServiceAccess.Get<ProcessedMouseInputProvider>().MouseNDCTranslation.CastSaturating<double, float>();
		Vector3<float> pointerDirection = ndc.GetMouseWorldDirection( view.InverseMatrix, projection.InverseMatrix );

		if (!TryGetRaySphereIntersection( this.RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation, pointerDirection, 0, 1, out Vector3<float> intersectionPoint )) {
			this.RenderEntity.ServiceAccess.Get<InternalStateProvider>().Set( "hoveringTile", null );
			this.RenderEntity.ServiceAccess.Get<InternalStateProvider>().Set( "mousePointerGlobeSphereIntersection", null );
			return;
		}

		this.RenderEntity.ServiceAccess.Get<InternalStateProvider>().Set( "mousePointerGlobeSphereIntersection", intersectionPoint );

		//Use octree to find the tile to check. We can use the intersection point to find the base tile, but not the hovered tile.

		AABB<Vector3<float>> bounds = globe.ClusterBounds.MoveBy( intersectionPoint ).ScaleBy( 0.25f );
		foreach (BoundedRenderCluster cluster in globe.Clusters.Where( p => p.Bounds.Intersects( bounds ) ))
			foreach (Face face in cluster.Faces) {
				if (!RayIntersectsTriangle( this.RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation, pointerDirection, face.Blueprint.DisplayVectorA, face.Blueprint.DisplayVectorB, face.Blueprint.DisplayVectorC, out _ ))
					continue;
				this.RenderEntity.ServiceAccess.Get<InternalStateProvider>().Set( "hoveringTile", face );
				return;
			}

		this.RenderEntity.ServiceAccess.Get<InternalStateProvider>().Set( "hoveringTile", null );
	}

	protected override bool InternalDispose() {
		this.RenderEntity.ServiceAccess.Input.OnMouseMoved -= OnMouseMoved;
		this.RenderEntity.ServiceAccess.Input.OnMouseButton -= OnMouseButton;
		this.RenderEntity.ServiceAccess.CameraProvider.Main.Camera3.OnMatrixChanged -= OnCameraMatrixChanged;
		return true;
	}

	public static bool TryGetRaySphereIntersection( Vector3<float> rayOrigin, Vector3<float> rayDirection, Vector3<float> sphereCenter, float sphereRadius, out Vector3<float> intersectionPoint ) {
		intersectionPoint = Vector3<float>.Zero;
		Vector3<float> oc = rayOrigin - sphereCenter;
		//float a = Vector3.Dot( rayDirection, rayDirection ); // Should be 1 if normalized
		float b = 2.0f * oc.Dot( rayDirection );
		float c = oc.Dot( oc ) - (sphereRadius * sphereRadius);
		float discriminant = (b * b) - (4 /* * a*/ * c);

		if (discriminant < 0)           // No intersection
			return false; // Or some sentinel value indicating no intersection

		// Calculate the nearest intersection point
		float t = (-b - float.Sqrt( discriminant )) / 2.0f/* * a*/;
		if (t < 0)          // If the nearest t is negative, try the farther intersection
			t = (-b + float.Sqrt( discriminant )) / 2.0f/* * a*/;

		if (t < 0)          // Both intersections are behind the ray origin
			return false; // Or another sentinel value

		// Calculate the intersection point
		intersectionPoint = rayOrigin + (t * rayDirection);
		return true;
	}

	public static bool RayIntersectsTriangle( Vector3<float> rayOrigin, Vector3<float> rayDirection, Vector3<float> v0, Vector3<float> v1, Vector3<float> v2, out float t ) {
		const float EPSILON = 1e-8f;
		t = 0;

		Vector3<float> edge1 = v1 - v0;
		Vector3<float> edge2 = v2 - v0;

		Vector3<float> h = rayDirection.Cross( edge2 );
		float a = edge1.Dot( h );

		if (float.Abs( a ) < EPSILON)
			return false; // Ray is parallel to triangle.

		float f = 1.0f / a;
		Vector3<float> s = rayOrigin - v0;
		float u = f * s.Dot( h );

		if (u < 0.0f || u > 1.0f)
			return false;

		Vector3<float> q = s.Cross( edge1 );
		float v = f * rayDirection.Dot( q );

		if (v < 0.0f || u + v > 1.0f)
			return false;

		t = f * edge2.Dot( q );

		if (t > EPSILON)
			return true; // Intersection detected.

		return false; // Intersection is behind the ray.
	}
}
