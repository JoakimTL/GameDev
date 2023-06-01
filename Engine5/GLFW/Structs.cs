using System.Runtime.InteropServices;

namespace GLFW;

/// <summary>
///     Wrapper around a handle for a window cursor object.
/// </summary>
/// <seealso cref="Cursor" />
[StructLayout( LayoutKind.Sequential )]
public struct Cursor : IEquatable<Cursor> {
	/// <summary>
	///     Represents a <c>null</c> value for a <see cref="Cursor" /> object.
	/// </summary>
	public static readonly Cursor None;

	/// <summary>
	///     Internal pointer.
	/// </summary>
	private readonly nint cursor;

	/// <summary>
	///     Determines whether the specified <see cref="Cursor" />, is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="Cursor" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="Cursor" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals( Cursor other ) => this.cursor.Equals( other.cursor );

	/// <summary>
	///     Determines whether the specified <see cref="object" />, is equal to this instance.
	/// </summary>
	/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals( object? obj ) {
		if ( obj is Cursor cur )
			return Equals( cur );
		return false;
	}

	/// <summary>
	///     Returns a hash code for this instance.
	/// </summary>
	/// <returns>
	///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	/// </returns>
	public override int GetHashCode() => this.cursor.GetHashCode();

	/// <summary>
	///     Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator ==( Cursor left, Cursor right ) { return left.Equals( right ); }

	/// <summary>
	///     Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator !=( Cursor left, Cursor right ) { return !left.Equals( right ); }
}

/// <summary>
///     Wrapper around a EGL context pointer.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
// ReSharper disable once InconsistentNaming
public struct EGLContext : IEquatable<EGLContext> {
	/// <summary>
	///     Describes a default/null instance.
	/// </summary>
	public static readonly EGLContext None;

	/// <summary>
	///     Internal pointer.
	/// </summary>
	private readonly nint handle;

	/// <summary>
	///     Performs an implicit conversion from <see cref="EGLContext" /> to <see cref="nint" />.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <returns>
	///     The result of the conversion.
	/// </returns>
	public static implicit operator nint( EGLContext context ) { return context.handle; }

	/// <summary>
	///     Returns a <see cref="string" /> that represents this instance.
	/// </summary>
	/// <returns>
	///     A <see cref="string" /> that represents this instance.
	/// </returns>
	public override string ToString() => this.handle.ToString();

	/// <summary>
	///     Determines whether the specified <see cref="EGLContext" />, is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="EGLContext" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="EGLContext" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals( EGLContext other ) => this.handle.Equals( other.handle );

	/// <summary>
	///     Determines whether the specified <see cref="object" />, is equal to this instance.
	/// </summary>
	/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals( object? obj ) {
		if ( obj is EGLContext context )
			return Equals( context );
		return false;
	}

	/// <summary>
	///     Returns a hash code for this instance.
	/// </summary>
	/// <returns>
	///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	/// </returns>
	public override int GetHashCode() => this.handle.GetHashCode();

	/// <summary>
	///     Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator ==( EGLContext left, EGLContext right ) { return left.Equals( right ); }

	/// <summary>
	///     Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator !=( EGLContext left, EGLContext right ) { return !left.Equals( right ); }
}

/// <summary>
///     Wrapper around a EGL display pointer.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
public struct EGLDisplay : IEquatable<EGLDisplay> {
	/// <summary>
	///     Describes a default/null instance.
	/// </summary>
	public static readonly EGLDisplay None;

	/// <summary>
	///     Internal pointer.
	/// </summary>
	private readonly nint handle;

	/// <summary>
	///     Performs an implicit conversion from <see cref="EGLDisplay" /> to <see cref="nint" />.
	/// </summary>
	/// <param name="display">The display.</param>
	/// <returns>
	///     The result of the conversion.
	/// </returns>
	public static implicit operator nint( EGLDisplay display ) { return display.handle; }

	/// <summary>
	///     Returns a <see cref="string" /> that represents this instance.
	/// </summary>
	/// <returns>
	///     A <see cref="string" /> that represents this instance.
	/// </returns>
	public override string ToString() => this.handle.ToString();

	/// <summary>
	///     Determines whether the specified <see cref="EGLDisplay" />, is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="EGLDisplay" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="EGLDisplay" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals( EGLDisplay other ) => this.handle.Equals( other.handle );

	/// <summary>
	///     Determines whether the specified <see cref="System.object" />, is equal to this instance.
	/// </summary>
	/// <param name="obj">The <see cref="System.object" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="System.object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals( object? obj ) {
		if ( obj is EGLDisplay display )
			return Equals( display );
		return false;
	}

	/// <summary>
	///     Returns a hash code for this instance.
	/// </summary>
	/// <returns>
	///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	/// </returns>
	public override int GetHashCode() => this.handle.GetHashCode();

	/// <summary>
	///     Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator ==( EGLDisplay left, EGLDisplay right ) { return left.Equals( right ); }

	/// <summary>
	///     Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator !=( EGLDisplay left, EGLDisplay right ) { return !left.Equals( right ); }
}

