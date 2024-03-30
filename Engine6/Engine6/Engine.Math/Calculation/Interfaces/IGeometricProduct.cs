namespace Engine.Math.Calculation.Interfaces;

public interface IGeometricProduct<TLeft, TRight, TReturn> where TLeft : unmanaged where TRight : unmanaged where TReturn : unmanaged {
	static abstract TReturn Multiply( in TLeft l, in TRight r );
}
