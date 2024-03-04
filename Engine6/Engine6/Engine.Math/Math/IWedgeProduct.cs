namespace Engine.Math.Math;

public interface IWedgeProduct<TLeft, TRight, TReturn> where TLeft : unmanaged where TRight : unmanaged where TReturn : unmanaged {
	static abstract TReturn Wedge( in TLeft l, in TRight r );
}
