using Engine;
using System.Runtime.InteropServices;

internal unsafe class TestClass {
	private byte* _bytes;
	private uint _length;
	public Memory<byte> Bytes => DebugUtilities.PointerToMemory( _bytes, _length );
	public TestClass() {
		_length = 4096;
		_bytes = (byte*) NativeMemory.AllocZeroed( _length );
		Random random = new Random();
		for ( int i = 0; i < _length; i++ ) {
			_bytes[ i ] = unchecked((byte) random.Next());
		}
	}
}