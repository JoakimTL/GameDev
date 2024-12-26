using OpenGL;

namespace Engine.Graphics.Objects.Default.Junctions {
	public class JunctionClearBuffer : Engine.Pipelines.Junction {
		private Buffer buffer;
		private int attachment;
		private float[] value;

		public JunctionClearBuffer( string name, Buffer buffer, int attachment, float[] value ) : base( name, null ) {
			this.buffer = buffer;
			this.attachment = attachment;
			this.value = value;
			Effect = Execute;
		}

		private void Execute() {
			Gl.ClearBuffer( buffer, attachment, value );
		}
	}
}
