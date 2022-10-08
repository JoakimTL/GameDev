using GLFW;
using System.Numerics;

namespace Engine.Rendering.InputHandling;

public class MouseInputEventManager : Identifiable {

	public static readonly int MinButtonIndex = Enum.GetValues<MouseButton>().Min( p => (int) p );
	public static readonly int MaxButtonIndex = Enum.GetValues<MouseButton>().Max( p => (int) p );

	private readonly Window _window;
	private readonly MouseButtonCallback _buttonCallback;
	private readonly MouseEnterCallback _enterCallback;
	private readonly MouseCallback _cursorCallback;
	private readonly MouseCallback _scrollCallback;

	private readonly MouseData _data;
	/// <summary>
	/// The current state of the mouse pointer.
	/// </summary>
	public MouseState State { get; }

	/// <summary>
	/// This event fires the mouse wheel has changed position.
	/// </summary>
	public event MouseScrollHandler? WheelScrolled;
	/// <summary>
	/// This event fires whenever a mouse button is pressed down.
	/// </summary>
	public event MouseButtonHandler? ButtonPressed;
	/// <summary>
	/// This event fires whenever a mouse button is released.
	/// </summary>
	public event MouseButtonHandler? ButtonReleased;
	/// <summary>
	/// If a mouse button is held down for a period of time, warranting a repeat event.
	/// </summary>
	public event MouseButtonHandler? ButtonRepeated;
	/// <summary>
	/// If the cursor is visible, this event will trigger when the mouse is moved.<br/>
	/// This is equivalent to <see cref="IMouseChangeEventListener.OnMouseCursorMove(Window, MouseState)"/>!<br/> 
	/// Use <see cref="MovedHidden"/> for hidden mouse movements.
	/// </summary>
	public event MouseMoveHandler? MovedVisible;
	/// <summary>
	/// If the cursor is hidden, this event will trigger when the mouse is moved.<br/>
	/// This is equivalent to <see cref="IMouseChangeEventListener.OnMouseLockedMove(Window, MouseState)(Window, MouseState)"/>!<br/> 
	/// Use <see cref="MovedVisible"/> for visible mouse movements.
	/// </summary>
	public event MouseMoveHandler? MovedHidden;

	internal MouseInputEventManager( Window window ) {
		this._window = window;
		this._data = new MouseData();
		this.State = new MouseState( this._data );
		this._data.inside = Glfw.GetWindowAttribute( this._window.Pointer, WindowAttribute.MouseHover );

		this._enterCallback = OnEnter;
		this._buttonCallback = OnButton;
		this._cursorCallback = OnCursor;
		this._scrollCallback = OnScroll;

		Glfw.SetCursorEnterCallback( this._window.Pointer, this._enterCallback );
		Glfw.SetMouseButtonCallback( this._window.Pointer, this._buttonCallback );
		Glfw.SetCursorPositionCallback( this._window.Pointer, this._cursorCallback );
		Glfw.SetScrollCallback( this._window.Pointer, this._scrollCallback );

		this.LogLine( $"Lowest button input index: {MinButtonIndex}", Log.Level.NORMAL, ConsoleColor.Blue );
		this.LogLine( $"Highest button input index: {MaxButtonIndex}", Log.Level.NORMAL, ConsoleColor.Blue );
		Log.Line( Glfw.RawMouseMotionSupported() ? "Raw mouse input supported!" : "Raw mouse input not supported!", Log.Level.NORMAL, color: ConsoleColor.Blue );

		AddListener( Resources.GlobalService<ClientInput>() );
	}

	#region Add/Remove
	public void AddListener( IMouseChangeEventListener listener ) {
		MovedVisible += listener.OnMouseCursorMove;
		MovedHidden += listener.OnMouseLockedMove;
	}

	public void AddListener( IWheelScrollChangeEventListener listener ) => WheelScrolled += listener.OnMouseScroll;

	public void AddListener( IButtonChangeEventListener listener ) {
		ButtonPressed += listener.OnButtonPressed;
		ButtonReleased += listener.OnButtonReleased;
		ButtonRepeated += listener.OnButtonRepeat;
	}