/// <summary>
///     Wrapper around a EGL surface pointer.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
public struct EGLSurface : IEquatable<EGLSurface> {
	/// <summary>
	///     Describes a default/null instance.
	/// </summary>
	public static readonly EGLSurface None;

	/// <summary>
	///     Internal pointer.
	/// </summary>
	private readonly nint handle;

	/// <summary>
	///     Performs an implicit conversion from <see cref="EGLSurface" /> to <see cref="nint" />.
	/// </summary>
	/// <param name="surface">The surface.</param>
	/// <returns>
	///     The result of the conversion.
	/// </returns>
	public static implicit operator nint( EGLSurface surface ) { return surface.handle; }

	/// <summary>
	///     Returns a <see cref="string" /> that represents this instance.
	/// </summary>
	/// <returns>
	///     A <see cref="string" /> that represents this instance.
	/// </returns>
	public override string ToString() => this.handle.ToString();

	/// <summary>
	///     Determines whether the specified <see cref="EGLSurface" />, is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="EGLSurface" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="EGLSurface" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals( EGLSurface other ) => this.handle.Equals( other.handle );

	/// <summary>
	///     Determines whether the specified <see cref="System.object" />, is equal to this instance.
	/// </summary>
	/// <param name="obj">The <see cref="System.object" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="System.object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals( object? obj ) {
		if ( obj is EGLSurface surface )
			return Equals( surface );
		return false;
	}

	/// <summary>
	///     Returns a hash code for this instance.
	/// </summary>
	/// <returns>
	///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	/// </returns>
	public override int GetHashCode() => this.handle.GetHashCode();

	/// <summary>
	///     Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator ==( EGLSurface left, EGLSurface right ) { return left.Equals( right ); }

	/// <summary>
	///     Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator !=( EGLSurface left, EGLSurface right ) { return !left.Equals( right ); }
}

/// <summary>
///     Represents the state of a gamepad.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
public struct GamePadState {
	[MarshalAs( UnmanagedType.ByValArray, SizeConst = 15 )]
	private readonly InputState[] states;

	[MarshalAs( UnmanagedType.ByValArray, SizeConst = 6 )]
	private readonly float[] axes;

	/// <summary>
	///     Gets the state of the specified <paramref name="button" />.
	/// </summary>
	/// <param name="button">The button to retrieve the state of.</param>
	/// <returns>The button state, either <see cref="InputState.Press" /> or <see cref="InputState.Release" />.</returns>
	public InputState GetButtonState( GamePadButton button ) => this.states[ (int) button ];

	/// <summary>
	///     Gets the value of the specified <paramref name="axis" />.
	/// </summary>
	/// <param name="axis">The axis to retrieve the value of.</param>
	/// <returns>The axis value, in the range of <c>-1.0</c> and <c>1.0</c> inclusive.</returns>
	public float GetAxis( GamePadAxis axis ) => this.axes[ (int) axis ];
}

/// <summary>
///     Describes the gamma ramp for a monitor.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
public struct GammaRamp {
	/// <summary>
	///     An array of value describing the response of the red channel.
	/// </summary>
	[MarshalAs( UnmanagedType.LPArray )]
	public ushort[] Red;

	/// <summary>
	///     An array of value describing the response of the green channel.
	/// </summary>
	[MarshalAs( UnmanagedType.LPArray )]
	public readonly ushort[] Green;

	/// <summary>
	///     An array of value describing the response of the blue channel.
	/// </summary>
	[MarshalAs( UnmanagedType.LPArray )]
	public readonly ushort[] Blue;

	/// <summary>
	///     The number of elements in each array.
	/// </summary>
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
}

/// <summary>
///     Used internally for marshalling
/// </summary>
[StructLayout( LayoutKind.Sequential )]
internal struct GammaRampInternal {
	public readonly nint Red;
	public readonly nint Green;
	public readonly nint Blue;
	public readonly int Size;

	public static explicit operator GammaRamp( GammaRampInternal ramp ) {
		int offset = 0;
		ushort[]? red = new ushort[ ramp.Size ];
		ushort[]? green = new ushort[ ramp.Size ];
		ushort[]? blue = new ushort[ ramp.Size ];
		for ( int i = 0; i < ramp.Size; i++, offset += sizeof( ushort ) ) {
			red[ i ] = unchecked((ushort) Marshal.ReadInt16( ramp.Red, offset ));
			green[ i ] = unchecked((ushort) Marshal.ReadInt16( ramp.Green, offset ));
			blue[ i ] = unchecked((ushort) Marshal.ReadInt16( ramp.Blue, offset ));
		}

		return new GammaRamp( red, green, blue );
	}
}

