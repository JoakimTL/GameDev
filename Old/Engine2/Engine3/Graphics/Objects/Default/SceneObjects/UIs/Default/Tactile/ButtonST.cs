using Engine.GLFrameWork;
using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Tactile {
	/// <summary>
	/// Single Texture Button. If not sized correctly the texture might stretch in weird ways.
	/// </summary>
	public class ButtonST : UIElement {

		/// <summary>
		/// Indicates whether the mouse pointer is hovering over the button or not. Will not update unless UpdateHover is true.
		/// </summary>
		public bool Hovering { get; private set; }

		/// <summary>
		/// Indicates whether the button will check for hovering during updates.
		/// </summary>
		public bool TrackCollision { get; protected set; }

		/// <summary>
		/// Indicates whether the mouse button click will trigger the internal event handler trigger (no other ui elements will be checked if triggered.)
		/// </summary>
		public bool TriggerOnPress { get; protected set; }

		private bool pressed;

		public delegate void MouseEventHandler( MouseInputEventData data );
		/// <summary>
		/// Triggers when the button has had the mouse button pressed on and released on.
		/// </summary>
		public event MouseEventHandler Click;
		/// <summary>
		/// If TrackCollision is true this event fires when the mouse pointer enters the buttons collision box.<br></br><b>Does not trigger when the Activate event fires.</b>
		/// </summary>
		public event Action Enter;
		/// <summary>
		/// If TrackCollision is true this event fires when the mouse pointer leaves the buttons collision box.<br></br><b>Does not trigger when the Activate event fires.</b>
		/// </summary>
		public event Action Leave;

		public ButtonST( Texture texture ) {
			Mesh = Mem.Mesh2.Square;
			Mem.CollisionMolds.SquareUniform.MoldNew( Data.CollisionModel );
			Material = new Material( $"Button[{ID}]" ).AddTexture( OpenGL.TextureUnit.Texture0, texture );
			MouseMoved += OnMouseMoved;
			MouseButtonPressed += OnMousePress;
			MouseButtonReleased += OnMouseReleased;
			Activated += OnActivation;
			Deactivated += OnDeactivation;
			TriggerOnPress = true;
		}

		private void OnMousePress( IntPtr winPtr, uint id, MouseButton button, MouseInputEventData data, ref bool triggered ) {
			if( CheckCollisionToMouse().Colliding ) {
				pressed = true;
				triggered = TriggerOnPress;
			}
		}

		private void OnMouseReleased( IntPtr winPtr, uint id, MouseButton button, MouseInputEventData data, ref bool triggered ) {
			if( CheckCollisionToMouse().Colliding && pressed ) {
				Click?.Invoke( data );
				triggered = true;
			}
			pressed = false;
		}

		private void OnMouseMoved( IntPtr winPtr, uint id, MouseInputEventData data, ref bool triggered ) {
			if( TrackCollision ) {
				bool hover = CheckCollisionToMouse().Colliding;
				if( hover != Hovering )
					if( hover ) {
						Enter?.Invoke();
					} else {
						Leave?.Invoke();
					}
				Hovering = hover;
			}
		}

		private void OnActivation() {
			if( TrackCollision ) {
				bool hover = CheckCollisionToMouse().Colliding;
				if( hover != Hovering )
					if( hover ) {
						Enter?.Invoke();
					} else {
						Leave?.Invoke();
					}
				Hovering = hover;
			}
		}

		private void OnDeactivation() {
			Hovering = false;
			pressed = false;
		}
	}
}
