using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Colors;
/// <summary>
/// Color with 2 bytes of precision per color.
/// </summary>
[StructLayout( LayoutKind.Explicit )]
public readonly struct Color16x4 {
	public const uint SIZE = 8;
	public const float USHORT_INV = 1f / ushort.MaxValue;

	[FieldOffset( 0 )]
	private readonly ushort _red;
	[FieldOffset( 2 )]
	private readonly ushort _green;
	[FieldOffset( 4 )]
	private readonly ushort _blue;
	[FieldOffset( 6 )]
	private readonly ushort _alpha;

	public static readonly Color16x4 Zero = new( 0, 0, 0, 0 );
	public static readonly Color16x4 Red = new( ushort.MaxValue, 0, 0, ushort.MaxValue );
	public static readonly Color16x4 Green = new( 0, ushort.MaxValue, 0, ushort.MaxValue );
	public static readonly Color16x4 Blue = new( 0, 0, ushort.MaxValue, ushort.MaxValue );
	public static readonly Color16x4 Black = new( 0, 0, 0, ushort.MaxValue );
	public static readonly Color16x4 White = new( ushort.MaxValue, ushort.MaxValue, ushort.MaxValue, ushort.MaxValue );

	public Color16x4( ushort r, ushort g, ushort b, ushort a ) {
		this._red = r;
		this._green = g;
		this._blue = b;
		this._alpha = a;
	}

	public Color16x4( in Vector4 vec4 ) {
		Vector4 valueTrue = Vector4.Clamp( vec4, Vector4.Zero, Vector4.One ) * ushort.MaxValue;
		this._red = (ushort) MathF.Floor( valueTrue.X );
		this._green = (ushort) MathF.Floor( valueTrue.Y );
		this._blue = (ushort) MathF.Floor( valueTrue.Z );
		this._alpha = (ushort) MathF.Floor( valueTrue.W );
	}

	public override string ToString() => $"RGBA16-{this._red:X2}{this._green:X2}{this._blue:X2}{this._alpha:X2}";
	public override bool Equals( object? obj ) {
		if ( obj != null && obj is Color16x4 col )
			return Equals( this, col );
		return false;
	}
	public override int GetHashCode() => HashCode.Combine( this._red, this._green, this._blue, this._alpha );
	public static bool Equals( Color16x4 a, Color16x4 b ) => a._red == b._red && a._green == b._green && a._blue == b._blue && a._alpha == b._alpha;
	public static bool operator ==( Color16x4 a, Color16x4 b ) => Equals( a, b );
	public static bool operator !=( Color16x4 a, Color16x4 b ) => !Equals( a, b );
	public static implicit operator Color16x4( in Vector4 v ) => new( v );
	public static implicit operator Vector4( Color16x4 v ) => new Vector4( v._red, v._green, v._blue, v._alpha ) * USHORT_INV;
}
