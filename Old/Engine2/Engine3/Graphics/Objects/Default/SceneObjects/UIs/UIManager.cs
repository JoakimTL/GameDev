using Engine.GLFrameWork;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics.D2;
using Engine.Physics.D2.Shapes;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public class UIManager : IEventListener {

		private readonly Scene<SceneObjectData2> scene;
		private readonly Dictionary<uint, UIElement> elements;
		private readonly Dictionary<string, IUIEventListener> eventListeners;

		public UIStencilManager Stencils { get; private set; }
		public GLWindow Window { get; private set; }
		public Clock32 UIClock { get; private set; }
		public float DeltaTime { get; private set; }
		public float Time { get; private set; }
		public MutableSinglet<View2> UIView { get; private set; }
		public Shape2PointTransformless MousePointer { get; private set; }

		public UIManager( Scene<SceneObjectData2> scene, GLWindow dw, Clock32 uiClock ) {
			this.scene = scene;
			this.UIClock = uiClock;
			this.UIView = new MutableSinglet<View2>( new View2( dw, 1, 1, 2, 0 ), ( View2 n ) => { return !( n is null ); } );
			Time = 0;
			DeltaTime = 0;
			Window = dw;
			Stencils = new UIStencilManager();

			elements = new Dictionary<uint, UIElement>();
			eventListeners = new Dictionary<string, IUIEventListener>();

			MousePointer = new Shape2PointTransformless( 1, 0 );

			dw.EventHandler.Keyboard.Add( this );
			dw.EventHandler.Mouse.Add( this );
			dw.EventHandler.Window.Add( this );
		}

		internal void AddToScene( UIElement e ) {
			scene.Add( e );
		}

		internal void RemoveFromScene( UIElement e ) {
			scene.Remove( e );
		}

		public void Add( UIElement element, bool activate = false ) {
			if( element == null )
				return;
			if( !elements.ContainsKey( element.ID ) ) {
				element.SetUIManager( this );
				elements.Add( element.ID, element );
				element.InternalWindowResized( Window.AspectRatioVector, Window.Size );
				if( activate )
					element.Activate();
			}
		}

		public void Add( string eventName, IUIEventListener eListener ) {
			eventListeners.Add( eventName, eListener );
		}

		public void Remove( uint id ) {
			if( elements.TryGetValue( id, out UIElement e ) ) {
				elements.Remove( id );
				scene.Remove( e );
				e.Deactivate();
			}
		}

		public void Remove( string eventName ) {
			eventListeners.Remove( eventName );
		}

		public void Update() {
			float newTime = UIClock.Time;
			DeltaTime = newTime - Time;
			Time = newTime;
			UIView.Value.UpdateMatrices();
			var data = Window.EventHandler.Mouse.Data;
			MousePointer.Set( data.PositionNDCA * UIView.Value.Projection.Scale * 0.5f + UIView.Value.TranformInterface.Translation );
			foreach( UIElement e in elements.Values )
				e.InternalUpdate( data );
			Stencils.UpdateStencils( Window, UIView.Value );
		}

		public void WindowResizeHandler( IntPtr winPtr, int width, int height ) {
			if( winPtr.Equals( (IntPtr) Window.GLFWWindow ) ) {
				foreach( UIElement e in elements.Values )
					e.InternalWindowResized( Window.AspectRatioVector, new Vector2i( width, height ) );
			}
		}

		public void WindowFocusHandler( IntPtr winPtr, bool focused ) {

		}

		public void KeyReleaseHandler( IntPtr winPtr, Keys key, ModifierKeys mods ) {
			if( winPtr.Equals( (IntPtr) Window.GLFWWindow ) ) {
				UIEvent uiE = new UIEvent();
				foreach( UIElement e in elements.Values )
					e.InternalKeyRelease( winPtr, key, uiE );
				foreach( IUIEventListener e in eventListeners.Values )
					e.KeyReleaseHandler( winPtr, key, mods, uiE );
			}
		}

		public void KeyPressHandler( IntPtr winPtr, Keys key, ModifierKeys mods ) {
			if( winPtr.Equals( (IntPtr) Window.GLFWWindow ) ) {
				UIEvent uiE = new UIEvent();
				foreach( UIElement e in elements.Values )
					e.InternalKeyPress( winPtr, key, uiE );
				foreach( IUIEventListener e in eventListeners.Values )
					e.KeyPressHandler( winPtr, key, mods, uiE );
			}
		}

		public void WritingHandler( IntPtr winPtr, char c ) {
			if( winPtr.Equals( (IntPtr) Window.GLFWWindow ) ) {
				UIEvent uiE = new UIEvent();
				foreach( UIElement e in elements.Values )
					e.InternalKeyWritten( winPtr, c, uiE );
				foreach( IUIEventListener e in eventListeners.Values )
					e.WritingHandler( winPtr, c, uiE );
			}
		}

		public void ButtonReleaseHandler( IntPtr winPtr, MouseButton btn, ModifierKeys mods, MouseInputEventData data ) {
			if( winPtr.Equals( (IntPtr) Window.GLFWWindow ) ) {
				UIEvent uiE = new UIEvent();
				foreach( UIElement e in elements.Values )
					e.InternalMouseButtonRelease( winPtr, btn, data, uiE );
				foreach( IUIEventListener e in eventListeners.Values )
					e.ButtonReleaseHandler( winPtr, btn, mods, data, uiE );
			}
		}

		public void ButtonPressHandler( IntPtr winPtr, MouseButton btn, ModifierKeys mods, MouseInputEventData data ) {
			if( winPtr.Equals( (IntPtr) Window.GLFWWindow ) ) {
				UIEvent uiE = new UIEvent();
				foreach( UIElement e in elements.Values )
					e.InternalMouseButtonPress( winPtr, btn, data, uiE );
				foreach( IUIEventListener e in eventListeners.Values )
					e.ButtonPressHandler( winPtr, btn, mods, data, uiE );
			}
		}

		public void MouseMoveHandler( IntPtr winPtr, MouseInputEventData data ) {
			if( winPtr.Equals( (IntPtr) Window.GLFWWindow ) ) {
				UIEvent uiE = new UIEvent();
				foreach( UIElement e in elements.Values )
					e.InternalMouseMove( winPtr, data, uiE );
				foreach( IUIEventListener e in eventListeners.Values )
					e.MouseMoveHandler( winPtr, data, uiE );
			}
		}

		public void MouseDragHandler( IntPtr winPtr, MouseInputEventData data ) {
			if( winPtr.Equals( (IntPtr) Window.GLFWWindow ) ) {
				UIEvent uiE = new UIEvent();
				foreach( UIElement e in elements.Values )
					e.InternalMouseMovedLocked( winPtr, data, uiE );
				foreach( IUIEventListener e in eventListeners.Values )
					e.MouseDragHandler( winPtr, data, uiE );
			}
		}

		public void WheelScrollChangeHandler( IntPtr winPtr, float delta, MouseInputEventData data ) {
			if( winPtr.Equals( (IntPtr) Window.GLFWWindow ) ) {
				UIEvent uiE = new UIEvent();
				foreach( UIElement e in elements.Values )
					e.InternalMouseWheelMoved( winPtr, delta, data, uiE );
				foreach( IUIEventListener e in eventListeners.Values )
					e.WheelScrollChangeHandler( winPtr, delta, data, uiE );
			}
		}
	}
}
