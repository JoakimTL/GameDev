namespace Engine.Math.Calculation.Interfaces;

public interface IOuterProduct<TLeft, TRight, TReturn> where TLeft : unmanaged where TRight : unmanaged where TReturn : unmanaged {
	static abstract TReturn Wedge( in TLeft l, in TRight r );
}
