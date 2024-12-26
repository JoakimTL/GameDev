using Engine.LinearAlgebra;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Engine.Graphics.Objects.Default.Lights.D3 {
	public class LightSpot : LightPoint {

		private Quaternion rotation;
		public Quaternion Rotation { get => rotation; set => SetRotation( value ); }

		private float cutoff;
		public float Cutoff { get => cutoff; set => SetCutoff( value ); }

		public LightSpot( Vector4 color, Vector3 translation, float range, float yaw, float pitch, float cutoff, bool shadows ) : base( color, translation, range, shadows ) {
			this.cutoff = cutoff;
			rotation = Quaternion.Identity;
			rotation = Quaternion.FromAxisAngle( Vector3.UnitX, (float) ( pitch / 180 * Math.PI ) ) * rotation;
			rotation = Quaternion.FromAxisAngle( Vector3.UnitY, (float) ( yaw / 180 * Math.PI ) ) * rotation;
			rotation = rotation.Normalized;
		}

		private void SetRotation( Quaternion value ) {
			rotation = value;
			InvokeChangeEvent();
		}

		private void SetCutoff( float value ) {
			cutoff = value;
			InvokeChangeEvent();
		}

	}
}