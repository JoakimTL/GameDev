using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics.D3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects {
	public class SceneObjectData3 : SceneObjectData {

		public Transform3 Transform { get; private set; }
		public Physics3Model CollisionModel { get; private set; }

		public override ITransform TransformObject => Transform;

		public SceneObjectData3() {
			Transform = new Transform3();
			CollisionModel = new Physics3Model( "SceneObject<SceneObjectData3> #" + ID, Transform );
		}

		public override void Dispose() {
			Transform.SetParent( null );
		}

	}
}
