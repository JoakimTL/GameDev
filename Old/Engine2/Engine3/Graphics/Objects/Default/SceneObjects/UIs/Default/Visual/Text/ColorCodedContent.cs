using Engine.LinearAlgebra;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class ColorCodedContent {

		public string Content => content.ToString();
		private StringBuilder content;
		public IReadOnlyDictionary<int, Vector4i> ColorCodes => colorCodes;
		private Dictionary<int, Vector4i> colorCodes;

		public ColorCodedContent() {
			content = new StringBuilder();
			colorCodes = new Dictionary<int, Vector4i>();
		}

		public ColorCodedContent( string content, Dictionary<int, Vector4i> colorCodes ) {
			this.content = new StringBuilder( content );
			this.colorCodes = colorCodes;
		}

		public ColorCodedContent Append( string content, Vector4i color ) {
			for( int i = this.content.Length; i < this.content.Length + content.Length; i++ ) {
				colorCodes[ i ] = color;
			}
			this.content.Append( content );
			return this;
		}

		public ColorCodedContent Append( string content ) {
			this.content.Append( content );
			return this;
		}
	}
}
