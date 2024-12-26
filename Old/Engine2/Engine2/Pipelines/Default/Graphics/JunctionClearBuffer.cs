using OpenGL;

namespace Engine.Pipelines.Default.Graphics {
	public class JunctionClearBuffer : Junction {
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
