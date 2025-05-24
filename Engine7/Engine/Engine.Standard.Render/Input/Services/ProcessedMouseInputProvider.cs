using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Input.Services;
public sealed class ProcessedMouseInputProvider : DisposableIdentifiable, IServiceProvider {
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
		this._pressedMouseButtons = new bool[ 64 ];
		this._userInputEventService.OnMouseMoved += OnMouseMoved;
		this._userInputEventService.OnMouseButton += OnMouseButton;
	}

	public IReadOnlyList<bool> PressedMouseButtons => this._pressedMouseButtons;
	public bool this[ MouseButton button ] => this._pressedMouseButtons[ (int) button ];

	private void OnMouseMoved( MouseMoveEvent @event ) {
		Vector2<double> windowSize = this._windowService.Window.Size.CastSaturating<int, double>();
		this.MousePixelTranslation = @event.Position;
		this.MouseWindowTranslation = this.MousePixelTranslation.DivideEntrywise( windowSize );
		this.MouseNDCTranslation = new Vector2<double>( this.MouseWindowTranslation.X - .5, .5 - this.MouseWindowTranslation.Y ) * 2;
		this.MouseNDCAspectTranslation = this.MouseNDCTranslation.MultiplyEntrywise( this._windowService.Window.AspectRatioVector.CastSaturating<float, double>() );
	}

	private void OnMouseButton( MouseButtonEvent @event ) {
		this._pressedMouseButtons[ (int) @event.Button ] = @event.InputType == TactileInputType.Press;
	}

	protected override bool InternalDispose() {
		this._userInputEventService.OnMouseMoved -= OnMouseMoved;
		return true;
	}
}
