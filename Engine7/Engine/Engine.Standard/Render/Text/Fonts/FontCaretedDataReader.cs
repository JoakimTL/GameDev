using System.Runtime.CompilerServices;

namespace Engine.Standard.Render.Text.Fonts;

public sealed class FontCaretedDataReader( FontDataReader dataReader ) {
	private readonly FontDataReader _dataReader = dataReader;
	private nint _offset = 0;

	public uint CurrentOffset => (uint) this._offset;

	public T Read<T>( bool ensureEndianessMatchesArch = true ) where T : unmanaged {
		T value = this._dataReader.Read<T>( this._offset, ensureEndianessMatchesArch );
		this._offset += Unsafe.SizeOf<T>();
		return value;
	}

	/// <summary>
	/// Moves the caret by the number of bytes specified
	/// </summary>
	/// <param name="numBytes"></param>
	public void MoveCaretBy( int numBytes ) {
		this._offset += numBytes;
	}

	/// <summary>
	/// Moves the caret to the specified offset
	/// </summary>
	/// <param name="offsetBytes"></param>
	public void GoTo( uint offsetBytes ) {
		this._offset = (nint) offsetBytes;
	}
}
