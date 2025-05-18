using Civlike.Logic.Nations;
using Civlike.Logic.World;
using Engine;
using Engine.Module.Entities.Container;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Entities.Providers;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard;
using Engine.Transforms.Camera;

namespace Civlike.Client.Render.World;
public sealed class WorldCameraRenderBehaviour : DependentRenderBehaviourBase<WorldArchetype> {

	private Rotor3<float> _rotation = Rotor3<float>.MultiplicativeIdentity;
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
	private bool _initialized = false;

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
		if (!_initialized) {
			var startLocation = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Vector3<float>?>( "startLocation" );
			if (!startLocation.HasValue)
				return;
			var polar = startLocation.Value.ToNormalizedPolar();
			float yaw = float.Pi * 3 / 2 - polar.X;
			float pitch = float.Pi * 3 / 2 - polar.Y;
			Rotor3<float> yawRotor = Rotor3.FromAxisAngle( Vector3<float>.UnitY, yaw );
			Rotor3<float> pitchRotor = Rotor3.FromAxisAngle( Vector3<float>.UnitX, pitch );
			_rotation = (yawRotor * pitchRotor).Normalize<Rotor3<float>, float>();
			_zoom = 1.1f;
			_initialized = true;
			return;
		}
		//Handle zooming
		{
			_zoomVelocity = float.Round( _zoomVelocity * float.Max( 1 - (float) deltaTime * 10, 0 ), 6, MidpointRounding.ToZero );
			_zoom += _zoomVelocity * (float) deltaTime;
			if (_zoom < _minZoom)
				_zoom = _minZoom;
		}

		//Handle acceleration
		Vector2<float> acceleration = Vector2<float>.AdditiveIdentity;
		{
			if (_wKeyDown)
				acceleration += (0, 1);
			if (_sKeyDown)
				acceleration += (0, -1);
			if (_aKeyDown)
				acceleration += (-1, 0);
			if (_dKeyDown)
				acceleration += (1, 0);

			if (acceleration != 0)
				acceleration = acceleration.Normalize<Vector2<float>, float>();
		}

		//Handle rotation
		var changedRotation = _lastRotation != 0;
		{
			_customYawRotation += _lastRotation.X * 0.002f;
			_customPitchRotation += _lastRotation.Y * 0.002f;
			_lastRotation = 0;
			if (_customPitchRotation < -float.Pi / 3)
				_customPitchRotation = -float.Pi / 3;
			if (_customPitchRotation > 0)
				_customPitchRotation = 0;

			if (_rKeyDown) {
				changedRotation |= _customYawRotation != 0 || _customPitchRotation != 0;
				_customYawRotation = 0;
				_customPitchRotation = 0;
			}
		}

		//Handle movement
		{
			acceleration *= float.Exp( float.Min( _zoom * 2 - 2.25f, float.E ) ) * 4;

			_velocity += acceleration * (float) deltaTime;
			_velocity = (_velocity * float.Max( 1 - (float) deltaTime * 10, 0 )).Round<Vector2<float>, float>( 6, MidpointRounding.ToZero );

			var rotationRotor = Rotor3.FromAxisAngle( _rotation.Forward, _customYawRotation ) * _rotation;
			var up = rotationRotor.Up;
			var left = rotationRotor.Left;

			_rotation = Rotor3.FromAxisAngle( up, _velocity.X * ( float ) deltaTime ) * _rotation;
			_rotation = Rotor3.FromAxisAngle( left, _velocity.Y * (float) deltaTime ) * _rotation;
			_rotation = _rotation.Normalize<Rotor3<float>, float>();
		}
		//Update camera
		View3 cameraView = RenderEntity.ServiceAccess.CameraProvider.Main.View3; //TODO: Allow for named cameras through the component?
		Vector3<float> newTranslation = -_rotation.Forward * _zoom;
		if ((newTranslation - cameraView.Translation).Round<Vector3<float>, float>( 5, MidpointRounding.ToEven ) == 0 && !changedRotation)
			return;
		cameraView.Translation = newTranslation;
		//float yaw = float.Pi * 3 / 2 - _polarCoordinate.X;
		//float pitch = float.Pi * 3 / 2 - _polarCoordinate.Y;
		//Rotor3<float> yawRotor = Rotor3.FromAxisAngle( Vector3<float>.UnitY, yaw );
		//var pitchRotor = Rotor3.FromAxisAngle( Vector3<float>.UnitX, pitch );
		//var newRotation = (yawRotor * pitchRotor).Normalize<Rotor3<float>, float>();
		//newRotation = Rotor3.FromAxisAngle( newRotation.Forward, _customYawRotation ) * newRotation;
		//newRotation = Rotor3.FromAxisAngle( newRotation.Left, _customPitchRotation ) * newRotation;

		cameraView.Rotation = Rotor3.FromAxisAngle( _rotation.Forward, _customYawRotation ) * Rotor3.FromAxisAngle( _rotation.Left, _customPitchRotation ) * _rotation;
	}

	protected override bool InternalDispose() {
		RenderEntity.ServiceAccess.Input.OnKey -= OnKey;
		RenderEntity.ServiceAccess.Input.OnMouseWheelScrolled -= OnMouseWheelScrolled;
		RenderEntity.ServiceAccess.Input.OnMouseMoved -= OnMouseMoved;
		RenderEntity.ServiceAccess.Input.OnMouseButton -= OnMouseButton;
		return true;
	}
}