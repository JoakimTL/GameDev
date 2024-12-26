using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text;
using System.Collections.Generic;

namespace Engine.MemLib {
	public class StoreFont {

		private readonly Dictionary<string, Font> fonts;

		public StoreFont() {
			fonts = new Dictionary<string, Font>();
		}

		public Font Get( string path ) {
			if( fonts.TryGetValue( path, out Font v ) )
				return v;
			Font f = new Font( path );
			fonts[ path ] = f;
			return f;
		}
	}
}
