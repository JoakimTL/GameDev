using System;

namespace EM {
	public class Complexd {

		public double X, Y;

		public double Radius {
			get {
				return Math.Sqrt( X * X + Y * Y );
			}
			set {
				if( X == 0 && Y == 0 ) {
					double angle = Angle;
					X = Math.Cos( angle ) * value;
					Y = Math.Sin( angle ) * value;
				} else {
					double co = value / Radius;
					X *= co;
					Y *= co;
				}
			}
		}

		public double Angle {
			get {
				return Math.Atan2( Y, X );
			}
			set {
				double radius = Radius;
				X = Math.Cos( value ) * radius;
				Y = Math.Sin( value ) * radius;
			}
		}

		public Complexd( double x, double y ) {
			X = x;
			Y = y;
		}

		public static Complexd FromPolar( double angle, double radius ) {
			double a = angle;
			return new Complexd( Math.Cos( a ) * radius, Math.Sin( a ) * radius );
		}

		public static Complexd operator +( Complexd a, Complexd b ) {
			return new Complexd( a.X + b.X, a.Y + b.Y );
		}

		public static Complexd operator -( Complexd a ) {
			return new Complexd( -a.X, -a.Y );
		}

		public static Complexd operator -( Complexd a, Complexd b ) {
			return a + ( -b );
		}

		public static Complexd operator *( Complexd a, Complexd b ) {
			return new Complexd( a.X * b.X - a.Y * b.Y, a.X * b.Y + a.Y * b.X );
		}

		public static Complexd operator /( Complexd a, Complexd b ) {
			double det = 1d / ( b.X * b.X + b.Y * b.Y );
			return new Complexd( ( a.X * b.X + a.Y * b.Y ) * det, ( b.X * a.Y - b.Y * a.X ) * det );
		}

		public string GetCartesian() {
			return $"{X.ToString( "N4" )} + i{Y.ToString( "N4" )}";
		}

		public string GetPolar() {
			return $"{Radius.ToString( "N4" )}∠({Angle.ToString( "N4" )})°";
		}

		public override string ToString() {
			return $"{X.ToString( "N4" )} + i{Y.ToString( "N4" )} = {Radius.ToString( "N4" )}∠({Angle.ToString( "N4" )})°";
		}
	}
}
