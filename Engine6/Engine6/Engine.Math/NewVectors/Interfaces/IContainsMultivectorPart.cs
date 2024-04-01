namespace Engine.Math.NewVectors.Interfaces;

public interface IContainsMultivectorPart<TMultivector, TPart>
	where TMultivector :
		unmanaged, IContainsMultivectorPart<TMultivector, TPart>
	where TPart :
		unmanaged {
    static abstract explicit operator TPart( in TMultivector part );
}