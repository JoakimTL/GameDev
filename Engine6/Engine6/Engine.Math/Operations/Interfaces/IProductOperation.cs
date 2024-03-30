namespace Engine.Math.Operations.Interfaces;

public interface IProductOperation<TLeft, TRight, TResult> where TLeft : unmanaged where TRight : unmanaged where TResult : unmanaged {
	static abstract TResult Multiply( in TLeft l, in TRight r );
}
