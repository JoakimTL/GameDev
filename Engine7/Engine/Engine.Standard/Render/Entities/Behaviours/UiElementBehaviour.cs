using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Input;
using Engine.Physics;
using Engine.Standard.Entities.Components;
using Engine.Standard.Render.Input.Services;
using Engine.Transforms;

namespace Engine.Standard.Render.Entities.Behaviours;

public sealed class UiElementBehaviour : SynchronizedRenderBehaviourBase<UiElementArchetype> {
	//Have the ui element act alone. It should be sending a message to the ui component when it is clicked, but visuals and the like should be handled on the render thread with behaviours.
	//This means collision detection is handled here. The ui component should contain collision data, but collision should be handled here.

	private MouseColliderProvider _mouseColliderProvider = null!;

	private List<Vector2<double>> _incomingVertices = [];
	private readonly Collider2Shape _collider = new();
	private Collision2Calculation<double> _collisionCalculation = null!;

	private bool _hovering = false;

	protected override void OnRenderEntitySet() {
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseButton += OnMouseButton;
		_mouseColliderProvider = RenderEntity.ServiceAccess.Get<MouseColliderProvider>();
		_incomingVertices = [ .. Archetype.Collider2Component.ColliderVertices ];
		_collisionCalculation = new Collision2Calculation<double>( _collider, _mouseColliderProvider.ColliderNDC );
	}

	private void OnMouseButton( MouseButtonEvent @event ) {
		if (@event.InputType == TactileInputType.Press && _hovering) {
			this.LogLine( "Click detected." );
		}
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!_collider.HasTransformer && RenderEntity.TryGetBehaviour( out Transform2Behaviour? t2b ))
			_collider.SetTransform( t2b.HighPrecisionTransform );
		if (!_collisionCalculation.Evaluate()) {
			this.LogWarning( "Collision calculation failed." );
			return;
		}
		if (_hovering == _collisionCalculation.CollisionResult.IsColliding)
			return;
		_hovering = _collisionCalculation.CollisionResult.IsColliding;
		if (_hovering) {
			this.LogLine( "Hovering detected." );
		} else {
			this.LogLine( "Hovering ended." );
		}
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		if (component is UiComponent) {
			_incomingVertices = [ .. Archetype.Collider2Component.ColliderVertices ];
			return true;
		}
		return false;
	}

	protected override void Synchronize() {
		var vertices = _incomingVertices;
		_collider.SetBaseVertices( vertices );
	}

	protected override bool InternalDispose() {
		RenderEntity.ServiceAccess.UserInputEventService.OnMouseButton -= OnMouseButton;
		return base.InternalDispose();
	}
}

public sealed class Collider2Shape : ConvexShapeBase<Vector2<double>, double> {
	public Collider2Shape() : base( Array.Empty<Vector2<double>>().AsSpan() ) { }

	internal new void SetBaseVertices( ReadOnlySpan<Vector2<double>> vertices ) => base.SetBaseVertices( vertices );
	internal new void SetBaseVertices( IEnumerable<Vector2<double>> vertices ) => base.SetBaseVertices( vertices );
	internal new void SetTransform( IMatrixProvider<double>? transform ) => base.SetTransform( transform );
}