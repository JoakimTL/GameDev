using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities.D3 {
	public class Render3Module : Module {

		private Transform3Module transformModule;
		public readonly ModuleRenderable RenderObject;

		public Render3Module() {
			RenderObject = new ModuleRenderable( this );
		}

		public Render3Module( Mesh mesh, Material mat, ShaderBundle shaderBundle ) {
			RenderObject = new ModuleRenderable( this ) {
				Mesh = mesh,
				Material = mat,
				ShaderBundle = shaderBundle
			};
		}

		protected override void Initialize() {
			Entity.ModuleAdded += ModuleAdded;
			Entity.ModuleRemoved += ModuleRemoved;
			if( Entity.Get( out Transform3Module tm ) ) {
				transformModule = tm;
				RenderObject.UpdateTransformComponent();
			}
		}

		private void ModuleAdded( Entity e, Module m ) {
			if( m is Transform3Module tm ) {
				transformModule = tm;
				RenderObject.UpdateTransformComponent();
			}
		}

		private void ModuleRemoved( Entity e, Module m ) {
			if( m is Transform3Module )
				transformModule = null;
		}

		public override void Update( float time, float deltaTime ) { }

		public override string ToString() {
			return RenderObject.ToString();
		}

		public class ModuleRenderable : Renderable3 {
			private Render3Module module;

			public ModuleRenderable( Render3Module mod ) {
				module = mod;
			}

			protected override void OnDispose() {
			}

			internal void UpdateTransformComponent() {
				Data.Transform.SetParent( module.transformModule.Transform );
			}
		}
	}
}
