﻿using Engine.Module.Entities.Render;
using Engine.Module.Render.Entities.Providers;
using Engine.Module.Render.Input;
using Engine.Standard.Render;
using Engine.Transforms;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

public sealed class WorldTileSelectionBehaviour : DependentRenderBehaviourBase<WorldSelectedTileArchetype> {

	private Vector2<double> _mousePointerLocation;
	private Vector3<float> _pointerDirection;
	private bool _changed = true;

	private DebugInstance _debugInstance;

	protected override void OnRenderEntitySet() {
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseMoved += OnMouseMoved;
		RenderEntity.ServiceAccess.CameraProvider.Main.Camera3.OnMatrixChanged += OnCameraMatrixChanged;
		_debugInstance = RenderEntity.RequestSceneInstance<DebugInstance>( "test", 0 );
		_debugInstance.SetShaderBundle( RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<TestShaderBundle>()! );
		_debugInstance.SetVertexArrayObject( RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity2SceneData>()! );
		_debugInstance.SetMesh( RenderEntity.ServiceAccess.MeshProvider.CreateMesh(
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
			] ) );
	}

	private void OnCameraMatrixChanged( IMatrixProvider<float> provider ) {
		_changed = true;
	}

	private void OnMouseMoved( MouseMoveEvent @event ) {
		_mousePointerLocation = @event.Position;
		_changed = true;

	}

	public override void Update( double time, double deltaTime ) {
		if (!_changed)
			return;
		_changed = false;
		var projection = RenderEntity.ServiceAccess.CameraProvider.Main.Projection3;
		var view = RenderEntity.ServiceAccess.CameraProvider.Main.View3;
		var window = RenderEntity.ServiceAccess.Get<WindowProvider>().Window;
		var ndc = (_mousePointerLocation.DivideEntrywise( window.Size.CastSaturating<int, double>() ) * 2).CastSaturating<double, float>();
		ndc = (ndc.X - 1, 1 - ndc.Y);
		_pointerDirection = GetMouseUnprojected( projection.InverseMatrix, view.InverseMatrix, ndc );

		if (!TryGetRaySphereIntersection( RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation, _pointerDirection, 0, 1, out Vector3<float> intersectionPoint )) {
			RenderEntity.SendMessageToEntity( new TileHoverMessage( null ) );
			return;
		}

		//Use octree to find the tile to check. We can use the intersection point to find the base tile, but not the hovered tile.

		var vertices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.Vertices;
		var baseTile = Archetype.WorldTilingComponent.Tiling.Tiles.FirstOrDefault( p => RayIntersectsTriangle( 0, intersectionPoint, p.VectorA, p.VectorB, p.VectorC, out _ ) );

		Tile? selectedTiled = FindTileSelection( baseTile, intersectionPoint );

		RenderEntity.SendMessageToEntity( new TileHoverMessage( selectedTiled ) );
	}

	private Tile? FindTileSelection( IContainingTile? baseTile, Vector3<float> intersectionPoint ) {
		if (baseTile is null)
			return null;
		foreach (var subTile in baseTile.SubTiles) {
			if (RayIntersectsTriangle( 0, intersectionPoint, subTile.VectorA, subTile.VectorB, subTile.VectorC, out _ )) {
				if (subTile is Tile tile)
					return tile;
				if (subTile is not IContainingTile subTileContainingTile)
					continue;
				return FindTileSelection( subTileContainingTile, intersectionPoint );
			}
		}
		return null;
	}

	protected override bool InternalDispose() {
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseMoved -= OnMouseMoved;
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

		if (discriminant < 0) {
			// No intersection
			return false; // Or some sentinel value indicating no intersection
		}

		// Calculate the nearest intersection point
		float t = (-b - float.Sqrt( discriminant )) / (2.0f/* * a*/);
		if (t < 0) {
			// If the nearest t is negative, try the farther intersection
			t = (-b + float.Sqrt( discriminant )) / (2.0f/* * a*/);
		}

		if (t < 0) {
			// Both intersections are behind the ray origin
			return false; // Or another sentinel value
		}

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