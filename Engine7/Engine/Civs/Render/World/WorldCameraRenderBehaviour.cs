using Civs.Logic.World;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Transforms.Camera;

namespace Civs.Render.World;
public sealed class WorldCameraRenderBehaviour : DependentRenderBehaviourBase<WorldArchetype> {

	private Vector2<float> _polarCoordinate = (0, 0);
	private Vector2<float> _velocity;
	private float _zoom = 2;
	private float _zoomVelocity;
	private float _minZoom = 1.005f;
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
		RenderEntity.ServiceAccess.Input.OnKey += OnKey;
		RenderEntity.ServiceAccess.Input.OnMouseWheelScrolled += OnMouseWheelScrolled;
		RenderEntity.ServiceAccess.Input.OnMouseMoved += OnMouseMoved;
		RenderEntity.ServiceAccess.Input.OnMouseButton += OnMouseButton;
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
		_zoomVelocity = (float) -@event.Movement.Y * (float.Exp( float.Min( float.Max( _zoom * 2 - 3f, -0.95f ), float.E ) ) - .15f);
	}

	private void OnKey( KeyboardEvent @event ) {
		if (@event.Key == Keys.W)
			_wKeyDown = @event.InputType != TactileInputType.Release;
		if (@event.Key == Keys.S)
			_sKeyDown = @event.InputType != TactileInputType.Release;
		if (@event.Key == Keys.A)
			_aKeyDown = @event.InputType != TactileInputType.Release;
		if (@event.Key == Keys.D)
			_dKeyDown = @event.InputType != TactileInputType.Release;
		if (@event.Key == Keys.R)
			_rKeyDown = @event.InputType != TactileInputType.Release;
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

		_zoomVelocity = float.Round( _zoomVelocity * float.Max( 1 - (float) deltaTime * 10, 0 ), 6, MidpointRounding.ToZero );
		_zoom += _zoomVelocity * (float) deltaTime;
		if (_zoom < _minZoom)
			_zoom = _minZoom;
		acceleration *= float.Exp( float.Min( _zoom * 2 - 2.25f, float.E ) ) * 4;

		_velocity += acceleration * (float) deltaTime;
		_velocity = (_velocity * float.Max( 1 - (float) deltaTime * 10, 0 )).Round<Vector2<float>, float>( 6, MidpointRounding.ToZero );

		_polarCoordinate += _velocity * (float) deltaTime;

		View3 cameraView = RenderEntity.ServiceAccess.CameraProvider.Main.View3; //TODO: Allow for named cameras through the component?

		_customYawRotation += _lastRotation.X * 0.002f;
		_customPitchRotation += _lastRotation.Y * 0.002f;
		_lastRotation = 0;
		if (_customPitchRotation < -float.Pi / 3)
			_customPitchRotation = -float.Pi / 3;
		if (_customPitchRotation > 0)
			_customPitchRotation = 0;

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
		RenderEntity.ServiceAccess.Input.OnKey -= OnKey;
		RenderEntity.ServiceAccess.Input.OnMouseWheelScrolled -= OnMouseWheelScrolled;
		RenderEntity.ServiceAccess.Input.OnMouseMoved -= OnMouseMoved;
		RenderEntity.ServiceAccess.Input.OnMouseButton -= OnMouseButton;
		return true;
	}
}