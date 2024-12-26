namespace Engine.LMath {
	public interface ILANormalizable<T> {
		T Normalized {
			get;
		}
	}

	public interface ILAMeasurable {
		float Length {
			get;
		}
		float LengthSquared {
			get;
		}
	}
}
