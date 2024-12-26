using Engine.LinearAlgebra;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Engine.Graphics.Objects.Default.Lights.D3 {
	public class LightPoint : Light {

		private Vector3 translation;
		public Vector3 Translation { get => translation; set => SetTranslation( value ); }

		private float range;
		public float Range { get => range; set => SetRange( value ); }

		public bool HasShadows { get; private set; }

		public LightPoint( Vector4 color, Vector3 translation, float range, bool shadows ) : base( color ) {
			this.translation = translation;
			this.range = range;
			if( this.range <= 0 ) {
				Logging.Warning( "Light range was negative or 0, and has been set to 1." );
				this.range = 1;
			}
			HasShadows = shadows;
		}

		private void SetTranslation( Vector3 value ) {
			translation = value;
			InvokeChangeEvent();
		}

		private void SetRange( float value ) {
			if( value <= 0 )
				return;
			range = value;
			InvokeChangeEvent();
		}

	}
}