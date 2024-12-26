using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using Engine.LinearAlgebra;
using Engine.QuickstartKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDev {
	class CameraHandler : IMouseEventListener, IKeyboardEventListener {

		private QuickstartClientRender qscr;

		public CameraHandler( QuickstartClientRender qscr ) {
			this.qscr = qscr;
		}

		public void ButtonHoldHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public void ButtonPressHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public void ButtonReleaseHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public void KeyPressHandler( IntPtr window, Keys key, ModifierKeys mods ) {

		}

		public void KeyReleaseHandler( IntPtr window, Keys key, ModifierKeys mods ) {
			if( key == Keys.C )
				qscr.Window.EventHandler.Mouse.SetLock( qscr.Window.GLFWWindow, !qscr.Window.EventHandler.Mouse.Data.Locked );
			if( key == Keys.Escape )
				qscr.Window.EventHandler.Mouse.SetLock( qscr.Window.GLFWWindow, false );
		}

		public void MouseDragHandler( IntPtr window, MouseInputEventData data ) {
			qscr.Render3.Camera.TranformInterface.Rotation = Quaternion.FromAxisAngle( Vector3.UnitY, ( data.LastPositionLocked - data.PositionLocked ).X / (float) ( Math.PI * 2 * 100 ) ) * qscr.Render3.Camera.TranformInterface.Rotation;
			qscr.Render3.Camera.TranformInterface.Rotation = Quaternion.FromAxisAngle( qscr.Render3.Camera.TranformInterface.Rotation.Right, ( data.LastPositionLocked - data.PositionLocked ).Y / (float) ( Math.PI * 2 * 100 ) ) * qscr.Render3.Camera.TranformInterface.Rotation;
			qscr.Render3.Camera.TranformInterface.Rotation = qscr.Render3.Camera.TranformInterface.Rotation.Normalized;
		}

		public void MouseMoveHandler( IntPtr window, MouseInputEventData data ) {
		}

		public void WheelScrollChangeHandler( IntPtr window, float delta, MouseInputEventData data ) {

		}

		public void WritingHandler( IntPtr window, char c ) {
		}

		internal void CameraUpdate() {
			float speed = 0.05f;
			if( qscr.Window.EventHandler.Keyboard[ Keys.LeftShift ] )
				speed *= 10;
			if( qscr.Window.EventHandler.Keyboard[ Keys.W ] ) {
				qscr.Render3.Camera.TranformInterface.Translation += qscr.Render3.Camera.TranformInterface.Rotation.Forward * speed;
			}
			if( qscr.Window.EventHandler.Keyboard[ Keys.A ] ) {
				qscr.Render3.Camera.TranformInterface.Translation += qscr.Render3.Camera.TranformInterface.Rotation.Left * speed;
			}
			if( qscr.Window.EventHandler.Keyboard[ Keys.S ] ) {
				qscr.Render3.Camera.TranformInterface.Translation += qscr.Render3.Camera.TranformInterface.Rotation.Backward * speed;
			}
			if( qscr.Window.EventHandler.Keyboard[ Keys.D ] ) {
				qscr.Render3.Camera.TranformInterface.Translation += qscr.Render3.Camera.TranformInterface.Rotation.Right * speed;
			}
			if( qscr.Window.EventHandler.Keyboard[ Keys.Space ] ) {
				qscr.Render3.Camera.TranformInterface.Translation += qscr.Render3.Camera.TranformInterface.Rotation.Up * speed;
			}
			if( qscr.Window.EventHandler.Keyboard[ Keys.LeftControl ] ) {
				qscr.Render3.Camera.TranformInterface.Translation += qscr.Render3.Camera.TranformInterface.Rotation.Down * speed;
			}
		}
	}
}
