using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class Symbol {

		public readonly char Character;
		public readonly float TextureX, TextureY;
		public readonly float TextureW, TextureH;
		public readonly float Width, Height;
		public readonly float OffsetX, OffsetY;
		public readonly float SizeX;

		public Symbol( char rep, Vector4 t, float width, float height, float offX, float offY, float sizeX ) {
			this.Character = rep;
			this.TextureX = t.X;
			this.TextureY = t.Y;
			this.TextureW = t.Z;
			this.TextureH = t.W;
			this.Width = width;
			this.Height = height;
			this.OffsetX = offX;
			this.OffsetY = offY;
			this.SizeX = sizeX;
		}

		public override string ToString() {
			return Character.ToString();
		}
	}
}
