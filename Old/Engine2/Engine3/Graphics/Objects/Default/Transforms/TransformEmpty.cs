using System;
using Engine.Graphics.Objects;
using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.Transforms {
	public class TransformEmpty : Transform<float, float, float> {

		public override float GlobalTranslation { get { return 0; } }
		public override float GlobalRotation { get { return 0; } }
		public override float GlobalScale { get { return 0; } }

		public override float Translation { get => 0; set { } }
		public override float Rotation { get => 0; set { } }
		public override float Scale { get => 0; set { } }

		protected override Matrix4 TranslationMatrix => Matrix4.Identity;
		protected override Matrix4 RotationMatrix => Matrix4.Identity;
		protected override Matrix4 ScaleMatrix => Matrix4.Identity;
	}
}
