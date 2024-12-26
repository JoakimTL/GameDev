using Engine.LinearAlgebra;
using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles {
	public class AnimationTarget {
		public readonly int textureIndex;
		public readonly Vector4b targetDiffuse;
		public readonly Vector4b targetGlow;
		public readonly float duration, delay;
		public readonly InterpolationMethod interpolation;

		public AnimationTarget( int index, Vector4b diffuse, Vector4b glow, float duration, float delay, InterpolationMethod interp ) {
			textureIndex = index;
			targetDiffuse = diffuse;
			targetGlow = glow;
			this.duration = duration;
			this.delay = delay;
			this.interpolation = interp;
		}

		public AnimationTarget( int index, Vector4b diffuse, Vector4b glow, float duration, float delay ) {
			textureIndex = index;
			targetDiffuse = diffuse;
			targetGlow = glow;
			this.duration = duration;
			this.delay = delay;
			this.interpolation = InterpolationMethods.CosineInterpolation;
		}

		public AnimationTarget( int index, Vector4i diffuse, Vector4i glow, float duration, float delay, InterpolationMethod interp ) {
			textureIndex = index;
			targetDiffuse = diffuse.AsByte;
			targetGlow = glow.AsByte;
			this.duration = duration;
			this.delay = delay;
			this.interpolation = interp;
		}

		public AnimationTarget( int index, Vector4i diffuse, Vector4i glow, float duration, float delay ) {
			textureIndex = index;
			targetDiffuse = diffuse.AsByte;
			targetGlow = glow.AsByte;
			this.duration = duration;
			this.delay = delay;
			this.interpolation = InterpolationMethods.CosineInterpolation;
		}
	}
}
