using Engine.Module.Entities.Render;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.Services;
using Engine.Physics;
using Engine.Standard.Render.Entities.Behaviours;

namespace Engine.Standard.Render.Input.Services;
public sealed class ProcessedMouseInputProvider : DisposableIdentifiable, IRenderEntityServiceProvider {
	private readonly UserInputEventService _userInputEventService;
	private readonly WindowService _windowService;

	public Vector2<double> MousePixelTranslation { get; private set; }
	public Vector2<double> MouseWindowTranslation { get; private set; }
	public Vector2<double> MouseNDCTranslation { get; private set; }
	public Vector2<double> MouseNDCAspectTranslation { get; private set; }
	private readonly bool[] _pressedMouseButtons;

	public ProcessedMouseInputProvider( UserInputEventService userInputEventService, WindowService windowService ) {
		this._userInputEventService = userInputEventService;
		this._windowService = windowService;
		_pressedMouseButtons = new bool[ 64 ];
		_userInputEventService.OnMouseMoved += OnMouseMoved;
		_userInputEventService.OnMouseButton += OnMouseButton;
	}

	public IReadOnlyList<bool> PressedMouseButtons => _pressedMouseButtons;
	public bool this[ MouseButton button ] => _pressedMouseButtons[ (int) button ];

	private void OnMouseMoved( MouseMoveEvent @event ) {
		var windowSize = _windowService.Window.Size.CastSaturating<int, double>();
		MousePixelTranslation = @event.Position;
		MouseWindowTranslation = MousePixelTranslation.DivideEntrywise( windowSize );
		MouseNDCTranslation = new Vector2<double>( MouseWindowTranslation.X - .5, .5 - MouseWindowTranslation.Y ) * 2;
		MouseNDCAspectTranslation = MouseNDCTranslation.MultiplyEntrywise( _windowService.Window.AspectRatioVector.CastSaturating<float, double>() );
	}

	private void OnMouseButton( MouseButtonEvent @event ) {
		_pressedMouseButtons[ (int) @event.Button ] = @event.InputType == TactileInputType.Press;
	}

	protected override bool InternalDispose() {
		_userInputEventService.OnMouseMoved -= OnMouseMoved;
		return true;
	}
}

public sealed class MouseColliderProvider : DisposableIdentifiable, IRenderEntityServiceProvider {
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