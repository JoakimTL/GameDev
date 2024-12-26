using Engine.GLFrameWork;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Tactile {
	/// <summary>
	/// Single Texture Button. If not sized correctly the texture might stretch in weird ways.
	/// </summary>
	public class TextField : UIElement {

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
		private bool selected;

		public string Hint { get; private set; }
		public string Text { get; private set; }

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

		private int caretIndex;
		private ModTranslationSet caretTranslation;

		private ModColorSet caretColor;

		private TextureDisplay underscore;
		private TextureDisplay caretDisplay;

		private TextLabel textLabel;

		private float ratio;
		private Vector2 ratioCaret;

		public TextField( string hint ) {
			Hint = hint;
			Text = "";
			Mem.CollisionMolds.SquareUniform.MoldNew( Data.CollisionModel );

			underscore = new TextureDisplay( Mem.Textures.BlankWhite );
			caretDisplay = new TextureDisplay( Mem.Textures.BlankWhite );

			underscore.Constraints.Set(
				new ConstraintBundle(
					new ModScalingSet( (1, 0) ),
					new ModScalingAddPixel( (0, 5) ),
					new ModTranslationSet( (0, -1) ),
					new ModTranslationAddPixel( (0, 5) )
				)
			);
			caretDisplay.Constraints.Set(
				new ConstraintBundle(
					new ModScalingSet( (0, 0.4f) ),
					new ModScalingAddPixel( (3, 0) ),
					caretTranslation = new ModTranslationSet( 0 ),
					new ModTranslationAdd( (0, -0.4f) ),
					caretColor = new ModColorSet( 1 )
				)
			);
			textLabel = new TextLabel();
			textLabel.TextData.Set( hint, (122, 122, 122, 255) );
			textLabel.Attributes.HorizontalAlignment = HorizontalAlignment.CENTER;
			textLabel.Attributes.VerticalAlignment = VerticalAlignment.CENTER;
			textLabel.Attributes.MaxLength = 2;
			ratio = 1;
			ratioCaret = 1;

			textLabel.SetParent( this );
			underscore.SetParent( this );
			caretDisplay.SetParent( this );

			MouseMoved += OnMouseMoved;
			MouseButtonPressed += OnMousePress;
			MouseButtonReleased += OnMouseReleased;
			Activated += OnActivation;
			Deactivated += OnDeactivation;
			KeyWritten += OnWritten;
			KeyPressed += OnKeyPressed;
			textLabel.TextUpdated += OnLabelUpdate;
			UpdatedSecondActive += OnUpdateSecond;
			TransformChanged += OnTransformed;
			TriggerOnPress = true;
		}

		public TextField( string hint, Vector2 location, Vector2 scale ) : this( hint ) {
			Constraints.Set(
				new ConstraintBundle(
					new ModTranslationSet( location ),
					new ModScalingSet( scale )
				)
			);
		}

		private void OnTransformed( SceneObject<SceneObjectData2> r ) {
			if( TransformInterface.Scale.Y == 0 )
				return;
			ratio = TransformInterface.Scale.X / TransformInterface.Scale.Y;
			ratioCaret = (1f / Math.Max( ratio, 1 ), 1f / Math.Min( ratio, 1 ));
			if( ratio * 2 != textLabel.Attributes.MaxLength )
				textLabel.Attributes.MaxLength = ratio * 2;
		}

		public void SetText( string s ) {
			Text = s;
			if( Text.Length > 0 ) {
				textLabel.TextData.Set( Text, 255 );
			} else {
				textLabel.TextData.Set( Hint, (122, 122, 122, 255) );
			}
		}

		public void SetHint( string s ) {
			Hint = s;
			if( Text.Length > 0 ) {
				textLabel.TextData.Set( Text, 255 );
			} else {
				textLabel.TextData.Set( Hint, (122, 122, 122, 255) );
			}
		}

		private void OnUpdateSecond( MouseInputEventData data ) {
			if( selected ) {
				caretColor.Color = (1, 1, 1, Manager.Time % 1);
			}
		}

		private void OnLabelUpdate() {
			caretTranslation.Translation = textLabel.GetPosition( caretIndex ) * ratioCaret;
		}

		private void OnKeyPressed( IntPtr winPtr, uint id, Keys key, ref bool triggered ) {
			if( selected ) {
				if( key == Keys.Backspace ) {
					if( Text.Length > 0 ) {
						if( caretIndex > 0 ) {
							Text = Text.Remove( caretIndex - 1, 1 );
							caretIndex--;
						}
					} else {
						caretIndex = 0;
					}
				}

				if( key == Keys.Delete ) {
					if( Text.Length > 0 && caretIndex < Text.Length )
						Text = Text.Remove( caretIndex, 1 );
				}

				if( key == Keys.Up ) {
					caretIndex = textLabel.GetIndexAbsolute( textLabel.TransformInterface.GlobalTranslation + ( textLabel.GetPosition( caretIndex ) + new Vector2( 0, textLabel.Attributes.Font.Data.LineHeight - textLabel.Attributes.Font.Data.LineHeight / 2 ) ) * textLabel.TransformInterface.GlobalScale, Manager.UIView.Value );
					caretTranslation.Translation = textLabel.GetPosition( caretIndex ) * ratioCaret;
				}

				if( key == Keys.Down ) {
					caretIndex = textLabel.GetIndexAbsolute( textLabel.TransformInterface.GlobalTranslation + ( textLabel.GetPosition( caretIndex ) - new Vector2( 0, textLabel.Attributes.Font.Data.LineHeight + textLabel.Attributes.Font.Data.LineHeight / 2 ) ) * textLabel.TransformInterface.GlobalScale, Manager.UIView.Value );
					caretTranslation.Translation = textLabel.GetPosition( caretIndex ) * ratioCaret;
				}

				if( key == Keys.Left ) {
					if( caretIndex > 0 ) {
						caretIndex--;
						caretTranslation.Translation = textLabel.GetPosition( caretIndex ) * ratioCaret;
					}
				}

				if( key == Keys.Right ) {
					if( caretIndex < Text.Length ) {
						caretIndex++;
						caretTranslation.Translation = textLabel.GetPosition( caretIndex ) * ratioCaret;
					}
				}

				if( Manager.Window.EventHandler.Keyboard[ Keys.LeftControl ] || Manager.Window.EventHandler.Keyboard[ Keys.RightControl ] ) {
					if( key == Keys.V ) {
						string clip = Clipboard.GetClipboardText( Manager.Window.GLFWWindow );
						Text = Text.Insert( caretIndex, clip );
						caretIndex += clip.Length;
					}
					if( key == Keys.C ) {
						Clipboard.SetClipboardText( Manager.Window.GLFWWindow, Text );
					}
				}

				if( Text.Length > 0 ) {
					textLabel.TextData.Set( Text, 255 );
				} else {
					textLabel.TextData.Set( Hint, (122, 122, 122, 255) );
				}
			}
		}

		private void OnWritten( IntPtr winPtr, uint id, char c, ref bool triggered ) {
			if( selected ) {
				Text = Text.Insert( caretIndex, c.ToString() );
				caretIndex++;
				textLabel.TextData.Set( Text, 255 );
			}
		}

		private void OnMousePress( IntPtr winPtr, uint id, MouseButton button, MouseInputEventData data, ref bool triggered ) {
			selected = false;
			if( CheckCollisionToMouse().Colliding ) {
				pressed = true;
				triggered = TriggerOnPress;
			}
		}

		private void OnMouseReleased( IntPtr winPtr, uint id, MouseButton button, MouseInputEventData data, ref bool triggered ) {
			if( CheckCollisionToMouse().Colliding && pressed ) {
				Click?.Invoke( data );
				triggered = true;
				selected = true;
				if( Text.Length > 0 ) {
					caretIndex = textLabel.GetIndexAbsolute( data.PositionNDCA, Manager.UIView.Value );
					caretTranslation.Translation = textLabel.GetPosition( caretIndex ) * ratioCaret;
				} else {
					caretIndex = 0;
					caretTranslation.Translation = textLabel.GetPosition( caretIndex ) * ratioCaret;
				}
			}
			pressed = false;
			if( selected ) {
				caretDisplay.Activate();
			} else {
				caretDisplay.Deactivate();
			}
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
