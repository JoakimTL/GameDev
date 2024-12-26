using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace _2dGame {
	class Program {


		static void Main( string[] args ) {
			List<float> a = new List<float>();

			for( float i = 0; i < Math.PI * 2; i += (float) Math.PI / 1000 ) {
				a.Add( (float) Math.Sin( i ) );
			}

			const float inv = 1f / (float) Math.PI;
			float Get( float i ) {
				i %= (float) Math.PI * 2;
				i *= inv;
				return a[ (int) ( i * 1000 ) ];
			}

			long time1 = Stopwatch.GetTimestamp();
			for( int i = 0; i < 200_000; i++ ) {
				float an = (float) Math.Sin( i );
			}
			long time2 = Stopwatch.GetTimestamp();
			for( int i = 0; i < 200_000; i++ ) {
				float an = Get( i );
			}
			long time3 = Stopwatch.GetTimestamp();

			Console.WriteLine( ( time2 - time1 ) / (double) TimeSpan.TicksPerSecond );
			Console.WriteLine( ( time3 - time2 ) / (double) TimeSpan.TicksPerSecond );

			Console.WriteLine( "Hello World!" );
			Console.ReadLine();
		}

	}
}
