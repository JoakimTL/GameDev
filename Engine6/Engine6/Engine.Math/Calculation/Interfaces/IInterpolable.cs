namespace Engine.Math.Calculation.Interfaces;

public interface IInterpolable<T, TScalar> where T : unmanaged where TScalar : unmanaged {
	static abstract T Interpolate( in T l, in T r, TScalar factor );
}