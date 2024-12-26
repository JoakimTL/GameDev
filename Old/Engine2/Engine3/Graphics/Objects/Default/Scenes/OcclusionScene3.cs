using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.Physics.Collision;
using Engine.Physics.D3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Scenes {
	public class OcclusionScene3 : OcclusionScene<Transform3, SceneObject<SceneObjectData3>, SceneObjectData3> {

		public Physics3Model CollisionModel { get; private set; }

		public OcclusionScene3( string name, Scene<SceneObjectData3> scene ) : base( name, scene, new Transform3() ) {
			CollisionModel = new Physics3Model( "OccScene3 #" + name, transform );
		}
		public OcclusionScene3( string name, Scene<SceneObjectData3> scene, Transform3 transform ) : base( name, scene, transform ) {
			CollisionModel = new Physics3Model( "OccScene3 #" + name, this.transform );
		}

		protected override bool CheckOcclusion( SceneObject<SceneObjectData3> r ) {
			return CollisionChecker.CheckAABBCollision( CollisionModel, r.Data.CollisionModel );
		}

		protected override void RemoveTracking( SceneObject<SceneObjectData3> r ) {
			r.TransformChanged -= RenderableChange;
		}

		protected override void AddTracking( SceneObject<SceneObjectData3> r ) {
			r.TransformChanged += RenderableChange;
		}
	}
}
