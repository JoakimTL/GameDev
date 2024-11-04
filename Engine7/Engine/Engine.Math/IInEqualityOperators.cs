namespace Engine;

public interface IInEqualityOperators<TLeft, TRight, TResult>
	where TLeft :
		IInEqualityOperators<TLeft, TRight, TResult> {
	static abstract TResult operator ==( in TLeft l, in TRight r );
	static abstract TResult operator !=( in TLeft l, in TRight r );
}