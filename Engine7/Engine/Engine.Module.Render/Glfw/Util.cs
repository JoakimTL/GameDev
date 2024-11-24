using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.Module.Render.Glfw;

internal static class Util {
	#region Methods

	/// <summary>
	///     Reads memory from the pointer until the first null byte is encountered and decodes the bytes from UTF-8 into a
	///     managed <see cref="string" />.
	/// </summary>
	/// <param name="ptr">Pointer to the start of the string.</param>
	/// <returns>Managed string created from read UTF-8 bytes.</returns>
	// ReSharper disable once InconsistentNaming
	public static string? PtrToStringUTF8( nint ptr ) {
		if (ptr == nint.Zero)
			return null;

		int length = 0;
		while (Marshal.ReadByte( ptr, length ) != 0)
			length++;
		unsafe {
			byte* buffer = stackalloc byte[ length ];
			Unsafe.CopyBlock( buffer, ptr.ToPointer(), (uint) length );
			return Encoding.UTF8.GetString( buffer, length );
		}
	}

	#endregion
}