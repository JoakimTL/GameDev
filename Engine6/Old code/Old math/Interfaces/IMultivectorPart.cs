namespace Engine.Math.NewFolder.Interfaces;

public interface IMultivectorPart<TMultivector> where TMultivector : unmanaged
{
    TMultivector GetMultivector();
}