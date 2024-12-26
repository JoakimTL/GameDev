using Engine.GLFrameWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public interface IUIEventListener {
		void KeyReleaseHandler( IntPtr window, Keys key, ModifierKeys mods, UIEvent eventData );
		void KeyPressHandler( IntPtr window, Keys key, ModifierKeys mods, UIEvent eventData );
		void KeyHoldHandler( IntPtr window, Keys key, ModifierKeys mods, UIEvent eventData );
		void WritingHandler( IntPtr window, char c, UIEvent eventData );
		void ButtonReleaseHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data, UIEvent eventData );
		void ButtonPressHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data, UIEvent eventData );
		void ButtonHoldHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data, UIEvent eventData );
		void MouseMoveHandler( IntPtr window, MouseInputEventData data, UIEvent eventData );
		void MouseDragHandler( IntPtr window, MouseInputEventData data, UIEvent eventData );
		void WheelScrollChangeHandler( IntPtr window, float delta, MouseInputEventData data, UIEvent eventData );
	}
}
