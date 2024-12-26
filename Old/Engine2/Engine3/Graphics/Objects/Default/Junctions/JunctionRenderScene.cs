using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Utilities.Data.Boxing;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Engine.Graphics.Objects.Default.Junctions {
	class JunctionRenderScene<T, V> : Engine.Pipelines.Junction where T : SceneObjectData where V : IView {

		private readonly MutableSinglet<V> view;
		private readonly SceneRenderer<T> scene;
		private readonly RenderMethod<T> renOverride;
		private readonly uint usecase;
		private readonly Material matOverride;

		public JunctionRenderScene( string name, MutableSinglet<V> view, SceneRenderer<T> scene, uint usecase = 0, RenderMethod<T> renOverride = null, Material matOverride = null ) : base( name, null ) {
			this.view = view;
			this.scene = scene;
			this.renOverride = renOverride;
			this.usecase = usecase;
			this.matOverride = matOverride;
			Effect = Execute;
		}

		protected void Execute() {
			scene.Render( view.Value, usecase, renOverride, matOverride );
		}
	}
}
