namespace Engine.Math.Interfaces;

public interface IMultivectorPart<TMultivector> where TMultivector : unmanaged
{
    TMultivector GetMultivector();
}