	public void AddListener( IMouseEventListener listener ) {
		AddListener( listener as IMouseChangeEventListener );
		AddListener( listener as IWheelScrollChangeEventListener );
		AddListener( listener as IButtonChangeEventListener );
	}

	public void RemoveListener( IMouseChangeEventListener listener ) {
		MovedVisible -= listener.OnMouseCursorMove;
		MovedHidden -= listener.OnMouseLockedMove;
	}

	public void RemoveListener( IWheelScrollChangeEventListener listener ) => WheelScrolled -= listener.OnMouseScroll;

	public void RemoveListener( IButtonChangeEventListener listener ) {
		ButtonPressed -= listener.OnButtonPressed;
		ButtonReleased -= listener.OnButtonReleased;
		ButtonRepeated -= listener.OnButtonRepeat;
	}

	public void RemoveListener( IMouseEventListener listener ) {
		RemoveListener( listener as IMouseChangeEventListener );
		RemoveListener( listener as IWheelScrollChangeEventListener );
		RemoveListener( listener as IButtonChangeEventListener );
	}
	#endregion

	public void SetLock( bool state ) {

		if ( state ) {
			if ( !this._data.locked ) {
				Glfw.SetInputMode( this._window.Pointer, InputMode.Cursor, (int) CursorMode.Disabled );

				Glfw.GetCursorPosition( this._window.Pointer, out double x, out double y );
				this._data.lockedCursor.pos = new Vector2( (float) x, (float) y );
				GetPositionData( this._data.lockedCursor.pos.X, this._data.lockedCursor.pos.Y, out Vector2 ndc, out Vector2 ndca );
				this._data.lockedCursor.posNDC = ndc;
				this._data.lockedCursor.posNDCA = ndca;
				this._data.lastLockedCursor.pos = this._data.lockedCursor.pos;
				this._data.lastLockedCursor.posNDC = ndc;
				this._data.lastLockedCursor.posNDCA = ndca;
			}
		} else {
			if ( this._data.locked ) {
				Glfw.SetInputMode( this._window.Pointer, InputMode.Cursor, (int) CursorMode.Normal );
			}
		}
		this._data.locked = state;
	}

	private void GetPositionData( float x, float y, out Vector2 ndc, out Vector2 ndca ) {
		Vector2 preNDC = new Vector2( x, y ) / this._window.Size.AsFloat * 2;
		//In OGL the y-axis in the NDC space is "flipped". It goes bottom to top rather than top to bottom.
		ndc = new Vector2( preNDC.X - 1, 1 - preNDC.Y );
		ndca = ndc * this._window.AspectRatioVector;
	}

	private void OnEnter( WindowPtr winPtr, bool enter ) => this._data.inside = enter;

	private void OnButton( WindowPtr winPtr, MouseButton button, InputState state, ModifierKeys modifiers ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnButton )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		switch ( state ) {
			case InputState.Press:
				ButtonPressed?.Invoke( button, modifiers, this.State );
				break;
			case InputState.Release:
				ButtonReleased?.Invoke( button, modifiers, this.State );
				break;
			case InputState.Repeat:
				ButtonRepeated?.Invoke( button, modifiers, this.State );
				break;
		}
	}

	private void OnCursor( WindowPtr winPtr, double x, double y ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnCursor )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		if ( this._data.locked ) {
			//Set last location to current, as the current location will be the previous one after this movement.
			this._data.lastLockedCursor.pos = this._data.lockedCursor.pos;
			this._data.lastLockedCursor.posNDC = this._data.lockedCursor.posNDC;
			this._data.lastLockedCursor.posNDCA = this._data.lockedCursor.posNDCA;

			this._data.lockedCursor.pos = new Vector2( (float) x, (float) y );
			GetPositionData( this._data.lockedCursor.pos.X, this._data.lockedCursor.pos.Y, out Vector2 ndc, out Vector2 ndca );
			this._data.lockedCursor.posNDC = ndc;
			this._data.lockedCursor.posNDCA = ndca;

			MovedHidden?.Invoke( this.State );

		} else {
			//Set last location to current, as the current location will be the previous one after this movement.
			this._data.lastCursor.pos = this._data.lockedCursor.pos;
			this._data.lastCursor.posNDC = this._data.lockedCursor.posNDC;
			this._data.lastCursor.posNDCA = this._data.lockedCursor.posNDCA;

			this._data.cursor.pos = new Vector2( (float) x, (float) y );
			GetPositionData( this._data.cursor.pos.X, this._data.cursor.pos.Y, out Vector2 ndc, out Vector2 ndca );
			this._data.cursor.posNDC = ndc;
			this._data.cursor.posNDCA = ndca;

			MovedVisible?.Invoke( this.State );
		}
	}

	private void OnScroll( WindowPtr winPtr, double x, double y ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnScroll )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		WheelScrolled?.Invoke( (float) x, (float) y, this.State );
	}
}

