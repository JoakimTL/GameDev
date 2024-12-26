using Engine.Graphics.Objects;
using VoxDev.Voxels;

namespace VoxDev {
	public class TestBlockSet : VoxelBlockSet {
		public TestBlockSet( string name, Texture diffuse, Texture normal, Texture lighting, Texture glow ) : base( name, diffuse, normal, lighting, glow ) {
		}

		protected override void LoadTypes() {
			AddType( "Sparklez", new Engine.LinearAlgebra.Vector2b( 1, 0 ), true, true );
			AddType( "Concrete", new Engine.LinearAlgebra.Vector2b( 2, 0 ), true, true );
		}
	}
}