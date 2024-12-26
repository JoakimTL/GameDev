using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public class UIStencilManager {

		private readonly HashSet<WeakReference<UIStencil>> stencils, removed;

		public UIStencilManager() {
			stencils = new HashSet<WeakReference<UIStencil>>();
			removed = new HashSet<WeakReference<UIStencil>>();
		}

		public UIStencil CreateNew( GLWindow window ) {
			UIStencil n = new UIStencil( window );
			stencils.Add( new WeakReference<UIStencil>( n ) );
			return n;
		}

		internal void UpdateStencils( GLWindow window, IView view ) {
			bool updated = false;
			foreach( WeakReference<UIStencil> stencilRef in stencils ) {
				if( stencilRef.TryGetTarget( out UIStencil stencil ) ) {
					updated |= stencil.Update( view );
				} else {
					removed.Add( stencilRef );
				}
			}
			foreach( WeakReference<UIStencil> stencilRef in removed )
				stencils.Remove( stencilRef );
			removed.Clear();

			if( updated )
				window.Bind();
		}

	}
}
