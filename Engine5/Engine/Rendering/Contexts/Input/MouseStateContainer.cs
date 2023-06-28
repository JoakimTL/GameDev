using Engine.Rendering.Contexts.Input.StateStructs;
using Engine.Rendering.OGL;
using GlfwBinding.Enums;

namespace Engine.Rendering.Contexts.Input;

public class MouseStateContainer : Identifiable {

    public readonly MousePointer.MovementState VisibleCursor;
    public readonly MousePointer.MovementState LockedCursor;
    public bool IsLocked { get; private set; }
    public bool IsInside { get; internal set; }
    private readonly bool[] _buttons;
    public Point2d CurrentScrollWheelPosition { get; private set; }
    public Point2d PreviousScrollWheelPosition { get; private set; }

    public MouseStateContainer() {
        VisibleCursor = new();
        LockedCursor = new();
        _buttons = new bool[ Services.InputEventService.ButtonBooleanArrayLength ];
    }

    public bool this[ MouseButton button ] {
        get => _buttons[ (int) button ];
        internal set => _buttons[ (int) button ] = value;
    }

    public MouseButtonState MouseButtonState => new( _buttons[0], _buttons[1], _buttons[2], _buttons[3], _buttons[4], _buttons[5], _buttons[6], _buttons[7] );

    internal void ScrolledWheel( Point2d axis ) {
        PreviousScrollWheelPosition = CurrentScrollWheelPosition;
        CurrentScrollWheelPosition = new( CurrentScrollWheelPosition.X + axis.X, CurrentScrollWheelPosition.Y + axis.Y );
    }

    internal void MouseMoved( Point2d newPixelPosition, Point2d newNdcaPosition ) {
        var cursor = IsLocked ? VisibleCursor : LockedCursor;
        cursor.PreviousPointerState.Set( cursor.CurrentPointerState );
        cursor.CurrentPointerState.Set( newPixelPosition, newNdcaPosition );
    }

    internal bool SetLock( Window window, bool state ) {
        if ( state == IsLocked )
            return false;
        IsLocked = state;
        if ( state ) {
            InputUtilities.SetInputMode( window.Pointer, InputMode.Cursor, (int) CursorMode.Disabled );
            //InputUtilities.GetCursorPosition( _window.Pointer, out double x, out double y );
            //_data.lockedCursor.pos = new( (float) x, (float) y );
            //GetPositionData( _data.lockedCursor.pos, out Vector2 ndc, out Vector2 ndca );
            //_data.lockedCursor.posNDC = ndc;
            //_data.lockedCursor.posNDCA = ndca;
            //_data.lastLockedCursor.pos = _data.lockedCursor.pos;
            //_data.lastLockedCursor.posNDC = ndc;
            //_data.lastLockedCursor.posNDCA = ndca;
        } else {
            InputUtilities.SetInputMode( window.Pointer, InputMode.Cursor, (int) CursorMode.Normal );
        }
        return true;
    }
}
