using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.Physics.Collision;
using Engine.Physics.D2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Scenes {
	public class OcclusionScene2 : OcclusionScene<Transform2, SceneObject<SceneObjectData2>, SceneObjectData2> {

		public Physics2Model CollisionModel { get; private set; }

		public OcclusionScene2( string name, Scene<SceneObjectData2> scene ) : base( name, scene, new Transform2() ) {
			CollisionModel = new Physics2Model( "OccScene2 #" + name, transform );
		}
		public OcclusionScene2( string name, Scene<SceneObjectData2> scene, Transform2 transform ) : base( name, scene, transform ) {
			CollisionModel = new Physics2Model( "OccScene2 #" + name, this.transform );
		}

		protected override bool CheckOcclusion( SceneObject<SceneObjectData2> r ) {
			return CollisionChecker.CheckAABBCollision( CollisionModel, r.Data.CollisionModel );
		}

		protected override void RemoveTracking( SceneObject<SceneObjectData2> r ) {
			r.TransformChanged -= RenderableChange;
		}

		protected override void AddTracking( SceneObject<SceneObjectData2> r ) {
			r.TransformChanged += RenderableChange;
		}
	}
}
