using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class InternalLetter {

		public Symbol Symbol { get; private set; }
		public Vector4i Color;

		public InternalLetter( Symbol s, Vector4i color ) {
			Symbol = s;
			Color = color;
		}

	}
}
