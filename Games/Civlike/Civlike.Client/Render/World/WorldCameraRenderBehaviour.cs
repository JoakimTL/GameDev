using Civlike.Logic.World;
using Civlike.World.Components;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Standard;
using Engine.Transforms.Camera;

namespace Civlike.Client.Render.World;
public sealed class WorldCameraRenderBehaviour : DependentRenderBehaviourBase<GlobeArchetype> {

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
		this.RenderEntity.ServiceAccess.Input.OnKey += OnKey;
		this.RenderEntity.ServiceAccess.Input.OnMouseWheelScrolled += OnMouseWheelScrolled;
		this.RenderEntity.ServiceAccess.Input.OnMouseMoved += OnMouseMoved;
		this.RenderEntity.ServiceAccess.Input.OnMouseButton += OnMouseButton;
	}

	private void OnMouseButton( MouseButtonEvent @event ) {
		this._shouldRotate = @event.InputType != TactileInputType.Release && @event.Button == MouseButton.Middle;
	}

	private void OnMouseMoved( MouseMoveEvent @event ) {
		if (this._shouldRotate) {
			Vector2<double> delta = @event.Position - this._lastMousePosition;
			this._lastRotation += delta.CastSaturating<double, float>();
		}

		this._lastMousePosition = @event.Position;
	}

	private void OnMouseWheelScrolled( MouseWheelEvent @event ) {
		this._zoomVelocity = (float) -@event.Movement.Y * (float.Exp( float.Min( float.Max( (this._zoom * 2) - 3f, -0.95f ), float.E ) ) - .15f);
	}

	private void OnKey( KeyboardEvent @event ) {
		if (@event.Key == Keys.W)
			this._wKeyDown = @event.InputType != TactileInputType.Release;
		if (@event.Key == Keys.S)
			this._sKeyDown = @event.InputType != TactileInputType.Release;
		if (@event.Key == Keys.A)
			this._aKeyDown = @event.InputType != TactileInputType.Release;
		if (@event.Key == Keys.D)
			this._dKeyDown = @event.InputType != TactileInputType.Release;
		if (@event.Key == Keys.R)
			this._rKeyDown = @event.InputType != TactileInputType.Release;
	}

	public override void Update( double time, double deltaTime ) {
		if (!this._initialized) {
			Vector3<float>? startLocation = this.RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Vector3<float>?>( "startLocation" );
			if (!startLocation.HasValue)
				return;
			Vector2<float> polar = startLocation.Value.ToNormalizedPolar();
			float yaw = (float.Pi * 3 / 2) - polar.X;
			float pitch = (float.Pi * 3 / 2) - polar.Y;
			Rotor3<float> yawRotor = Rotor3.FromAxisAngle( Vector3<float>.UnitY, yaw );
			Rotor3<float> pitchRotor = Rotor3.FromAxisAngle( Vector3<float>.UnitX, pitch );
			this._rotation = (yawRotor * pitchRotor).Normalize<Rotor3<float>, float>();
			this._zoom = 1.1f;
			this._initialized = true;
			return;
		}
		//Handle zooming
		{
			this._zoomVelocity = float.Round( this._zoomVelocity * float.Max( 1 - ((float) deltaTime * 10), 0 ), 6, MidpointRounding.ToZero );
			this._zoom += this._zoomVelocity * (float) deltaTime;
			if (this._zoom < this._minZoom)
				this._zoom = this._minZoom;
		}

		//Handle acceleration
		Vector2<float> acceleration = Vector2<float>.AdditiveIdentity;
		{
			if (this._wKeyDown)
				acceleration += (0, 1);
			if (this._sKeyDown)
				acceleration += (0, -1);
			if (this._aKeyDown)
				acceleration += (-1, 0);
			if (this._dKeyDown)
				acceleration += (1, 0);

			if (acceleration != 0)
				acceleration = acceleration.Normalize<Vector2<float>, float>();
		}

		//Handle rotation
		bool changedRotation = this._lastRotation != 0;
		{
			this._customYawRotation += this._lastRotation.X * 0.002f;
			this._customPitchRotation += this._lastRotation.Y * 0.002f;
			this._lastRotation = 0;
			if (this._customPitchRotation < -float.Pi * 0.8f)
				this._customPitchRotation = -float.Pi * 0.8f;
			if (this._customPitchRotation > 0)
				this._customPitchRotation = 0;

			if (this._rKeyDown) {
				changedRotation |= this._customYawRotation != 0 || this._customPitchRotation != 0;
				this._customYawRotation = 0;
				this._customPitchRotation = 0;
			}
		}

		//Handle movement
		{
			acceleration *= float.Exp( float.Min( (this._zoom * 2) - 2.25f, float.E ) ) * 4;

			this._velocity += acceleration * (float) deltaTime;
			this._velocity = (this._velocity * float.Max( 1 - ((float) deltaTime * 10), 0 )).Round<Vector2<float>, float>( 6, MidpointRounding.ToZero );

			Rotor3<float> rotationRotor = Rotor3.FromAxisAngle( this._rotation.Forward, this._customYawRotation ) * this._rotation;
			Vector3<float> up = rotationRotor.Up;
			Vector3<float> left = rotationRotor.Left;

			this._rotation = Rotor3.FromAxisAngle( up, this._velocity.X * ( float ) deltaTime ) * this._rotation;
			this._rotation = Rotor3.FromAxisAngle( left, this._velocity.Y * (float) deltaTime ) * this._rotation;
			this._rotation = this._rotation.Normalize<Rotor3<float>, float>();
		}
		//Update camera
		View3 cameraView = this.RenderEntity.ServiceAccess.CameraProvider.Main.View3; //TODO: Allow for named cameras through the component?
		Vector3<float> newTranslation = -this._rotation.Forward * this._zoom;
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

		cameraView.Rotation = Rotor3.FromAxisAngle( this._rotation.Forward, this._customYawRotation ) * Rotor3.FromAxisAngle( this._rotation.Left, this._customPitchRotation ) * this._rotation;
	}

	protected override bool InternalDispose() {
		this.RenderEntity.ServiceAccess.Input.OnKey -= OnKey;
		this.RenderEntity.ServiceAccess.Input.OnMouseWheelScrolled -= OnMouseWheelScrolled;
		this.RenderEntity.ServiceAccess.Input.OnMouseMoved -= OnMouseMoved;
		this.RenderEntity.ServiceAccess.Input.OnMouseButton -= OnMouseButton;
		return true;
	}
}