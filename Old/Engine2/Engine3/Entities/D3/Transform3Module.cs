using Engine.Graphics.Objects.Default.Transforms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities.D3 {
	public class Transform3Module : Module {

		public Transform3 Transform { get; private set; }

		public Transform3Module() {
			Transform = new Transform3();
		}

		protected override void Initialize() { }

		public override void Update( float time, float deltaTime ) { }

		public override string ToString() {
			return Transform.ToString();
		}
	}
}
