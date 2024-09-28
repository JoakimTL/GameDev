namespace Engine.Math.NewFolder.Calculation.Interfaces;

public interface IMatrixDeterminant<TMatrix, T> where TMatrix : unmanaged where T : unmanaged
{
    static abstract T GetDeterminant(in TMatrix m);
}