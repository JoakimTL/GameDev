using OpenGL;

namespace Engine.Rendering.Disposal;

public class BufferDisposer : DisposableIdentifiable {
	private readonly uint[] _bufferIds;

	public BufferDisposer(uint[] bufferIds) {
		this._bufferIds = bufferIds;
	}
	protected override bool OnDispose() {
		Gl.DeleteRenderbuffers( this._bufferIds );
		this.LogLine( "Disposed on context thread!", Log.Level.HIGH, ConsoleColor.Cyan );
		return true;
	}
}