internal class MouseData {
	public readonly bool[] buttons;
	public readonly InternalData cursor;
	public readonly InternalData lockedCursor;
	public readonly InternalData lastCursor;
	public readonly InternalData lastLockedCursor;
	public bool locked = false;
	public bool inside = true;

	public MouseData() {
		this.buttons = new bool[ 8 ];
		this.cursor = new InternalData();
		this.lockedCursor = new InternalData();
		this.lastCursor = new InternalData();
		this.lastLockedCursor = new InternalData();
	}

	public class InternalData {
		public Vector2 pos = new();
		public Vector2 posNDC = new();
		public Vector2 posNDCA = new();
	}
}

public class MouseState {
	private readonly MouseData _data;

	/// <summary>
	/// The current position of the visible cursor. This value does not update if the mouse pointer is locked <b>(not visible)</b>.
	/// </summary>
	public readonly InternalState Visible;
	/// <summary>
	/// The current position of the hidden cursor. This value does not update if the mouse pointer is unlocked <b>(visible)</b>.
	/// </summary>
	public readonly InternalState Hidden;
	/// <summary>
	/// The last position of the visible cursor. This value does not update if the mouse pointer is locked <b>(not visible)</b>.
	/// </summary>
	public readonly InternalState LastVisible;
	/// <summary>
	/// The last position of the hidden cursor. This value does not update if the mouse pointer is unlocked <b>(visible)</b>.
	/// </summary>
	public readonly InternalState LastHidden;

	/// <summary>
	/// Whether the mouse if locked or not (hidden or not).
	/// </summary>
	public bool IsLocked => this._data.locked;

	/// <summary>
	/// Whether the mouse if locked or not (hidden or not).
	/// </summary>
	public bool IsInside => this._data.inside;

	internal MouseState( MouseData data ) {
		this._data = data;
		this.Visible = new InternalState( data.cursor );
		this.Hidden = new InternalState( data.lockedCursor );
		this.LastVisible = new InternalState( data.lastCursor );
		this.LastHidden = new InternalState( data.lastLockedCursor );
	}

	/// <summary>
	/// Returns whether the mouse button is held down or not.
	/// </summary>
	/// <param name="button">The button to check.</param>
	/// <returns>The state of the button, true if pressed, false if not.</returns>
	public bool this[ MouseButton button ] => this._data.buttons[ (int) button ];

	public class InternalState {
		private readonly MouseData.InternalData _internalData;

		internal InternalState( MouseData.InternalData iData ) {
			this._internalData = iData;
		}

		/// <summary>
		/// The position vector for the mouse pointer, this is in pixels.
		/// </summary>
		public Vector2 Position => this._internalData.pos;
		/// <summary>
		/// The Normalized Device Coordinates of the mouse pointer.
		/// </summary>
		public Vector2 PositionNDC => this._internalData.posNDC;
		/// <summary>
		/// The Normalized Device Coordinates with the Aspect Vector from the window multiplied in.<br/>
		/// This causes the absolute values to be greater than 1 if they surpass the axis with shortest resolution on the window.
		/// </summary>
		public Vector2 PositionNDCA => this._internalData.posNDCA;
	}
}
