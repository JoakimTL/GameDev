using System;

namespace Engine.Utilities.Time {
	public class Sampler32 {

		public Watch32 Watch {
			get; private set;
		}
		public float this[ int i ] { get { return Samples[ ( ( i % Samples.Length ) + Samples.Length ) % Samples.Length ]; } }
		public float[] Samples {
			get; private set;
		}
		private int currentSample;

		public Sampler32( Watch32 watch, int nSamples = 30 ) {
			this.Watch = watch;
			Samples = new float[ nSamples ];
			currentSample = 0;
		}

		public void Record() {
			Samples[ ( currentSample++ ) % Samples.Length ] = Watch.ElapsedTime;
			Watch.Zero();
		}

		public void ZeroAll() {
			for( int i = 0; i < Samples.Length; i++ ) {
				Samples[ i ] = 0;
			}
		}

		public double GetAverage() {
			double a = 0;
			int n = 0;
			for( int i = 0; i < Samples.Length; i++ ) {
				if( Samples[ i ] > 0 ) {
					a += Samples[ i ];
					n++;
				}
			}
			if( n > 0 )
				return a / n;
			return a;
		}

		public double GetAverageMillis() {
			return Math.Round( GetAverage() * 100000 ) / 100;
		}

		public double GetAverageInverse() {
			return Math.Round( ( 1d / GetAverage() ) * 100 ) / 100;
		}

	}
}
