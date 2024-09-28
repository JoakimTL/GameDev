namespace Engine;

public interface IPartOfMultivector<TMultivector, TPart>
    where TMultivector :
        unmanaged, IPartOfMultivector<TMultivector, TMultivector>
    where TPart :
        unmanaged, IPartOfMultivector<TMultivector, TPart>
{
    TMultivector GetMultivector();
}