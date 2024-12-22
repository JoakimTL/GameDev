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

	private DebugInstance _debugInstance;

	protected override void OnRenderEntitySet() {
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseMoved += OnMouseMoved;
		RenderEntity.ServiceAccess.CameraProvider.Main.Camera3.OnMatrixChanged += OnCameraMatrixChanged;
		_debugInstance = RenderEntity.RequestSceneInstance<DebugInstance>( "test", 0 );
		_debugInstance.SetShaderBundle( RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<TestShaderBundle>()! );
		_debugInstance.SetVertexArrayObject( RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity2SceneData>()! );
		_debugInstance.SetMesh( RenderEntity.ServiceAccess.Get<PrimitiveMesh3Provider>().Get( Primitive3.Cube ) );
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
		_pointerDirection = GetMouseUnprojected( projection.InverseMatrix, view.InverseMatrix, (_mousePointerLocation.DivideEntrywise( window.Size.CastSaturating<int, double>() ) * 2 - 1).MultiplyEntrywise( (1, -1) ).CastSaturating<double, float>() );

		this.LogLine( $"Pointer direction: {_pointerDirection}", Log.Level.VERBOSE );

		if (!TryGetRaySphereIntersection( RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation, _pointerDirection, 0, 1, out Vector3<float> intersectionPoint )) {
			RenderEntity.SendMessageToEntity( new TileSelectionMessage( null ) );
			return;
		}

		this.LogLine( $"Intersection: {intersectionPoint}", Log.Level.VERBOSE );
		_debugInstance.Write( new Entity2SceneData( Matrix.Create4x4.Scaling( 0.01f, 0.01f, 0.01f ) * Matrix.Create4x4.Translation( intersectionPoint ) ) );
	}

	protected override bool InternalDispose() {
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseMoved -= OnMouseMoved;
		return true;
	}

	public static Vector3<float> GetMouseUnprojected( Matrix4x4<float> inverseProjection, Matrix4x4<float> inverseView, Vector2<float> ndc ) {
		Vector4<float> mouseVector = new( ndc.X, ndc.Y, -1, 1 );
		Vector4<float> mouseEye = inverseProjection * mouseVector;
		mouseEye = new( mouseEye.X, mouseEye.Y, -1, 0 );
		Vector4<float> mouseWorld = inverseView * mouseEye;

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
}


public sealed class DebugInstance : SceneInstanceCollection<Vertex3, Entity2SceneData>.InstanceBase {

	public new void SetVertexArrayObject( OglVertexArrayObjectBase vao ) => base.SetVertexArrayObject( vao );
	public new void SetShaderBundle( ShaderBundleBase shaderBundle ) => base.SetShaderBundle( shaderBundle );
	public new void SetMesh( IMesh mesh ) => base.SetMesh( mesh );
	public new bool Write<T>( T data ) where T : unmanaged => base.Write( data );
}


public sealed class TileSelectionMessage( Tile? tile ) {
	public Tile? Tile { get; } = tile;
}

public sealed class WorldSelectedTileRenderBehaviour : SynchronizedRenderBehaviourBase<WorldSelectedTileArchetype> {
	protected override void OnUpdate( double time, double deltaTime ) {
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		return false;
	}

	protected override void Synchronize() {
	}
}