using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Lights.D3 {
	public abstract class Light {
		private Vector4 color;
		public Vector4 Color { get => color; set => SetColor( value ); }

		public event Action AttributesChanged;

		public Light( Vector4 color ) {
			Color = color;
		}

		private void SetColor( Vector4 value ) {
			if( value.X < 0 || value.Y < 0 || value.Z < 0 || value.X > 1 || value.Y > 1 || value.Z > 1 )
				return;
			if( value.W < 0 )
				return;
			color = value;
			InvokeChangeEvent();
		}

		protected void InvokeChangeEvent() {
			AttributesChanged?.Invoke();
		}
	}
}
