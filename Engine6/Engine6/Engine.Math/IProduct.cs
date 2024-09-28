namespace Engine;

public interface IProduct<TLeft, TRight, TResult> :
        IProductOperation<TLeft, TRight, TResult>,
        IProductOperator<TLeft, TRight, TResult>
    where TLeft :
        unmanaged, IProduct<TLeft, TRight, TResult>, IProductOperation<TLeft, TRight, TResult>, IProductOperator<TLeft, TRight, TResult>
    where TRight :
        unmanaged
    where TResult :
        unmanaged
{ }
