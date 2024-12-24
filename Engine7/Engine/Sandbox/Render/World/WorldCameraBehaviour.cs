using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Entities.Providers;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render;
using Engine.Transforms;
using Engine.Transforms.Camera;
using OpenGL;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;
using System.Drawing;
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

		cameraView.Translation = _polarCoordinate.ToCartesianFromPolar( _zoom ).Round<Vector3<float>, float>( 5, MidpointRounding.ToEven );
		Rotor3<float> newRotation = Rotor3.FromAxisAngle( Vector3<float>.UnitY, _polarCoordinate.X );
		newRotation = Rotor3.FromAxisAngle( newRotation.Left, _polarCoordinate.Y ) * newRotation;
		if ((newRotation.Left + newRotation.Forward).Round<Vector3<float>, float>( 5, MidpointRounding.ToEven ) == (cameraView.Rotation.Left + cameraView.Rotation.Forward).Round<Vector3<float>, float>( 5, MidpointRounding.ToEven ))
			return;
		cameraView.Rotation = newRotation;
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

		//Use octree to find the tile to check. We can use the intersection point to find the base tile, but not the hovered tile.

		var vertices = Archetype.WorldTilingComponent.Tiling.WorldIcosphere.Vertices;
		var baseTile = Archetype.WorldTilingComponent.Tiling.Tiles.FirstOrDefault( p => RayIntersectsTriangle( 0, intersectionPoint, p.VectorA.CastSaturating<double, float>(), p.VectorB.CastSaturating<double, float>(), p.VectorC.CastSaturating<double, float>(), out _ ) );

		Tile? selectedTiled = FindTileSelection( baseTile, intersectionPoint );

		RenderEntity.SendMessageToEntity( new TileHoverMessage( selectedTiled ) );
	}

	private Tile? FindTileSelection( IContainingTile? baseTile, Vector3<float> intersectionPoint ) {
		if (baseTile is null)
			return null;
		foreach (var subTile in baseTile.SubTiles) {
			if (RayIntersectsTriangle( 0, intersectionPoint, subTile.VectorA.CastSaturating<double, float>(), subTile.VectorB.CastSaturating<double, float>(), subTile.VectorC.CastSaturating<double, float>(), out _ )) {
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


			var vA = region.VectorA.CastSaturating<double, float>();
			var vB = region.VectorB.CastSaturating<double, float>();
			var vC = region.VectorC.CastSaturating<double, float>();

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

[Identity( nameof( LineVertex ) )]
[VAO.Setup( 0, 0, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public readonly struct LineVertex( Vector2<float> translation, Vector2<float> uv, Vector4<byte> color ) {
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 0 )]
	public readonly Vector2<float> Translation = translation;
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 8 )]
	public readonly Vector2<float> UV = uv;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 16 )]
	public readonly Vector4<byte> Color = color;
}

[Identity( nameof( Line3SceneData ) )]
[VAO.Setup( 0, 1, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public readonly struct Line3SceneData( Vector3<float> pointA, float widthA, Vector3<float> pointB, float widthB, Vector3<float> lineNormal, float negativeAnchor, float positiveAnchor, Vector3<float> quadratic, float gradientWidth, Vector4<byte> color ) {
	[VAO.Data( VertexAttribType.Float, 4 ), FieldOffset( 0 )]
	public readonly Vector4<float> PointA = new( pointA.X, pointA.Y, pointA.Z, widthA );
	[VAO.Data( VertexAttribType.Float, 4 ), FieldOffset( 16 )]
	public readonly Vector4<float> PointB = new( pointB.X, pointB.Y, pointB.Z, widthB );
	[VAO.Data( VertexAttribType.Float, 3 ), FieldOffset( 32 )]
	public readonly Vector3<float> LineNormal = lineNormal;
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 44 )]
	public readonly Vector2<float> FillAnchors = new( negativeAnchor, positiveAnchor );
	[VAO.Data( VertexAttribType.Float, 4 ), FieldOffset( 52 )]
	public readonly Vector4<float> FillQuadratic = new( quadratic.X, quadratic.Y, quadratic.Z, gradientWidth );
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 68 )]
	public readonly Vector4<byte> Color = color;
}

public sealed class Line3Instance : SceneInstanceCollection<LineVertex, Line3SceneData>.InstanceBase {
	public new void SetMesh( IMesh mesh ) => base.SetMesh( mesh );
	public new bool Write<T>( T data ) where T : unmanaged => base.Write( data );
}

[Identity( nameof( Line3ShaderBundle ) )]
public sealed class Line3ShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) => AddPipeline( "default", pipelineService.Get<Line3ShaderPipeline>() );
}

public sealed class Line3ShaderPipeline : OglShaderPipelineBase {
	public override bool UsesTransparency => false;

	protected override IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<Line3VertexShaderProgram>();
		yield return shaderProgramService.Get<LineFragmentShaderProgram>();
	}
}

public sealed class Line3VertexShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "line3.vert" ) );
}
public sealed class LineFragmentShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "line.frag" ) );
}