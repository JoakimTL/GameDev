using System.Runtime.CompilerServices;

namespace Engine.Standard.Render.Text.Fonts;

public sealed class FontCaretedDataReader( FontDataReader dataReader) {
	private readonly FontDataReader _dataReader = dataReader;
	private nint _offset = 0;
	public T Read<T>() where T : unmanaged {
		T value = this._dataReader.Read<T>( this._offset );
		this._offset += Unsafe.SizeOf<T>( );
		return value;
	}

	public void SkipBytes( int numBytes ) {
		this._offset += numBytes;
	}

	public void GoTo( uint offset ) {
		this._offset = (nint) offset;
	}
}
