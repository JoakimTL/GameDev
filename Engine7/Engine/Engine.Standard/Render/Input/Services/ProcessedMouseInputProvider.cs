using Engine.Module.Render;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Input.Services;
public sealed class ProcessedMouseInputProvider : DisposableIdentifiable, IRenderServiceProvider {
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
		Vector2<double> windowSize = _windowService.Window.Size.CastSaturating<int, double>();
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