/// <summary>
///     Wrapper around a GLX context pointer.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
// ReSharper disable once InconsistentNaming
public struct GLXContext : IEquatable<GLXContext> {
	/// <summary>
	///     Describes a default/null instance.
	/// </summary>
	public static readonly GLXContext None;

	/// <summary>
	///     Internal pointer.
	/// </summary>
	private readonly nint handle;

	/// <summary>
	///     Performs an implicit conversion from <see cref="GLXContext" /> to <see cref="nint" />.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <returns>
	///     The result of the conversion.
	/// </returns>
	public static implicit operator nint( GLXContext context ) { return context.handle; }

	/// <summary>
	///     Returns a <see cref="string" /> that represents this instance.
	/// </summary>
	/// <returns>
	///     A <see cref="string" /> that represents this instance.
	/// </returns>
	public override string ToString() => this.handle.ToString();

	/// <summary>
	///     Determines whether the specified <see cref="GLXContext" />, is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="GLXContext" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="GLXContext" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals( GLXContext other ) => this.handle.Equals( other.handle );

	/// <summary>
	///     Determines whether the specified <see cref="object" />, is equal to this instance.
	/// </summary>
	/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals( object? obj ) {
		if ( obj is GLXContext context )
			return Equals( context );
		return false;
	}

	/// <summary>
	///     Returns a hash code for this instance.
	/// </summary>
	/// <returns>
	///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	/// </returns>
	public override int GetHashCode() => this.handle.GetHashCode();

	/// <summary>
	///     Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator ==( GLXContext left, GLXContext right ) { return left.Equals( right ); }

	/// <summary>
	///     Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator !=( GLXContext left, GLXContext right ) { return !left.Equals( right ); }
}

/// <summary>
///     Wrapper around a Window's HGLRC pointer.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
public struct HGLRC : IEquatable<HGLRC> {
	/// <summary>
	///     Describes a default/null instance.
	/// </summary>
	public static readonly HGLRC None;

	/// <summary>
	///     Internal pointer.
	/// </summary>
	private readonly nint handle;

	/// <summary>
	///     Performs an implicit conversion from <see cref="HGLRC" /> to <see cref="nint" />.
	/// </summary>
	/// <param name="hglrc">The hglrc.</param>
	/// <returns>
	///     The result of the conversion.
	/// </returns>
	public static implicit operator nint( HGLRC hglrc ) { return hglrc.handle; }

	/// <summary>
	///     Returns a <see cref="string" /> that represents this instance.
	/// </summary>
	/// <returns>
	///     A <see cref="string" /> that represents this instance.
	/// </returns>
	public override string ToString() => this.handle.ToString();

	/// <summary>
	///     Determines whether the specified <see cref="HGLRC" />, is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="HGLRC" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="HGLRC" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals( HGLRC other ) => this.handle.Equals( other.handle );

	/// <summary>
	///     Determines whether the specified <see cref="System.object" />, is equal to this instance.
	/// </summary>
	/// <param name="obj">The <see cref="System.object" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="System.object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals( object? obj ) {
		if ( obj is HGLRC hglrc )
			return Equals( hglrc );
		return false;
	}

	/// <summary>
	///     Returns a hash code for this instance.
	/// </summary>
	/// <returns>
	///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	/// </returns>
	public override int GetHashCode() => this.handle.GetHashCode();

	/// <summary>
	///     Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator ==( HGLRC left, HGLRC right ) { return left.Equals( right ); }

	/// <summary>
	///     Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator !=( HGLRC left, HGLRC right ) { return !left.Equals( right ); }
}

/// <summary>
///     Describes a basic image structure.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
public struct Image {
	/// <summary>
	///     The height, in pixels, of this image.
	/// </summary>
	public readonly int Width;

	/// <summary>
	///     The width, in pixels, of this image.
	/// </summary>
	public readonly int Height;

	/// <summary>
	///     Pointer to the RGBA pixel data of this image, arranged left-to-right, top-to-bottom.
	/// </summary>
	public readonly nint Pixels;

	/// <summary>
	///     Initializes a new instance of the <see cref="Image" /> struct.
	/// </summary>
	/// <param name="width">The height, in pixels, of this image.</param>
	/// <param name="height">The width, in pixels, of this image..</param>
	/// <param name="pixels">Pointer to the RGBA pixel data of this image, arranged left-to-right, top-to-bottom.</param>
	public Image( int width, int height, nint pixels ) {
		this.Width = width;
		this.Height = height;
		this.Pixels = pixels;
	}

	// TODO: Implement manual load of bmp
}

