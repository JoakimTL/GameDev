namespace Engine.Math.Interfaces;

public interface IProductOperator<TSelf, TOther, TResult> where TSelf : unmanaged, IProductOperator<TSelf, TOther, TResult>
{
    static abstract TResult operator *(in TSelf l, in TOther r);
}
