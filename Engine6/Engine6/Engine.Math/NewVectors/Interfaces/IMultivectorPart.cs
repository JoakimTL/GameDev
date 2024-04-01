namespace Engine.Math.NewVectors.Interfaces;

public interface IMultivectorPart<TMultivector, TMultivectorPart>
    where TMultivector :
        unmanaged, IMultivectorPart<TMultivector, TMultivector>
    where TMultivectorPart :
        unmanaged, IMultivectorPart<TMultivector, TMultivectorPart>
{
    static abstract TMultivector GetMultivector(in TMultivectorPart part);
}