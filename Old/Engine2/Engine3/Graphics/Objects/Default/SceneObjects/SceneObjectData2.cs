using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics.D2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects {
	public class SceneObjectData2 : SceneObjectData {

		public Transform2 Transform { get; private set; }
		public Physics2Model CollisionModel { get; private set; }

		public override ITransform TransformObject => Transform;

		public SceneObjectData2() {
			Transform = new Transform2();
			CollisionModel = new Physics2Model( "SceneObject<SceneObjectData2> #" + ID, Transform );
		}

		public override void Dispose() {
			Transform.SetParent( null );
		}
	}
}
