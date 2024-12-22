using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Entities.Providers;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Engine.Standard.Render.Meshing;
using Engine.Standard.Render.Meshing.Services;
using Engine.Transforms;
using Engine.Transforms.Camera;
using Sandbox.Logic.World;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Sandbox.Render.World;

public sealed class WorldCameraBehaviour : DependentRenderBehaviourBase<WorldCameraArchetype> {

	private Vector2<float> _polarCoordinate = (0, 0);
	private Vector2<float> _velocity;
	private float _zoom = 2;
	private float _zoomVelocity;
	private float _minZoom = 1.01f;
	private bool _wKeyDown;
	private bool _sKeyDown;
	private bool _aKeyDown;
	private bool _dKeyDown;

	protected override void OnRenderEntitySet() {
		RenderEntity.ServiceAccess.UserInputEventService.OnKey += OnKey;
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseWheelScrolled += OnMouseWheelScrolled;
	}

	private void OnMouseWheelScrolled( MouseWheelEvent @event ) {
		_zoomVelocity = (float) -@event.Movement.Y * float.Exp( float.Min( _zoom * 2 - 3.25f, float.E ) );
	}

	private void OnKey( KeyboardEvent @event ) {
		if (@event.Key == Keys.W) {
			_wKeyDown = @event.InputType != TactileInputType.Release;
		}
		if (@event.Key == Keys.S) {
			_sKeyDown = @event.InputType != TactileInputType.Release;
		}
		if (@event.Key == Keys.A) {
			_aKeyDown = @event.InputType != TactileInputType.Release;
		}
		if (@event.Key == Keys.D) {
			_dKeyDown = @event.InputType != TactileInputType.Release;
		}
	}

	public override void Update( double time, double deltaTime ) {
		Vector2<float> acceleration = Vector2<float>.AdditiveIdentity;
		if (_wKeyDown)
			acceleration += new Vector2<float>( 0, 1 );
		if (_sKeyDown)
			acceleration += new Vector2<float>( 0, -1 );
		if (_aKeyDown)
			acceleration += new Vector2<float>( -1, 0 );
		if (_dKeyDown)
			acceleration += new Vector2<float>( 1, 0 );

		_zoomVelocity = float.Round( _zoomVelocity * float.Max( 1 - (float) deltaTime * 10, 0 ), 3, MidpointRounding.ToZero );
		_zoom += _zoomVelocity * (float) deltaTime;
		if (_zoom < _minZoom)
			_zoom = _minZoom;
		acceleration *= float.Exp( float.Min( _zoom * 2 - 2.75f, float.E ) ) * 5;

		_velocity += acceleration * (float) deltaTime;
		_velocity = (_velocity * float.Max( 1 - (float) deltaTime * 10, 0 )).Round<Vector2<float>, float>( 3, MidpointRounding.ToZero );

		_polarCoordinate += _velocity * (float) deltaTime;

		View3 cameraView = RenderEntity.ServiceAccess.CameraProvider.Main.View3; //Allow for named cameras through the component?

		cameraView.Translation = _polarCoordinate.ToCartesianFromPolar( _zoom );
		cameraView.Rotation = Rotor3.FromAxisAngle( Vector3<float>.UnitY, _polarCoordinate.X );
		cameraView.Rotation = Rotor3.FromAxisAngle( cameraView.Rotation.Left, _polarCoordinate.Y ) * cameraView.Rotation;
	}

	protected override bool InternalDispose() {
		RenderEntity.ServiceAccess.UserInputEventService.OnKey -= OnKey;
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseWheelScrolled -= OnMouseWheelScrolled;
		return true;
	}
}

public sealed class WorldTileSelectionBehaviour : DependentRenderBehaviourBase<WorldSelectedTileArchetype> {

	private Vector2<double> _mousePointerLocation;
	private Vector3<float> _pointerDirection;
	private bool _changed = true;

	//private DebugInstance _debugInstance;

	protected override void OnRenderEntitySet() {
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseMoved += OnMouseMoved;
		RenderEntity.ServiceAccess.CameraProvider.Main.Camera3.OnMatrixChanged += OnCameraMatrixChanged;
		//_debugInstance = RenderEntity.RequestSceneInstance<DebugInstance>( "test", 0 );
		//_debugInstance.SetShaderBundle( RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<TestShaderBundle>()! );
		//_debugInstance.SetVertexArrayObject( RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity2SceneData>()! );
		//_debugInstance.SetMesh( RenderEntity.ServiceAccess.Get<PrimitiveMesh3Provider>().Get( Primitive3.Cube ) );
	}

	private void OnCameraMatrixChanged( IMatrixProvider<float> provider ) {
		_changed = true;
	}

	private void OnMouseMoved( MouseMoveEvent @event ) {
		_mousePointerLocation = @event.Movement;
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

		var vertices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.Vertices;
		var baseTile = Archetype.WorldTilingComponent.Tiling.Tiles.FirstOrDefault( p => RayIntersectsTriangle( 0, intersectionPoint, vertices[ (int) p.VectorIndexA ].CastSaturating<double, float>(), vertices[ (int) p.VectorIndexB ].CastSaturating<double, float>(), vertices[ (int) p.VectorIndexC ].CastSaturating<double, float>(), out _ ) );

		Tile? selectedTiled = FindTileSelection( baseTile, intersectionPoint );

		RenderEntity.SendMessageToEntity( new TileHoverMessage( selectedTiled ) );
	}

	private Tile? FindTileSelection( BaseTile? baseTile, Vector3<float> intersectionPoint ) {
		if (baseTile is null)
			return null;
		var vertices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.Vertices;
		if (baseTile.SubTiles is not null) {
			foreach (var subTile in baseTile.SubTiles) {
				if (RayIntersectsTriangle( 0, intersectionPoint, vertices[ (int) subTile.VectorIndexA ].CastSaturating<double, float>(), vertices[ (int) subTile.VectorIndexB ].CastSaturating<double, float>(), vertices[ (int) subTile.VectorIndexC ].CastSaturating<double, float>(), out _ )) {
					return FindTileSelection( subTile, intersectionPoint );
				}
			}
		}
		if (baseTile.Tiles is not null)
			foreach (var tile in baseTile.Tiles) {
				if (RayIntersectsTriangle( 0, intersectionPoint, vertices[ (int) tile.IndexA ].CastSaturating<double, float>(), vertices[ (int) tile.IndexB ].CastSaturating<double, float>(), vertices[ (int) tile.IndexC ].CastSaturating<double, float>(), out _ )) {
					return tile;
				}
			}
		return null;
	}

	protected override bool InternalDispose() {
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseMoved -= OnMouseMoved;
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


public sealed class DebugInstance : SceneInstanceCollection<Vertex3, Entity2SceneData>.InstanceBase {

	public new void SetVertexArrayObject( OglVertexArrayObjectBase vao ) => base.SetVertexArrayObject( vao );
	public new void SetShaderBundle( ShaderBundleBase shaderBundle ) => base.SetShaderBundle( shaderBundle );
	public new void SetMesh( IMesh mesh ) => base.SetMesh( mesh );
	public new bool Write<T>( T data ) where T : unmanaged => base.Write( data );
}


public sealed class TileHoverMessage( Tile? tile ) {
	public Tile? Tile { get; } = tile;
}

public sealed class WorldSelectedTileRenderBehaviour : SynchronizedRenderBehaviourBase<WorldSelectedTileArchetype> {

	private Tile? _desyncHoveringTile;
	private Tile? _currentHoveringTile;

	protected override void OnUpdate( double time, double deltaTime ) {
		//Render lines!
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
	}
}