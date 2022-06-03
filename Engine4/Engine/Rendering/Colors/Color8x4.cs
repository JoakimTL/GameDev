using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Colors;

/// <summary>
/// Color with 1 byte of precision per color.
/// </summary>
[StructLayout( LayoutKind.Explicit )]
public readonly struct Color8x4 {
	public const uint SIZE = 4;
	public const float BYTE_INV = 1f / byte.MaxValue;

	[FieldOffset( 0 )]
	private readonly byte _red;
	[FieldOffset( 1 )]
	private readonly byte _green;
	[FieldOffset( 2 )]
	private readonly byte _blue;
	[FieldOffset( 3 )]
	private readonly byte _alpha;

	public static readonly Color8x4 Zero = new( 0, 0, 0, 0 );
	public static readonly Color8x4 Red = new( byte.MaxValue, 0, 0, byte.MaxValue );
	public static readonly Color8x4 Green = new( 0, byte.MaxValue, 0, byte.MaxValue );
	public static readonly Color8x4 Blue = new( 0, 0, byte.MaxValue, byte.MaxValue );
	public static readonly Color8x4 Black = new( 0, 0, 0, byte.MaxValue );
	public static readonly Color8x4 White = new( byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue );

	public Color8x4( in Vector4 vec4 ) {
		Vector4 valueTrue = Vector4.Clamp( vec4, Vector4.Zero, Vector4.One ) * byte.MaxValue;
		this._red = (byte) MathF.Floor( valueTrue.X );
		this._green = (byte) MathF.Floor( valueTrue.Y );
		this._blue = (byte) MathF.Floor( valueTrue.Z );
		this._alpha = (byte) MathF.Floor( valueTrue.W );
	}

	public Color8x4( byte r, byte g, byte b, byte a ) {
		this._red = r;
		this._green = g;
		this._blue = b;
		this._alpha = a;
	}

	public override string ToString() => $"RGBA-{this._red:X2}{this._green:X2}{this._blue:X2}{this._alpha:X2}";
	public override bool Equals( object? obj ) {
		if ( obj != null && obj is Color8x4 col )
			return Equals( this, col );
		return false;
	}
	public override int GetHashCode() => HashCode.Combine( this._red, this._green, this._blue, this._alpha );
	public static bool Equals( Color8x4 a, Color8x4 b ) => a._red == b._red && a._green == b._green && a._blue == b._blue && a._alpha == b._alpha;
	public static bool operator ==( Color8x4 a, Color8x4 b ) => Equals( a, b );
	public static bool operator !=( Color8x4 a, Color8x4 b ) => !Equals( a, b );
	public static implicit operator Color8x4( in Vector4 v ) => new( v );
	public static implicit operator Vector4( Color8x4 v ) => new Vector4( v._red, v._green, v._blue, v._alpha ) * BYTE_INV;
}
