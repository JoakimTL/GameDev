using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using System;

namespace CollisionDetDev3 {
	public class CollisionModKeyHandler : IKeyboardEventListener, IMouseEventListener {
		private Entry entry;

		public CollisionModKeyHandler( Entry entry ) {
			this.entry = entry;
			entry.Window.EventHandler.Keyboard.Add( this );
			entry.Window.EventHandler.Mouse.Add( this );
		}

		public void ButtonPressHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public void ButtonReleaseHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public void MouseDragHandler( IntPtr window, MouseInputEventData data ) {
		}

		public void MouseMoveHandler( IntPtr window, MouseInputEventData data ) {
		}

		public void WheelScrollChangeHandler( IntPtr window, float delta, MouseInputEventData data ) {
			if( delta < 0 ) {
				entry.DecreaseView();
			} else
				entry.IncreaseView();
		}

		public void KeyPressHandler( IntPtr window, Keys key, ModifierKeys mods ) {
			if( key == Keys.U )
				entry.Unlock();
			if( key == Keys.L )
				entry.ToggleViewLast();
			if( key == Keys.P )
				entry.LogOne();
		}

		public void KeyReleaseHandler( IntPtr window, Keys key, ModifierKeys mods ) {
		}

		public void WritingHandler( IntPtr window, char c ) {
		}
	}
}