using Engine.GLFrameWork;
using Engine.LMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects {
	#region Delegates
	public delegate void WindowResizeEvent( IntPtr window, int width, int height );
	public delegate void KeyChangeEvent( IntPtr window, Keys key, ModifierKeys mods );
	public delegate void WritingEvent( IntPtr window, char c );
	public delegate void ButtonChangeEvent( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data );
	public delegate void MouseChangeEvent( IntPtr window, MouseInputEventData data );
	#endregion

	#region Interfaces
	public delegate void WheelScrollChangeEvent( IntPtr window, float delta, MouseInputEventData data ); public interface IWindowResizeEventListener {
		void WindowResizeHandler( IntPtr window, int width, int height );
	}

	public interface IKeyChangeEventListener {
		void KeyReleaseHandler( IntPtr window, Keys key, ModifierKeys mods );
		void KeyPressHandler( IntPtr window, Keys key, ModifierKeys mods );
		void KeyHoldHandler( IntPtr window, Keys key, ModifierKeys mods );
	}

	public interface IWritingEventListener {
		void WritingHandler( IntPtr window, char c );
	}

	public interface IButtonChangeEventListener {
		void ButtonReleaseHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data );
		void ButtonPressHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data );
		void ButtonHoldHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data );
	}

	public interface IMouseChangeEventListener {
		void MouseMoveHandler( IntPtr window, MouseInputEventData data );
		void MouseDragHandler( IntPtr window, MouseInputEventData data );
	}

	public interface IWheelScrollChangeEventListener {
		void WheelScrollChangeHandler( IntPtr window, float delta, MouseInputEventData data );
	}

	public interface IKeyboardEventListener : IKeyChangeEventListener, IWritingEventListener { }
	public interface IMouseEventListener : IButtonChangeEventListener, IMouseChangeEventListener, IWheelScrollChangeEventListener { }
	public interface IEventListener : IWindowResizeEventListener, IKeyboardEventListener, IMouseEventListener { }
	#endregion

	public class InputEventHandler {

		public readonly MouseInputEventManager Mouse;
		public readonly KeyboardInputEventManager Keyboard;
		public readonly WindowInputEventManager Window;

		public InputEventHandler() {
			Mouse = new MouseInputEventManager();
			Keyboard = new KeyboardInputEventManager();
			Window = new WindowInputEventManager();
		}

		internal void Bind( Window window ) {

			GLFW.SetKeyCallback( window, Keyboard.KeyboardKeyCallback );
			GLFW.SetCharCallback( window, Keyboard.KeyboardCharCallback );
			GLFW.SetCursorPositionCallback( window, Mouse.MouseMovedCallback );
			GLFW.SetMouseButtonCallback( window, Mouse.MouseButtonCallback );
			GLFW.SetScrollCallback( window, Mouse.MouseWheelCallback );
			GLFW.SetWindowSizeCallback( window, Window.WindowResizeCallback );
			//		Glfw.SetCursorEnterCallback( window, PlaceHolder1 );
			//		Glfw.SetDropCallback( window, PlaceHolder2 );
			//		Glfw.SetFramebufferSizeCallback( window, PlaceHolder3 );
			//		Glfw.SetCloseCallback( window, PlaceHolder3 );
			//		Glfw.SetCharModsCallback( window, PlaceHolder4 );
			//		Glfw.SetJoystickCallback( PlaceHolder5 );
			//		Glfw.SetMonitorCallback( PlaceHolder6 );
			//		Glfw.SetWindowContentScaleCallback( window, PlaceHolder7 );
			//		Glfw.SetWindowFocusCallback( window, PlaceHolder8 );
			//		Glfw.SetWindowIconifyCallback( window, PlaceHolder9 );
			//		Glfw.SetWindowMaximizeCallback( window, Window.WindowMaximizeCallback );
			//		Glfw.SetWindowPositionCallback( window, PlaceHolder11 );
			//		Glfw.SetWindowRefreshCallback( window, PlaceHolder12 );

		}
	}

	#region Keyboard Events
	public class KeyboardInputEventManager {

		public KeyCallback KeyboardKeyCallback {
			get; private set;
		}

		public CharCallback KeyboardCharCallback {
			get; private set;
		}

		private readonly bool[] keys;

		public KeyboardInputEventManager() {
			keys = new bool[ 4096 ];

			KeyboardKeyCallback = new KeyCallback( KeyEvent );
			KeyboardCharCallback = new CharCallback( WritingEvent );
		}

		public bool this[ Keys key ] {
			get {
				return keys[ (int) key ];
			}
		}

		#region Events

		public event KeyChangeEvent KeyPress;
		public event KeyChangeEvent KeyRelease;
		public event KeyChangeEvent KeyHold;
		public event WritingEvent Writing;

		public void Add( IWritingEventListener e ) {
			Writing += e.WritingHandler;
		}

		public void Add( IKeyChangeEventListener e ) {
			KeyPress += e.KeyPressHandler;
			KeyRelease += e.KeyReleaseHandler;
			KeyHold += e.KeyHoldHandler;
		}

		public void Add( IKeyboardEventListener e ) {
			Add( (IWritingEventListener) e );
			Add( (IKeyChangeEventListener) e );
		}

		public void Remove( IWritingEventListener e ) {
			Writing -= e.WritingHandler;
		}

		public void Remove( IKeyChangeEventListener e ) {
			KeyPress -= e.KeyPressHandler;
			KeyRelease -= e.KeyReleaseHandler;
			KeyHold -= e.KeyHoldHandler;
		}

		public void Remove( IKeyboardEventListener e ) {
			Remove( (IWritingEventListener) e );
			Remove( (IKeyChangeEventListener) e );
		}

		#endregion

		private void KeyEvent( IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods ) {
			if( key < 0 )
				return;

			switch( state ) {
				case InputState.Press:
					keys[ (int) key ] = true;
					KeyPress?.Invoke( window, key, mods );
					break;
				case InputState.Release:
					keys[ (int) key ] = false;
					KeyRelease?.Invoke( window, key, mods );
					break;
				case InputState.Repeat:
					KeyHold?.Invoke( window, key, mods );
					break;
			}
		}

		private void WritingEvent( IntPtr window, uint codePoint ) {
			Writing?.Invoke( window, (char) codePoint );
		}

	}
	#endregion

	#region Mouse Events
	public class MouseInputEventManager {

		public MouseCallback MouseMovedCallback {
			get; private set;
		}

		public MouseButtonCallback MouseButtonCallback {
			get; private set;
		}

		public MouseCallback MouseWheelCallback {
			get; private set;
		}

		private readonly MouseData mouseData;
		public MouseInputEventData Data {
			get; private set;
		}
		private IView movingView;

		public MouseInputEventManager() {
			movingView = null;
			mouseData = new MouseData();
			Data = new MouseInputEventData( mouseData );

			MouseMovedCallback = new MouseCallback( MouseMoved );
			MouseButtonCallback = new MouseButtonCallback( ButtonEvent );
			MouseWheelCallback = new MouseCallback( WheelEvent );
		}

		#region Events

		public event ButtonChangeEvent ButtonPress;
		public event ButtonChangeEvent ButtonRelease;
		public event ButtonChangeEvent ButtonHold;
		public event MouseChangeEvent MouseMove;
		public event MouseChangeEvent MouseDrag;
		public event WheelScrollChangeEvent WheelScroll;

		public void Add( IMouseChangeEventListener e ) {
			MouseMove += e.MouseMoveHandler;
			MouseDrag += e.MouseDragHandler;
		}

		public void Add( IButtonChangeEventListener e ) {
			ButtonPress += e.ButtonPressHandler;
			ButtonRelease += e.ButtonReleaseHandler;
			ButtonHold += e.ButtonHoldHandler;
		}

		public void Add( IWheelScrollChangeEventListener e ) {
			WheelScroll += e.WheelScrollChangeHandler;
		}

		public void Add( IMouseEventListener e ) {
			Add( (IMouseChangeEventListener) e );
			Add( (IButtonChangeEventListener) e );
			Add( (IWheelScrollChangeEventListener) e );
		}

		public void Remove( IMouseChangeEventListener e ) {
			MouseMove -= e.MouseMoveHandler;
			MouseDrag -= e.MouseDragHandler;
		}

		public void Remove( IButtonChangeEventListener e ) {
			ButtonPress -= e.ButtonPressHandler;
			ButtonRelease -= e.ButtonReleaseHandler;
			ButtonHold -= e.ButtonHoldHandler;
		}

		public void Remove( IWheelScrollChangeEventListener e ) {
			WheelScroll -= e.WheelScrollChangeHandler;
		}

		public void Remove( IMouseEventListener e ) {
			Remove( (IMouseChangeEventListener) e );
			Remove( (IButtonChangeEventListener) e );
			Remove( (IWheelScrollChangeEventListener) e );
		}

		#endregion

		public void SetCamera( IView camera ) {
			movingView = camera;
		}

		public void SetLock( Window window, bool state ) {
			if( state ) {
				if( !mouseData.Locked ) {
					GLFW.SetInputMode( window, InputMode.Cursor, (int) CursorMode.Disabled );
					mouseData.LastPositionLocked = mouseData.Position;
					mouseData.LastPositionLockedNDC = mouseData.PositionNDC;
					mouseData.LastPositionLockedNDCA = mouseData.PositionNDCA;
					mouseData.LastPositionLockedNDCAM = mouseData.PositionNDCAM;
					mouseData.PositionLocked = mouseData.Position;
					mouseData.PositionLockedNDC = mouseData.PositionNDC;
					mouseData.PositionLockedNDCA = mouseData.PositionNDCA;
					mouseData.PositionLockedNDCAM = mouseData.PositionNDCAM;
				}
			} else {
				if( mouseData.Locked ) {
					GLFW.SetInputMode( window, InputMode.Cursor, (int) CursorMode.Normal );
				}
			}
			mouseData.Locked = state;
		}

		public bool this[ MouseButton btn ] {
			get {
				return mouseData.Buttons[ (int) btn ];
			}
		}

		#region Event Handling

		private void MouseMoved( IntPtr window, double x, double y ) {
			if( mouseData.Locked ) {
				Vector2 lastPositionLocked = mouseData.PositionLocked;
				mouseData.PositionLocked = new Vector2( (float) x, (float) y );
				mouseData.LastPositionLocked = lastPositionLocked;
				GLWindow dw = MemLib.Mem.Windows[ window ];
				if( window != null ) {
					Vector2 lastPositionLockedNDC = mouseData.PositionLockedNDC;
					mouseData.PositionLockedNDC = new Vector2( ( mouseData.PositionLocked.X / dw.Size.X ) * 2 - 1, 1 - ( mouseData.PositionLocked.Y / dw.Size.Y ) * 2 );
					mouseData.LastPositionLockedNDC = lastPositionLockedNDC;

					Vector2 lastPositionLockedNDCA = mouseData.PositionLockedNDCA;
					Vector2 lastPositionLockedNDCAM = mouseData.PositionLockedNDCAM;
					Vector4 a = new Vector4( mouseData.PositionLockedNDC.X * dw.AspectRatioVector.X, mouseData.PositionLockedNDC.Y * dw.AspectRatioVector.Y, 0, 1 );
					mouseData.PositionLockedNDCA = a.XY;
					if( movingView != null )
						a *= movingView.VPMatrix;
					mouseData.PositionLockedNDCAM = a.XY;
					mouseData.LastPositionLockedNDCA = lastPositionLockedNDCA;
					mouseData.LastPositionLockedNDCAM = lastPositionLockedNDCAM;

					MouseDrag?.Invoke( window, Data );
				}
			} else {
				Vector2 lastPosition = mouseData.Position;
				mouseData.Position = new Vector2( (float) x, (float) y );
				mouseData.LastPosition = lastPosition;
				GLWindow dw = MemLib.Mem.Windows[ window ];
				if( window != null ) {
					Vector2 lastPositionNDC = mouseData.PositionNDC;
					mouseData.PositionNDC = new Vector2( ( mouseData.Position.X / dw.Size.X ) * 2 - 1, 1 - ( mouseData.Position.Y / dw.Size.Y ) * 2 );
					mouseData.LastPositionNDC = lastPositionNDC;

					Vector2 lastPositionNDCA = mouseData.PositionNDCA;
					Vector2 lastPositionNDCAM = mouseData.PositionNDCAM;
					Vector4 a = new Vector4( mouseData.PositionNDC.X * dw.AspectRatioVector.X, mouseData.PositionNDC.Y * dw.AspectRatioVector.Y, 0, 1 );
					mouseData.PositionNDCA = a.XY;
					if( movingView != null )
						a *= movingView.VPMatrix;
					mouseData.PositionNDCAM = a.XY;
					mouseData.LastPositionNDCA = lastPositionNDCA;
					mouseData.LastPositionNDCAM = lastPositionNDCAM;

					MouseMove?.Invoke( window, Data );
				}
			}
		}

		private void WheelEvent( IntPtr window, double x, double y ) {
			WheelScroll?.Invoke( window, (float) y, Data );
		}

		private void ButtonEvent( IntPtr window, MouseButton button, InputState state, ModifierKeys modifiers ) {
			switch( state ) {
				case InputState.Press:
					mouseData.Buttons[ (int) button ] = true;
					ButtonPress?.Invoke( window, button, modifiers, Data );
					break;
				case InputState.Release:
					mouseData.Buttons[ (int) button ] = false;
					ButtonRelease?.Invoke( window, button, modifiers, Data );
					break;
				case InputState.Repeat:
					ButtonHold?.Invoke( window, button, modifiers, Data );
					break;
			}
		}

		#endregion

	}

	internal class MouseData {

		public bool[] Buttons = new bool[ 8 ];
		public Vector2 Position;
		public Vector2 PositionNDC;
		public Vector2 PositionNDCA;
		public Vector2 PositionNDCAM;
		public Vector2 LastPosition;
		public Vector2 LastPositionNDC;
		public Vector2 LastPositionNDCA;
		public Vector2 LastPositionNDCAM;
		public Vector2 PositionLocked;
		public Vector2 PositionLockedNDC;
		public Vector2 PositionLockedNDCA;
		public Vector2 PositionLockedNDCAM;
		public Vector2 LastPositionLocked;
		public Vector2 LastPositionLockedNDC;
		public Vector2 LastPositionLockedNDCA;
		public Vector2 LastPositionLockedNDCAM;
		public bool Locked;

	}

	public class MouseInputEventData {

		private readonly MouseData mouseData;

		public Vector2 Position => mouseData.Position;
		public Vector2 PositionNDC => mouseData.PositionNDC;
		public Vector2 PositionNDCA => mouseData.PositionNDCA;
		public Vector2 PositionNDCAM => mouseData.PositionNDCAM;
		public Vector2 LastPosition => mouseData.LastPosition;
		public Vector2 LastPositionNDC => mouseData.LastPositionNDC;
		public Vector2 LastPositionNDCA => mouseData.LastPositionNDCA;
		public Vector2 LastPositionNDCAM => mouseData.LastPositionNDCAM;
		public Vector2 PositionLocked => mouseData.PositionLocked;
		public Vector2 PositionLockedNDC => mouseData.PositionLockedNDC;
		public Vector2 PositionLockedNDCA => mouseData.PositionLockedNDCA;
		public Vector2 PositionLockedNDCAM => mouseData.PositionLockedNDCAM;
		public Vector2 LastPositionLocked => mouseData.LastPositionLocked;
		public Vector2 LastPositionLockedNDC => mouseData.LastPositionLockedNDC;
		public Vector2 LastPositionLockedNDCA => mouseData.LastPositionLockedNDCA;
		public Vector2 LastPositionLockedNDCAM => mouseData.LastPositionLockedNDCAM;
		public bool Locked => mouseData.Locked;

		public bool this[ MouseButton button ] {
			get => mouseData.Buttons[ (int) button ];
		}

		internal MouseInputEventData( MouseData data ) {
			this.mouseData = data;
		}

		public override string ToString() {
			return $"|{Position}||{PositionNDC}||{PositionNDCA}||{PositionNDCAM}|\n|{LastPosition}||{LastPositionNDC}||{LastPositionNDCA}||{LastPositionNDCAM}|\n{Locked}\n|{PositionLocked}||{PositionLockedNDC}||{PositionLockedNDCA}||{PositionLockedNDCAM}|\n|{LastPositionLocked}||{LastPositionLockedNDC}||{LastPositionLockedNDCA}||{LastPositionLockedNDCAM}|";
		}

	}
	#endregion

	#region Window Events
	public class WindowInputEventManager {

		public SizeCallback WindowResizeCallback {
			get; private set;
		}
		public WindowMaximizedCallback WindowMaximizeCallback { get; internal set; }

		public WindowInputEventManager() {
			WindowResizeCallback = new SizeCallback( WindowResized );
		}

		#region Events

		public event WindowResizeEvent Resize;

		public void Add( IWindowResizeEventListener e ) {
			Resize += e.WindowResizeHandler;
		}

		public void Remove( IWindowResizeEventListener e ) {
			Resize -= e.WindowResizeHandler;
		}

		#endregion

		private void WindowResized( IntPtr window, int width, int height ) {
			Resize?.Invoke( window, width, height );
		}
	}
	#endregion
}
