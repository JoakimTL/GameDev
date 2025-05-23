﻿using Engine.Module.Render.Entities;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Standard.Render.Input.Services;
using Engine.Standard;
using Engine.Transforms;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

public sealed class GlobeTileSelectionBehaviour : DependentRenderBehaviourBase<GlobeArchetype> {

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
		_changed = true;
	}

	private void OnMouseButton( MouseButtonEvent @event ) {
		if (@event.Button != MouseButton.Left || @event.InputType != TactileInputType.Press)
			return;

		RenderEntity.ServiceAccess.Get<GameStateProvider>().SetNewState( "selectedTile", RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Tile>( "hoveringTile" ) );
	}

	public override void Update( double time, double deltaTime ) {
		if (!_changed)
			return;
		_changed = false;
		Engine.Transforms.Camera.Perspective.Dynamic projection = RenderEntity.ServiceAccess.CameraProvider.Main.Projection3;
		Engine.Transforms.Camera.View3 view = RenderEntity.ServiceAccess.CameraProvider.Main.View3;
		Vector2<float> ndc = RenderEntity.ServiceAccess.Get<ProcessedMouseInputProvider>().MouseNDCTranslation.CastSaturating<double, float>();
		Vector3<float> pointerDirection = ndc.GetMouseWorldDirection( view.InverseMatrix, projection.InverseMatrix );

		if (!TryGetRaySphereIntersection( RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation, pointerDirection, 0, 1, out Vector3<float> intersectionPoint )) {
			RenderEntity.ServiceAccess.Get<GameStateProvider>().SetNewState( "hoveringTile", null );
			//RenderEntity.SendMessageToEntity( new TileHoverMessage( null ) );
			return;
		}

		//Use octree to find the tile to check. We can use the intersection point to find the base tile, but not the hovered tile.

		AABB<Vector3<float>> bounds = Archetype.GlobeComponent.TileTree.MaxDepthBounds;
		bounds = AABB.Create( [ bounds.Minima * 0.25f, bounds.Maxima * 0.25f ] );
		IReadOnlyList<Tile> tilesInBounds = Archetype.GlobeComponent.TileTree.Get( bounds.MoveBy( intersectionPoint ) ); //TODO persistent list?

		foreach (Tile tile in tilesInBounds) {
			if (!RayIntersectsTriangle( RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation, pointerDirection, tile.RenderModel.VectorA, tile.RenderModel.VectorB, tile.RenderModel.VectorC, out _ ))
				continue;
			RenderEntity.ServiceAccess.Get<GameStateProvider>().SetNewState( "hoveringTile", tile );
			return;
		}

		RenderEntity.ServiceAccess.Get<GameStateProvider>().SetNewState( "hoveringTile", null );
	}

	protected override bool InternalDispose() {
		RenderEntity.ServiceAccess.Input.OnMouseMoved -= OnMouseMoved;
		RenderEntity.ServiceAccess.Input.OnMouseButton -= OnMouseButton;
		RenderEntity.ServiceAccess.CameraProvider.Main.Camera3.OnMatrixChanged -= OnCameraMatrixChanged;
		return true;
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
