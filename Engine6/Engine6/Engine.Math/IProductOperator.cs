namespace Engine;

public interface IProductOperator<TLeft, TRight, TResult>
    where TLeft :
        unmanaged, IProductOperation<TLeft, TRight, TResult>, IProductOperator<TLeft, TRight, TResult>
    where TRight :
        unmanaged
    where TResult :
        unmanaged
{
    static abstract TResult operator *(in TLeft l, in TRight r);
}
