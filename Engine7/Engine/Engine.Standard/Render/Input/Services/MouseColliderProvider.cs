using Engine.Module.Render;
using Engine.Module.Render.Input;
using Engine.Physics;

namespace Engine.Standard.Render.Input.Services;

public sealed class MouseColliderProvider : DisposableIdentifiable, IRenderServiceProvider {
	private readonly UserInputEventService _userInputEventService;
	private readonly ProcessedMouseInputProvider _processedMouseInputProvider;
	private readonly Collider2Shape _colliderShapeNDC;
	private readonly Collider2Shape _colliderShapeNDCA;

	public ConvexShapeBase<Vector2<double>, double> ColliderNDC => _colliderShapeNDC;
	public ConvexShapeBase<Vector2<double>, double> ColliderNDCA => _colliderShapeNDCA;

	public MouseColliderProvider( UserInputEventService userInputEventService, ProcessedMouseInputProvider processedMouseInputProvider) {
		this._userInputEventService = userInputEventService;
		this._processedMouseInputProvider = processedMouseInputProvider;
		_colliderShapeNDC = new();
		_colliderShapeNDCA = new();
		_colliderShapeNDC.SetBaseVertices( [ _processedMouseInputProvider.MouseNDCTranslation ] );
		_colliderShapeNDCA.SetBaseVertices( [ _processedMouseInputProvider.MouseNDCAspectTranslation ] );
		_userInputEventService.OnMouseMoved += OnMouseMoved;
	}

	private void OnMouseMoved( MouseMoveEvent @event ) {
		_colliderShapeNDC.SetBaseVertices( [ _processedMouseInputProvider.MouseNDCTranslation ] );
		_colliderShapeNDCA.SetBaseVertices( [ _processedMouseInputProvider.MouseNDCAspectTranslation ] );
	}

	protected override bool InternalDispose() {
		_userInputEventService.OnMouseMoved -= OnMouseMoved;
		return true;
	}
}