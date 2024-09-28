namespace Engine.Math.NewFolder.Calculation.Interfaces;

public interface IMatrixInverseProduct<TLeft, TReturn> where TLeft : unmanaged where TReturn : unmanaged
{
    static abstract TReturn Inverse(in TLeft l);
}
