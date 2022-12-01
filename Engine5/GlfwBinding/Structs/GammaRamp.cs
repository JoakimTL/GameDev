using System.Runtime.InteropServices;

namespace GlfwBinding.Structs;

[StructLayout( LayoutKind.Sequential )]
public readonly struct GammaRamp {

	[MarshalAs( UnmanagedType.LPArray )]
	public readonly ushort[] Red;

	[MarshalAs( UnmanagedType.LPArray )]
	public readonly ushort[] Green;

	[MarshalAs( UnmanagedType.LPArray )]
	public readonly ushort[] Blue;

	public readonly uint Size;

	/// <summary>
	///     Creates a new instance of a <see cref="GammaRamp" /> using the specified values.
	///     <para>WARNING: On some platforms (Windows), each value MUST be 256 in length.</para>
	/// </summary>
	/// <param name="red">An array of value describing the response of the red channel.</param>
	/// <param name="green">An array of value describing the response of the green channel.</param>
	/// <param name="blue">An array of value describing the response of the blue channel.</param>
	public GammaRamp( ushort[] red, ushort[] green, ushort[] blue ) {
		if ( red.Length == green.Length && green.Length == blue.Length ) {
			this.Red = red;
			this.Green = green;
			this.Blue = blue;
			this.Size = (uint) red.Length;
		} else {
			throw new ArgumentException(
				$"{nameof( red )}, {nameof( green )}, and {nameof( blue )} must all be equal length." );
		}
	}

	public unsafe static implicit operator GammaRamp( GammaRampInternal ramp ) {
		int offset = 0;
		ushort[]? red = new ushort[ ramp.Size ];
		ushort[]? green = new ushort[ ramp.Size ];
		ushort[]? blue = new ushort[ ramp.Size ];
		ushort* rSrc = (ushort*) ramp.Red;
		ushort* gSrc = (ushort*) ramp.Green;
		ushort* bSrc = (ushort*) ramp.Blue;
		for ( int i = 0; i < ramp.Size; i++, offset += sizeof( ushort ) ) {
			red[ i ] = rSrc[ offset ];
			green[i] = gSrc[ offset ];
			blue[ i ] = bSrc[ offset ];
		}

		return new GammaRamp( red, green, blue );
	}
}