/// <summary>
///     Wrapper around a NSOpenGL context pointer.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
// ReSharper disable once InconsistentNaming
public struct NSOpenGLContext : IEquatable<NSOpenGLContext> {
	/// <summary>
	///     Describes a default/null instance.
	/// </summary>
	public static readonly NSOpenGLContext None;

	/// <summary>
	///     Internal pointer.
	/// </summary>
	private readonly nint handle;

	/// <summary>
	///     Performs an implicit conversion from <see cref="NSOpenGLContext" /> to <see cref="nint" />.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <returns>
	///     The result of the conversion.
	/// </returns>
	public static implicit operator nint( NSOpenGLContext context ) { return context.handle; }

	/// <summary>
	///     Returns a <see cref="string" /> that represents this instance.
	/// </summary>
	/// <returns>
	///     A <see cref="string" /> that represents this instance.
	/// </returns>
	public override string ToString() => this.handle.ToString();

	/// <summary>
	///     Determines whether the specified <see cref="NSOpenGLContext" />, is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="NSOpenGLContext" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="NSOpenGLContext" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals( NSOpenGLContext other ) => this.handle.Equals( other.handle );

	/// <summary>
	///     Determines whether the specified <see cref="object" />, is equal to this instance.
	/// </summary>
	/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals( object? obj ) {
		if ( obj is NSOpenGLContext context )
			return Equals( context );
		return false;
	}

	/// <summary>
	///     Returns a hash code for this instance.
	/// </summary>
	/// <returns>
	///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	/// </returns>
	public override int GetHashCode() => this.handle.GetHashCode();

	/// <summary>
	///     Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator ==( NSOpenGLContext left, NSOpenGLContext right ) { return left.Equals( right ); }

	/// <summary>
	///     Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator !=( NSOpenGLContext left, NSOpenGLContext right ) { return !left.Equals( right ); }
}

/// <summary>
///     Wrapper around a OSMesa context pointer.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
// ReSharper disable once InconsistentNaming
public struct OSMesaContext : IEquatable<OSMesaContext> {
	/// <summary>
	///     Describes a default/null instance.
	/// </summary>
	public static readonly OSMesaContext None;

	/// <summary>
	///     Internal pointer.
	/// </summary>
	private readonly nint handle;

	/// <summary>
	///     Performs an implicit conversion from <see cref="OSMesaContext" /> to <see cref="nint" />.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <returns>
	///     The result of the conversion.
	/// </returns>
	public static implicit operator nint( OSMesaContext context ) { return context.handle; }

	/// <summary>
	///     Returns a <see cref="string" /> that represents this instance.
	/// </summary>
	/// <returns>
	///     A <see cref="string" /> that represents this instance.
	/// </returns>
	public override string ToString() => this.handle.ToString();

	/// <summary>
	///     Determines whether the specified <see cref="OSMesaContext" />, is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="OSMesaContext" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="OSMesaContext" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals( OSMesaContext other ) => this.handle.Equals( other.handle );

	/// <summary>
	///     Determines whether the specified <see cref="object" />, is equal to this instance.
	/// </summary>
	/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals( object? obj ) {
		if ( obj is OSMesaContext context )
			return Equals( context );
		return false;
	}

	/// <summary>
	///     Returns a hash code for this instance.
	/// </summary>
	/// <returns>
	///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	/// </returns>
	public override int GetHashCode() => this.handle.GetHashCode();

	/// <summary>
	///     Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator ==( OSMesaContext left, OSMesaContext right ) { return left.Equals( right ); }

	/// <summary>
	///     Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator !=( OSMesaContext left, OSMesaContext right ) { return !left.Equals( right ); }
}

/// <summary>
///     Structure that describes a single video mode.
/// </summary>
[StructLayout( LayoutKind.Explicit, Size = 24, Pack = 4 )]
public struct VideoMode {
	/// <summary>
	///     The width, in screen coordinates, of the video mode.
	/// </summary>
	[FieldOffset( 0 )]
	public readonly int Width;

	/// <summary>
	///     The height, in screen coordinates, of the video mode.
	/// </summary>
	[FieldOffset( 4 )]
	public readonly int Height;

	/// <summary>
	///     The bit depth of the red channel of the video mode.
	/// </summary>
	[FieldOffset( 8 )]
	public readonly int RedBits;

	/// <summary>
	///     The bit depth of the green channel of the video mode.
	/// </summary>
	[FieldOffset( 12 )]
	public readonly int GreenBits;

	/// <summary>
	///     The bit depth of the blue channel of the video mode.
	/// </summary>
	[FieldOffset( 16 )]
	public readonly int BlueBits;

	/// <summary>
	///     The refresh rate, in Hz, of the video mode.
	/// </summary>
	[FieldOffset( 20 )]
	public readonly int RefreshRate;
}
