using Engine.Module.Entities.Render;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Transforms.Camera;
using Sandbox.Logic.World;
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
		_zoomVelocity = (float) -@event.Movement.Y * float.Exp( float.Min( _zoom - 2.5f, float.E ) );
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
		acceleration *= float.Exp( float.Min( _zoom - 3, float.E ) ) * 5;

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