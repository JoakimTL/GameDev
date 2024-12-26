using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using System;

namespace Engine.Graphics.Objects.Default.EventListeners {
	public class VirtualEventListener : IEventListener {
		public virtual void ButtonHoldHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public virtual void ButtonPressHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public virtual void ButtonReleaseHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public virtual void KeyHoldHandler( IntPtr window, Keys key, ModifierKeys mods ) {
		}

		public virtual void KeyPressHandler( IntPtr window, Keys key, ModifierKeys mods ) {
		}

		public virtual void KeyReleaseHandler( IntPtr window, Keys key, ModifierKeys mods ) {
		}

		public virtual void MouseDragHandler( IntPtr window, MouseInputEventData data ) {
		}

		public virtual void MouseMoveHandler( IntPtr window, MouseInputEventData data ) {
		}

		public virtual void WheelScrollChangeHandler( IntPtr window, float delta, MouseInputEventData data ) {
		}

		public virtual void WindowFocusHandler( IntPtr window, bool focused ) {
		}

		public virtual void WindowResizeHandler( IntPtr window, int width, int height ) {
		}

		public virtual void WritingHandler( IntPtr window, char c ) {
		}
	}
}
