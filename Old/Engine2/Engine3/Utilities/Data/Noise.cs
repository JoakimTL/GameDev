using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Utilities.Data {
	public class NoisePattern {

		public int Seed { get; set; }
		public float Baseline { get; set; }
		public float Amplitude { get; set; }
		public float Smoothness { get; set; }
		public InterpolationMethod Interpolation { get; private set; }

		public NoisePattern( int seed, float baseline, float amp, float smoothness, InterpolationMethod met ) {
			Seed = seed;
			Baseline = baseline;
			Amplitude = amp;
			Smoothness = smoothness;
			Interpolation = met;
		}

		public float GetValue( float x, float y ) {
			return Noise.GetPerlinNoise( Seed, x, y, Interpolation, Baseline, Amplitude, Smoothness );
		}

		public void SetInterpolationMethod( InterpolationMethod met ) {
			if( met != null )
				Interpolation = met;
		}

	}

	public static class Noise {

		public static float GetPerlinNoise( int seed, float x, float y, InterpolationMethod interpolation, float baseline, float amp, float smoothness ) {
			if( amp == 0 )
				return baseline;
			float smooth = 1f / smoothness;
			return baseline + GetInterpolatedNoise( seed, x * smooth, y * smooth, interpolation ) * amp;

		}

		public static float GetInterpolatedNoise( int seed, float x, float y, InterpolationMethod interpolation ) {
			int lowX = (int) Math.Floor( x );
			int lowY = (int) Math.Floor( y );
			float fractionX = x - lowX;
			float fractionY = y - lowY;

			float v1 = GetSmoothNoise( seed, lowX, lowY );
			float v2 = GetSmoothNoise( seed, lowX + 1, lowY );
			float v3 = GetSmoothNoise( seed, lowX, lowY + 1 );
			float v4 = GetSmoothNoise( seed, lowX + 1, lowY + 1 );

			float iX = interpolation.Invoke( fractionX );
			float i1 = v1 * ( 1 - iX ) + v2 * iX;
			float i2 = v3 * ( 1 - iX ) + v4 * iX;

			float iY = interpolation.Invoke( fractionY );
			return i1 * ( 1 - iY ) + i2 * iY;

		}

		private const float CORDIV = 1f / 16;
		private const float SIDDIV = 1f / 8;
		private const float CENDIV = 1f / 4;
		public static float GetSmoothNoise( int seed, int x, int y ) {
			float nCorner = (
				GetRandomNumber( seed, x - 1, y - 1 ) +
				GetRandomNumber( seed, x + 1, y - 1 ) +
				GetRandomNumber( seed, x + 1, y + 1 ) +
				GetRandomNumber( seed, x - 1, y + 1 )
				) * CORDIV;
			float nSide = (
				GetRandomNumber( seed, x - 1, y ) +
				GetRandomNumber( seed, x + 1, y ) +
				GetRandomNumber( seed, x, y - 1 ) +
				GetRandomNumber( seed, x, y + 1 )
				) * SIDDIV;
			float nCenter = GetRandomNumber( seed, x, y ) * CENDIV;

			return nCenter + nCorner + nSide;
		}

		private static uint bitRotate( uint x ) {
			const int bits = 16;
			return ( x << bits ) | ( x >> ( 32 - bits ) );
		}

		private const double INTDIV = 1d / uint.MaxValue;
		public static float GetRandomNumber( int seed, int x, int y ) {
			unchecked {
				uint num = (uint) seed;
				num = num * 541 + (uint) x;
				num = bitRotate( num );
				num = num * 809 + (uint) y;
				num = bitRotate( num );
				num = num * 673 + 0;
				num = bitRotate( num );
				num = num * 541 + (uint) x;
				num = bitRotate( num );
				num = num * 809 + (uint) y;
				num = bitRotate( num );
				num = num * 673 + 1;
				num = bitRotate( num );
				num = num * 541 + (uint) x;
				num = bitRotate( num );
				num = num * 809 + (uint) y;
				num = bitRotate( num );
				num = num * 673 + 2;
				num = bitRotate( num );
				return (float) ( num * INTDIV ) * 2 - 1;
			}
		}

	}
}
