using Engine.Module.Render.Input;
using Engine.Physics;

namespace Engine.Standard.Render.Input.Services;

public sealed class MouseColliderProvider : DisposableIdentifiable, IServiceProvider {
	private readonly UserInputEventService _userInputEventService;
	private readonly ProcessedMouseInputProvider _processedMouseInputProvider;
	private readonly Collider2Shape _colliderShapeNDC;
	private readonly Collider2Shape _colliderShapeNDCA;

	public ConvexShapeBase<Vector2<double>, double> ColliderNDC => this._colliderShapeNDC;
	public ConvexShapeBase<Vector2<double>, double> ColliderNDCA => this._colliderShapeNDCA;

	public MouseColliderProvider( UserInputEventService userInputEventService, ProcessedMouseInputProvider processedMouseInputProvider ) {
		this._userInputEventService = userInputEventService;
		this._processedMouseInputProvider = processedMouseInputProvider;
		this._colliderShapeNDC = new();
		this._colliderShapeNDCA = new();
		this._colliderShapeNDC.SetBaseVertices( [ this._processedMouseInputProvider.MouseNDCTranslation ] );
		this._colliderShapeNDCA.SetBaseVertices( [ this._processedMouseInputProvider.MouseNDCAspectTranslation ] );
		this._userInputEventService.OnMouseMoved += OnMouseMoved;
	}

	private void OnMouseMoved( MouseMoveEvent @event ) {
		this._colliderShapeNDC.SetBaseVertices( [ this._processedMouseInputProvider.MouseNDCTranslation ] );
		this._colliderShapeNDCA.SetBaseVertices( [ this._processedMouseInputProvider.MouseNDCAspectTranslation ] );
	}

	protected override bool InternalDispose() {
		this._userInputEventService.OnMouseMoved -= OnMouseMoved;
		return true;
	}
}