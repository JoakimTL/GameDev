using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Colors;

/// <summary>
/// Color with 1 byte of precision per color.
/// </summary>
[StructLayout( LayoutKind.Explicit )]
public readonly struct Color8x3 {
	public const uint SIZE = 3;
	public const float BYTE_INV = 1f / byte.MaxValue;

	[FieldOffset( 0 )]
	private readonly byte _red;
	[FieldOffset( 1 )]
	private readonly byte _green;
	[FieldOffset( 2 )]
	private readonly byte _blue;

	public static readonly Color8x3 Red = new( byte.MaxValue, 0, 0 );
	public static readonly Color8x3 Green = new( 0, byte.MaxValue, 0 );
	public static readonly Color8x3 Blue = new( 0, 0, byte.MaxValue );
	public static readonly Color8x3 Black = new( 0, 0, 0 );
	public static readonly Color8x3 White = new( byte.MaxValue, byte.MaxValue, byte.MaxValue );

	public Color8x3( in Vector3 vec3 ) {
		Vector3 valueTrue = Vector3.Clamp( vec3, Vector3.Zero, Vector3.One ) * byte.MaxValue;
		this._red = (byte) MathF.Floor( valueTrue.X );
		this._green = (byte) MathF.Floor( valueTrue.Y );
		this._blue = (byte) MathF.Floor( valueTrue.Z );
	}

	public Color8x3( byte r, byte g, byte b ) {
		this._red = r;
		this._green = g;
		this._blue = b;
	}

	public override string ToString() => $"RGB-{this._red:X2}{this._green:X2}{this._blue:X2}";
	public override bool Equals( object? obj ) {
		if ( obj != null && obj is Color8x3 col )
			return Equals( this, col );
		return false;
	}
	public override int GetHashCode() => HashCode.Combine( this._red, this._green, this._blue );
	public static bool Equals( Color8x3 a, Color8x3 b ) => a._red == b._red && a._green == b._green && a._blue == b._blue;
	public static bool operator ==( Color8x3 a, Color8x3 b ) => Equals( a, b );
	public static bool operator !=( Color8x3 a, Color8x3 b ) => !Equals( a, b );

	public static implicit operator Color8x3( in Vector3 v ) => new( v );

	public static implicit operator Vector3( Color8x3 v ) => new Vector3( v._red, v._green, v._blue ) * BYTE_INV;
}
