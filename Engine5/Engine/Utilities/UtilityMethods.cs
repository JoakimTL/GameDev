using System.Text;

namespace Engine.Utilities;
public unsafe static class UtilityMethods {
	public static string PointerToStringNullStop( IntPtr pointer, Encoding encoding, int maxLen = 1024 ) {
		sbyte* ptr = (sbyte*) pointer.ToPointer();
		int len = 0;
		while ( ptr[ len ] != 0 && len < maxLen )
			len++;

		return new string( ptr, 0, len, encoding );
	}

	public static bool IsPowerOf2( this uint val ) => val != 0 && ( val & ( val - 1 ) ) == 0;
	public static bool IsPowerOf2( this int val ) => val != 0 && ( val & ( val - 1 ) ) == 0;
}
