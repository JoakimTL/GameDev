using Engine.Module.Render.Entities;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Standard;
using Engine.Standard.Render.Input.Services;
using Engine.Transforms;
using Sandbox.Logic.Old.OldWorld;
using Sandbox.Logic.Old.OldWorld.Tiles;

namespace Sandbox.Render.Oldworld;

public sealed class WorldTileSelectionBehaviour : DependentRenderBehaviourBase<WorldSelectedTileArchetype> {

	private Vector2<double> _mousePointerLocation;
	private Vector3<float> _pointerDirection;
	private bool _changed = true;


	protected override void OnRenderEntitySet() {
		RenderEntity.ServiceAccess.Input.OnMouseMoved += OnMouseMoved;
		RenderEntity.ServiceAccess.Input.OnMouseButton += OnMouseButton;
		RenderEntity.ServiceAccess.CameraProvider.Main.Camera3.OnMatrixChanged += OnCameraMatrixChanged;
	}

	private void OnCameraMatrixChanged( IMatrixProvider<float> provider ) {
		_changed = true;
	}

	private void OnMouseMoved( MouseMoveEvent @event ) {
		_mousePointerLocation = @event.Position;
		_changed = true;

	}

	private void OnMouseButton( MouseButtonEvent @event ) {
		if (@event.Button != MouseButton.Left || @event.InputType != TactileInputType.Press)
			return;

		RenderEntity.ServiceAccess.Get<GameStateProvider>().Set( "selectedTile", RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Tile>( "hoveringTile" ) );
	}

	public override void Update( double time, double deltaTime ) {
		if (!_changed)
			return;
		_changed = false;
		Engine.Transforms.Camera.Perspective.Dynamic projection = RenderEntity.ServiceAccess.CameraProvider.Main.Projection3;
		Engine.Transforms.Camera.View3 view = RenderEntity.ServiceAccess.CameraProvider.Main.View3;
		Vector2<float> ndc = RenderEntity.ServiceAccess.Get<ProcessedMouseInputProvider>().MouseNDCTranslation.CastSaturating<double, float>();
		_pointerDirection = GetMouseUnprojected( projection.InverseMatrix, view.InverseMatrix, ndc );


		if (!TryGetRaySphereIntersection( RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation, _pointerDirection, 0, 1, out Vector3<float> intersectionPoint )) {
			RenderEntity.ServiceAccess.Get<GameStateProvider>().Set<Tile>( "hoveringTile", null );
			//RenderEntity.SendMessageToEntity( new TileHoverMessage( null ) );
			return;
		}

		//Use octree to find the tile to check. We can use the intersection point to find the base tile, but not the hovered tile.

		IReadOnlyList<Vector3<float>> vertices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.Vertices;
		CompositeTile? baseTile = Archetype.WorldTilingComponent.Tiling.Tiles.FirstOrDefault( p => RayIntersectsTriangle( 0, intersectionPoint, p.VectorA, p.VectorB, p.VectorC, out _ ) );

		RenderEntity.ServiceAccess.Get<GameStateProvider>().Set( "hoveringTile", FindTileSelection( baseTile, intersectionPoint ) );
	}

	private Tile? FindTileSelection( IContainingTile? baseTile, Vector3<float> intersectionPoint ) {
		if (baseTile is null)
			return null;
		foreach (ITile subTile in baseTile.SubTiles)
			if (RayIntersectsTriangle( 0, intersectionPoint, subTile.VectorA, subTile.VectorB, subTile.VectorC, out _ )) {
				if (subTile is Tile tile)
					return tile;
				if (subTile is not IContainingTile subTileContainingTile)
					continue;
				return FindTileSelection( subTileContainingTile, intersectionPoint );
			}
		return null;
	}

	protected override bool InternalDispose() {
		RenderEntity.ServiceAccess.Input.OnMouseMoved -= OnMouseMoved;
		RenderEntity.ServiceAccess.CameraProvider.Main.Camera3.OnMatrixChanged -= OnCameraMatrixChanged;
		return true;
	}

	public static Vector3<float> GetMouseUnprojected( Matrix4x4<float> inverseProjection, Matrix4x4<float> inverseView, Vector2<float> ndc ) {
		Vector4<float> mouseVector = new( ndc.X, ndc.Y, -1, 1 );
		Vector4<float> mouseEye = mouseVector * inverseProjection;
		mouseEye = new( mouseEye.X, mouseEye.Y, -1, 0 );
		Vector4<float> mouseWorld = mouseEye * inverseView;

		return new Vector3<float>( mouseWorld.X, mouseWorld.Y, mouseWorld.Z ).Normalize<Vector3<float>, float>();
	}

	public static bool TryGetRaySphereIntersection( Vector3<float> rayOrigin, Vector3<float> rayDirection, Vector3<float> sphereCenter, float sphereRadius, out Vector3<float> intersectionPoint ) {
		intersectionPoint = Vector3<float>.Zero;
		Vector3<float> oc = rayOrigin - sphereCenter;
		//float a = Vector3.Dot( rayDirection, rayDirection ); // Should be 1 if normalized
		float b = 2.0f * oc.Dot( rayDirection );
		float c = oc.Dot( oc ) - sphereRadius * sphereRadius;
		float discriminant = b * b - 4 /* * a*/ * c;

		if (discriminant < 0)           // No intersection
			return false; // Or some sentinel value indicating no intersection

		// Calculate the nearest intersection point
		float t = (-b - float.Sqrt( discriminant )) / 2.0f/* * a*/;
		if (t < 0)          // If the nearest t is negative, try the farther intersection
			t = (-b + float.Sqrt( discriminant )) / 2.0f/* * a*/;

		if (t < 0)          // Both intersections are behind the ray origin
			return false; // Or another sentinel value

		// Calculate the intersection point
		intersectionPoint = rayOrigin + t * rayDirection;
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
