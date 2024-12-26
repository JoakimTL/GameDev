using Engine.LinearAlgebra;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Engine.Graphics.Objects.Default.Lights.D3 {
	public class LightDirectional : Light {

		private Quaternion rotation;
		public Quaternion Rotation { get => rotation; set => SetRotation( value ); }

		public LightDirectional( Vector4 color, float yaw, float pitch ) : base( color ) {
			rotation = Quaternion.Identity;
			rotation = Quaternion.FromAxisAngle( Vector3.UnitX, (float) ( pitch / 180 * Math.PI ) ) * rotation;
			rotation = Quaternion.FromAxisAngle( Vector3.UnitY, (float) ( yaw / 180 * Math.PI ) ) * rotation;
			rotation = rotation.Normalized;
		}

		private void SetRotation( Quaternion value ) {
			rotation = value;
			InvokeChangeEvent();
		}


	}
}