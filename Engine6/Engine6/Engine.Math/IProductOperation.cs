namespace Engine;

public interface IProductOperation<TLeft, TRight, TResult>
    where TLeft :
        unmanaged, IProductOperation<TLeft, TRight, TResult>
    where TRight :
        unmanaged
    where TResult :
        unmanaged
{
    TResult Multiply(in TRight r);
}
