using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNDDev {
	class CameraHandler : IMouseEventListener {

		private Start phyTest;

		public CameraHandler( Start t ) {
			phyTest = t;
		}

		public void ButtonHoldHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public void ButtonPressHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
			if( btn == MouseButton.Right )
				phyTest.Window.EventHandler.Mouse.SetLock( phyTest.Window.GLFWWindow, true );
		}

		public void ButtonReleaseHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
			if( btn == MouseButton.Right )
				phyTest.Window.EventHandler.Mouse.SetLock( phyTest.Window.GLFWWindow, false );
		}

		public void MouseDragHandler( IntPtr window, MouseInputEventData data ) {
			phyTest.Render3.Camera.TranformInterface.Rotation = Quaternion.FromAxisAngle( Vector3.UnitY, ( data.LastPositionLocked - data.PositionLocked ).X / (float) ( Math.PI * 2 * 100 ) ) * phyTest.Render3.Camera.TranformInterface.Rotation;
			phyTest.Render3.Camera.TranformInterface.Rotation = Quaternion.FromAxisAngle( phyTest.Render3.Camera.TranformInterface.Rotation.Right, ( data.LastPositionLocked - data.PositionLocked ).Y / (float) ( Math.PI * 2 * 100 ) ) * phyTest.Render3.Camera.TranformInterface.Rotation;
			phyTest.Render3.Camera.TranformInterface.Rotation = phyTest.Render3.Camera.TranformInterface.Rotation.Normalized;
		}

		public void MouseMoveHandler( IntPtr window, MouseInputEventData data ) {
		}

		public void WheelScrollChangeHandler( IntPtr window, float delta, MouseInputEventData data ) {

		}

		internal void CameraUpdate() {
			float speed = 0.05f;
			if( phyTest.Window.EventHandler.Keyboard[ Keys.LeftShift ] )
				speed *= 10;
			if( phyTest.Window.EventHandler.Keyboard[ Keys.W ] ) {
				phyTest.Render3.Camera.TranformInterface.Translation += phyTest.Render3.Camera.TranformInterface.Rotation.Forward * speed;
			}
			if( phyTest.Window.EventHandler.Keyboard[ Keys.A ] ) {
				phyTest.Render3.Camera.TranformInterface.Translation += phyTest.Render3.Camera.TranformInterface.Rotation.Left * speed;
			}
			if( phyTest.Window.EventHandler.Keyboard[ Keys.S ] ) {
				phyTest.Render3.Camera.TranformInterface.Translation += phyTest.Render3.Camera.TranformInterface.Rotation.Backward * speed;
			}
			if( phyTest.Window.EventHandler.Keyboard[ Keys.D ] ) {
				phyTest.Render3.Camera.TranformInterface.Translation += phyTest.Render3.Camera.TranformInterface.Rotation.Right * speed;
			}
			if( phyTest.Window.EventHandler.Keyboard[ Keys.Space ] ) {
				phyTest.Render3.Camera.TranformInterface.Translation += phyTest.Render3.Camera.TranformInterface.Rotation.Up * speed;
			}
			if( phyTest.Window.EventHandler.Keyboard[ Keys.LeftControl ] ) {
				phyTest.Render3.Camera.TranformInterface.Translation += phyTest.Render3.Camera.TranformInterface.Rotation.Down * speed;
			}
		}
	}
}
