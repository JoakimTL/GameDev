using Engine.GLFrameWork;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Physics.Collision;
using Engine.Physics.D2;
using Engine.Utilities.Data.Boxing;
using OpenGL;
using System;
using System.Collections.Generic;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public class UIElement : Renderable2 {

		/* 
		 * TODO
		 * Events:
		 * x Child Added
		 * x Child Removed
		 * x Parent Changed
		 * x Activation
		 * x Deactivation
		 * x Update
		 * x Active Update
		 * x Inactive Update
		 * x Mouse Moved
		 * - Mouse Hover
		 * x Mouse Button Pressed
		 * x Mouse Button Released
		 * x Key Pressed
		 * x Key Released
		 * x Mouse Wheel Moved
		 * x Window Resize
		 * x Layer Change
		 * x Transform Change
		 * x Mesh Change
		 * x Material Change
		 * x Shader Change
		 * x Pre Render
		 * x Post Render
		 * - Render Setup
		 * 
		 * Functionality
		 * x Stencil
		 * - Collision
		 * 
		 */

		#region Collision
		public CollisionResult<Vector2> CollisionResult { get; private set; }
		#endregion

		#region Stencil
		public UIStencil Stencil { get; private set; }
		#endregion

		#region Transform
		public ConstraintManager Constraints { get; private set; }
		public TransformReadonlyInterface<Vector2, float, Vector2> TransformInterface { get => Data.Transform.Readonly; }
		public bool Normalized { get => Data.Transform.Normalized; set => Data.Transform.SetNormalization( value ); }
		#endregion

		#region Hierarchy
		private readonly HashSet<UIElement> children;
		public IReadOnlyCollection<UIElement> Children { get => children; }
		public UIElement Parent { get; private set; }
		public UIManager Manager { get; private set; }
		private readonly MutableSinglet<uint> _layerOffset;
		public uint LayerOffset { get => _layerOffset.Value; set => _layerOffset.Value = value; }
		protected HashSet<uint> Whitelist { get; private set; }
		#endregion

		#region Flags
		public bool UseDefaultRender { get; set; }
		public bool ActivateWithParent { get; set; }
		public bool TakesInput { get; set; }
		#endregion

		#region Events
		public event KeyEventHandler KeyPressed;
		public event KeyEventHandler KeyReleased;
		public event KeyWrittenHandler KeyWritten;
		public event MouseButtonEventHandler MouseButtonPressed;
		public event MouseButtonEventHandler MouseButtonReleased;
		public event MouseMoveHandler MouseMoved;
		public event MouseMoveHandler LockedMouseMoved;
		public event MouseWheelMoveHandler MouseWheelMoved;
		public event WindowResizeHandler WindowResized;
		public event ElementRelationChangeHandler ChildRemoved;
		public event ElementRelationChangeHandler ChildAdded;
		public event ElementRelationChangeHandler ParentSet;
		public event ElementRelationChangeHandler ParentUnset;
		public event ManagerChangeHandler ManagerChanged;
		public event Action Activated;
		public event Action Deactivated;
		/**<summary>
		 * The second update event when an update tick happens.
		 * <br></br><b>Only happens when the element is active.</b>
		 * </summary>*/
		public event UpdateHandler UpdatedSecondActive;
		/**<summary>
		 * The third update event when an update tick happens.<br></br>
		 * <b>Only happens when the element is active.</b>
		 * <br></br>Happens after the children has had their update calls and before the constraint transformation has been applied.</summary>*/
		public event UpdateHandler UpdatedThirdActive;
		/**<summary>
		 * The fourth update event when an update tick happens.
		 * <br></br><b>Only happens when the element is active.</b>
		 * <br></br>Happens after the constraint transformation has been applied.</summary>*/
		public event UpdateHandler UpdatedFourthActive;
		/**<summary>
		 * The second update event when an update tick happens.
		 * <br></br><b>Only happens when the element is inactive.</b>
		 * <br></br>Happens after the children has had their update calls.</summary>*/
		public event UpdateHandler UpdatedSecondInactive;
		/**<summary>The first update event when an update tick happens. Happens before anything else.
		 * <br></br><b>Happens regardless of the activation status of the element.</b></summary>*/
		public event UpdateHandler UpdatedFirst;
		/**<summary>The last update event when an update tick happens. Happens after everything else.
		 * <br></br><b>Happens regardless of the activation status of the element.</b></summary>*/
		public event UpdateHandler UpdatedLast;
		#endregion

		public UIElement() {
			Constraints = new ConstraintManager( this );
			Active = false;
			children = new HashSet<UIElement>();
			Parent = null;
			_layerOffset = new MutableSinglet<uint>( 1 );
			_layerOffset.Changed += LayerChangeHandler;
			UseDefaultRender = true;
			ActivateWithParent = true;
			TakesInput = true;
			Whitelist = new HashSet<uint>();
			RenderFunction = DefaultUIRenderMethod;
			ShaderBundle = Mem.ShaderBundles.UI;
			CollisionResult = new CollisionResult<Vector2>();
		}

		private void DefaultUIRenderMethod( SceneObject<SceneObjectData2> so, Shader s, IView view ) {
			s.Set( "uMVP_mat", so.Data.TransformObject.Matrix * view.VPMatrix );
			s.Set( "uColor", Data.Color );
			so.Mesh.RenderMesh();
		}

		#region Stencil
		internal void StencilRender( Shader s, IView view ) {
			s.Set( "uMVP_mat", Data.TransformObject.Matrix * view.VPMatrix );
			Mesh.RenderMesh();
		}

		public void SetStencil( UIStencil stencil ) {
			Stencil = stencil;
			if( Stencil is null ) {
				BeforeRendering -= PreStencilRender;
				AfterRendering -= PostStencilRender;
			} else {
				BeforeRendering += PreStencilRender;
				AfterRendering += PostStencilRender;
			}
		}

		private void PreStencilRender() {
			Gl.ActiveTexture( TextureUnit.Texture4 );
			Gl.BindTexture( TextureTarget.Texture2d, Stencil.Buffer.Texture );
		}

		private void PostStencilRender() {
			Gl.ActiveTexture( TextureUnit.Texture4 );
			Gl.BindTexture( TextureTarget.Texture2d, 0 );
		}
		#endregion

		#region Update and Transform
		internal void InternalUpdate( MouseInputEventData data ) {
			UpdatedFirst?.Invoke( data );

			if( Active )
				UpdatedSecondActive?.Invoke( data );

			foreach( UIElement e in Children )
				e.InternalUpdate( data );

			if( Active ) {
				UpdatedThirdActive?.Invoke( data );
				UpdateTransform();
				UpdatedFourthActive?.Invoke( data );
			} else {
				UpdatedSecondInactive?.Invoke( data );
			}
			UpdatedLast?.Invoke( data );
		}

		public void UpdateTransform() {
			Constraints.Update();

			Data.Transform.Translation = Constraints.Transform.Translation;
			Data.Transform.Scale = Constraints.Transform.Scale;
			Data.Transform.Rotation = Constraints.Transform.Rotation;
			Data.Color = Constraints.Transform.Color;
		}

		/**
		 * <summary>Usually using data.PositionNDCAM</summary>
		 */
		public Vector2 GetRelativePosition( Vector2 point ) {
			return Vector2.Rotate( ( point - Data.Transform.GlobalTranslation ) / Data.Transform.GlobalScale, -Data.Transform.GlobalRotation );
		}
		#endregion

		#region Activation/Deactivation
		public void Activate() {
			if( Manager is null ) {
				Logging.Warning( $"[{this}]: Tried to activate, but manager is not set!" );
				return;
			}

			foreach( UIElement e in Children )
				if( e.ActivateWithParent )
					e.Activate();

			if( !Active ) {
				Active = true;
				UpdateTransform();
				Activated?.Invoke();
			}
		}

		public void Deactivate() {
			foreach( UIElement e in Children )
				e.Deactivate();
			if( Active ) {
				Active = false;
				Deactivated?.Invoke();
			}
		}
		#endregion

		#region Hierarchy
		public void SetParent( UIElement e ) {
			if( !( Parent is null ) ) {
				Parent.RemoveChild( this );
				ParentUnset?.Invoke( this, Parent );
				Parent.LayerChanged -= LayerChangeHandler;
			}

			Parent = e;

			if( !( Parent is null ) ) {
				Parent.AddChild( this );
				ParentSet?.Invoke( this, Parent );
				Layer = Parent.Layer + LayerOffset;
				SetUIManager( Parent.Manager );
				if( Parent.Active ) {
					if( ActivateWithParent ) {
						Activate();
					}
				} else if( !Parent.Active )
					Deactivate();
				Data.Transform.SetParent( Parent.Data.Transform );
				Parent.LayerChanged += LayerChangeHandler;
			} else {
				Layer = LayerOffset;
				Data.Transform.SetParent( null );
			}
		}

		private void RemoveChild( UIElement e ) {
			children.Remove( e );
			ChildRemoved?.Invoke( this, e );
		}

		private void AddChild( UIElement e ) {
			children.Add( e );
			ChildAdded?.Invoke( this, e );
		}

		internal void SetUIManager( UIManager manager ) {
			if( manager is null )
				return;

			if( !( Manager is null ) )
				Manager.RemoveFromScene( this );

			Manager = manager;
			Manager.AddToScene( this );
			ManagerChanged?.Invoke( Manager, this );

			foreach( UIElement e in children ) {
				e.SetUIManager( Manager );
			}
		}

		private void LayerChangeHandler( uint value ) {
			if( !( Parent is null ) ) {
				Layer = Parent.Layer + LayerOffset;
			} else {
				Layer = LayerOffset;
			}
		}

		public void DetachChildren() {
			HashSet<UIElement> children = new HashSet<UIElement>( Children );
			foreach( UIElement child in children )
				child.SetParent( null );
		}
		#endregion

		#region Collision
		/// <summary>
		/// Checks whether the mouse if colliding with the ui elements collision model.
		/// </summary>
		/// <param name="epa">Use the Expanding Polytope Algorithm to determine the depth and normal vector of the collision.</param>
		/// <param name="warn">Gives a warning whenever there are no collision shapes to collide with.</param>
		/// <returns>Returns the ui elements internal CollisionResult field.</returns>
		public CollisionResult<Vector2> CheckCollisionToMouse( bool epa = false, bool warn = true ) {
			CollisionResult.Clear();
			if( Manager is null )
				return CollisionResult;
			if( !Manager.Window.IsFocused )
				return CollisionResult;
			if( Data.CollisionModel.ShapeCount == 0 ) {
				Logging.Warning( "No shapes in UI collision model." );
				return CollisionResult;
			}
			CollisionChecker.CheckCollision( Manager.MousePointer, Data.CollisionModel, CollisionResult, epa );
			return CollisionResult;
		}
		#endregion

		#region Events
		internal void InternalWindowResized( Vector2 arv, Vector2i size ) {
			foreach( UIElement e in Children )
				e.InternalWindowResized( arv, size );
			WindowResized?.Invoke( arv, size );
		}

		internal void InternalKeyPress( IntPtr winPtr, Keys key, UIEvent uiEvent ) {
			if( !Active || !TakesInput )
				return;

			foreach( UIElement e in Children )
				e.InternalKeyPress( winPtr, key, uiEvent );

			if( uiEvent.ElementExists( children, Whitelist ) ) {
				uiEvent.Add( this );
			} else {
				bool activated = false;
				KeyPressed?.Invoke( winPtr, ID, key, ref activated );
				if( activated )
					uiEvent.Add( this );
			}
		}

		internal void InternalKeyRelease( IntPtr winPtr, Keys key, UIEvent uiEvent ) {
			if( !Active || !TakesInput )
				return;

			foreach( UIElement e in Children )
				e.InternalKeyRelease( winPtr, key, uiEvent );

			if( uiEvent.ElementExists( children, Whitelist ) ) {
				uiEvent.Add( this );
			} else {
				bool activated = false;
				KeyReleased?.Invoke( winPtr, ID, key, ref activated );
				if( activated )
					uiEvent.Add( this );
			}
		}

		internal void InternalKeyWritten( IntPtr winPtr, char c, UIEvent uiEvent ) {
			if( !Active || !TakesInput )
				return;

			foreach( UIElement e in Children )
				e.InternalKeyWritten( winPtr, c, uiEvent );

			if( uiEvent.ElementExists( children, Whitelist ) ) {
				uiEvent.Add( this );
			} else {
				bool activated = false;
				KeyWritten?.Invoke( winPtr, ID, c, ref activated );
				if( activated )
					uiEvent.Add( this );
			}
		}

		internal void InternalMouseButtonPress( IntPtr winPtr, MouseButton button, MouseInputEventData data, UIEvent uiEvent ) {
			if( !Active || !TakesInput )
				return;

			foreach( UIElement e in Children )
				e.InternalMouseButtonPress( winPtr, button, data, uiEvent );

			if( uiEvent.ElementExists( children, Whitelist ) ) {
				uiEvent.Add( this );
			} else {
				bool activated = false;
				MouseButtonPressed?.Invoke( winPtr, ID, button, data, ref activated );
				if( activated )
					uiEvent.Add( this );
			}
		}

		internal void InternalMouseButtonRelease( IntPtr winPtr, MouseButton button, MouseInputEventData data, UIEvent uiEvent ) {
			if( !Active || !TakesInput )
				return;

			foreach( UIElement e in Children )
				e.InternalMouseButtonRelease( winPtr, button, data, uiEvent );

			if( uiEvent.ElementExists( children, Whitelist ) ) {
				uiEvent.Add( this );
			} else {
				bool activated = false;
				MouseButtonReleased?.Invoke( winPtr, ID, button, data, ref activated );
				if( activated )
					uiEvent.Add( this );
			}
		}

		internal void InternalMouseMove( IntPtr winPtr, MouseInputEventData data, UIEvent uiEvent ) {
			if( !Active || !TakesInput )
				return;

			foreach( UIElement e in Children )
				e.InternalMouseMove( winPtr, data, uiEvent );

			if( uiEvent.ElementExists( children, Whitelist ) ) {
				uiEvent.Add( this );
			} else {
				bool activated = false;
				MouseMoved?.Invoke( winPtr, ID, data, ref activated );
				if( activated )
					uiEvent.Add( this );
			}
		}

		internal void InternalMouseMovedLocked( IntPtr winPtr, MouseInputEventData data, UIEvent uiEvent ) {
			if( !Active || !TakesInput )
				return;

			foreach( UIElement e in Children )
				e.InternalMouseMovedLocked( winPtr, data, uiEvent );

			if( uiEvent.ElementExists( children, Whitelist ) ) {
				uiEvent.Add( this );
			} else {
				bool activated = false;
				LockedMouseMoved?.Invoke( winPtr, ID, data, ref activated );
				if( activated )
					uiEvent.Add( this );
			}
		}

		internal void InternalMouseWheelMoved( IntPtr winPtr, float delta, MouseInputEventData data, UIEvent uiEvent ) {
			if( !Active || !TakesInput )
				return;

			foreach( UIElement e in Children )
				e.InternalMouseWheelMoved( winPtr, delta, data, uiEvent );

			if( uiEvent.ElementExists( children, Whitelist ) ) {
				uiEvent.Add( this );
			} else {
				bool activated = false;
				MouseWheelMoved?.Invoke( winPtr, ID, delta, data, ref activated );
				if( activated )
					uiEvent.Add( this );
			}
		}
		#endregion

		public override string ToString() {
			return $"[{base.ToString()}]: {ID}]";
		}

		protected override void OnDispose() {
			SetParent( null );
		}
	}
}
