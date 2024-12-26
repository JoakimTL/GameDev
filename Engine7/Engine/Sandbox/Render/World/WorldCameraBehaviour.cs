using Engine.Module.Entities.Render;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render;
using Engine.Transforms.Camera;
using OpenGL;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;
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
	private bool _rKeyDown;

	private float _customYawRotation = 0;
	private float _customPitchRotation = 0;
	private Vector2<double> _lastMousePosition;
	private Vector2<float> _lastRotation;
	private bool _shouldRotate = false;

	protected override void OnRenderEntitySet() {
		RenderEntity.ServiceAccess.UserInputEventService.OnKey += OnKey;
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseWheelScrolled += OnMouseWheelScrolled;
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseMoved += OnMouseMoved;
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseButton += OnMouseButton;
	}

	private void OnMouseButton( MouseButtonEvent @event ) {
		_shouldRotate = @event.InputType != TactileInputType.Release && @event.Button == MouseButton.Middle;
	}

	private void OnMouseMoved( MouseMoveEvent @event ) {
		if (_shouldRotate) {
			Vector2<double> delta = @event.Position - _lastMousePosition;
			_lastRotation += delta.CastSaturating<double, float>();
		}

		_lastMousePosition = @event.Position;
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
		if (@event.Key == Keys.R) {
			_rKeyDown = @event.InputType != TactileInputType.Release;
		}
	}

	public override void Update( double time, double deltaTime ) {
		Vector2<float> acceleration = Vector2<float>.AdditiveIdentity;
		if (_wKeyDown)
			acceleration += new Vector2<float>( float.Sin( _customYawRotation ), float.Cos( _customYawRotation ) );
		if (_sKeyDown)
			acceleration += new Vector2<float>( -float.Sin( _customYawRotation ), -float.Cos( _customYawRotation ) );
		if (_aKeyDown)
			acceleration += new Vector2<float>( -float.Cos( _customYawRotation ), float.Sin( _customYawRotation ) );
		if (_dKeyDown)
			acceleration += new Vector2<float>( float.Cos( _customYawRotation ), -float.Sin( _customYawRotation ) );

		if (acceleration != 0)
			acceleration = acceleration.Normalize<Vector2<float>, float>();

		if (_rKeyDown) {
			_customYawRotation = 0;
			_customPitchRotation = 0;
		}

		_zoomVelocity = float.Round( _zoomVelocity * float.Max( 1 - (float) deltaTime * 10, 0 ), 3, MidpointRounding.ToZero );
		_zoom += _zoomVelocity * (float) deltaTime;
		if (_zoom < _minZoom)
			_zoom = _minZoom;
		acceleration *= float.Exp( float.Min( _zoom * 2 - 2.75f, float.E ) ) * 5;

		_velocity += acceleration * (float) deltaTime;
		_velocity = (_velocity * float.Max( 1 - (float) deltaTime * 10, 0 )).Round<Vector2<float>, float>( 3, MidpointRounding.ToZero );

		_polarCoordinate += _velocity * (float) deltaTime;

		View3 cameraView = RenderEntity.ServiceAccess.CameraProvider.Main.View3; //Allow for named cameras through the component?

		_customYawRotation += _lastRotation.X * 0.002f;
		_customPitchRotation += _lastRotation.Y * 0.002f;
		_lastRotation = 0;

		cameraView.Translation = _polarCoordinate.ToCartesianFromPolar( _zoom ).Round<Vector3<float>, float>( 5, MidpointRounding.ToEven );
		Rotor3<float> newRotation = Rotor3.FromAxisAngle( Vector3<float>.UnitY, _polarCoordinate.X );
		newRotation = Rotor3.FromAxisAngle( newRotation.Left, _polarCoordinate.Y ) * newRotation;
		newRotation = Rotor3.FromAxisAngle( newRotation.Forward, _customYawRotation ) * newRotation;
		newRotation = Rotor3.FromAxisAngle( newRotation.Left, _customPitchRotation ) * newRotation;
		newRotation = newRotation.Normalize<Rotor3<float>, float>();

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


public sealed class DebugInstance : SceneInstanceCollection<Vertex3, Entity2SceneData>.InstanceBase {

	public new void SetVertexArrayObject( OglVertexArrayObjectBase vao ) => base.SetVertexArrayObject( vao );
	public new void SetShaderBundle( ShaderBundleBase shaderBundle ) => base.SetShaderBundle( shaderBundle );
	public new void SetMesh( IMesh mesh ) => base.SetMesh( mesh );
	public new bool Write<T>( T data ) where T : unmanaged => base.Write( data );
}


public sealed class TileHoverMessage( Tile? tile ) {
	public Tile? Tile { get; } = tile;
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
	public new void SetActive( bool active ) => base.SetActive( active );
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