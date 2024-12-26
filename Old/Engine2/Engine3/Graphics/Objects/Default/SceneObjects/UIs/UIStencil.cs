using Engine.Graphics.Objects.Default.Framebuffers;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Graphics.Utilities;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public class UIStencil {
		private readonly HashSet<UIElement> elements;
		private bool needsUpdate;
		public UIStencilBuffer Buffer { get; private set; }

		public UIStencil( GLWindow window ) {
			Buffer = new UIStencilBuffer( "Stencil Buffer", window, 0.5f );
			elements = new HashSet<UIElement>();
		}

		public void AddElement( UIElement e ) {
			elements.Add( e );
			e.TransformChanged += SignalUpdate;
			e.Activated += SignalUpdate;
			e.Deactivated += SignalUpdate;
			e.ValidationChanged += SignalUpdate;
			e.RenderChanged += ElementUpdated;
			Buffer.Remade += SignalUpdate;
			needsUpdate = true;
		}


		private void ElementUpdated( RenderMethod<SceneObjectData2> value ) => SignalUpdate();

		private void SignalUpdate( SceneObject<SceneObjectData2> r ) {
			needsUpdate = elements.Count > 0;
		}

		public void SignalUpdate() {
			needsUpdate = elements.Count > 0;
		}

		public void SignalUpdate( Renderable2 e ) {
			needsUpdate = elements.Count > 0;
		}

		private void RenderElement( UIElement e, IView view ) {
			if( e.Mesh is null )
				return;
			if( e.RenderFunction is null )
				return;
			Shader s = e.ShaderBundle.Get( 1 );
			if( s is null )
				return;
			s.Bind();
			e.Mesh.Bind();
			e.Mesh.BindShader( s );
			e.StencilRender( s, view );
		}

		public bool Update( IView view ) {
			if( needsUpdate ) {
				needsUpdate = false;
				Buffer.Bind( FramebufferTarget.Framebuffer );
				Gl.Disable( EnableCap.Blend );
				Gl.ClearColor( 1, 1, 1, 1 );
				Gl.Clear( ClearBufferMask.ColorBufferBit );
				foreach( UIElement e in elements ) {
					RenderElement( e, view );
				}
				Gl.ClearColor( 0, 0, 0, 0 );
				Gl.Enable( EnableCap.Blend );
				return true;
			}
			return false;
		}
	}
}
