using System.Numerics;

namespace Engine.Math.NewFolder.Operations.Interfaces;

public interface IAABBVolumeOperations<T, TRotor, TVector, TScalar> where T : unmanaged where TRotor : unmanaged where TVector : unmanaged where TScalar : unmanaged, INumber<TScalar>
{
    static abstract TScalar GetSurfaceArea(in T aabb);
    static abstract TScalar GetVolume(in T aabb);
    static abstract IEnumerable<TVector> GetPointsInAreaExclusive(T aabb, TScalar increment, TRotor planeRotor, TVector basisFromMiniman);
    static abstract IEnumerable<TVector> GetPointsInAreaInclusive(T aabb, TScalar increment, TRotor planeRotor, TVector basisFromMinima);
    static abstract IEnumerable<TVector> GetPointsInAreaExclusiveExcept(T aabb, T other, TScalar increment, TRotor planeRotor, TVector basisFromMinima);
    static abstract IEnumerable<TVector> GetPointsInAreaIncusiveExcept(T aabb, T other, TScalar increment, TRotor planeRotor, TVector basisFromMinima);
    static abstract IEnumerable<TVector> GetPointsInVolumeExclusive(T aabb, TScalar increment);
    static abstract IEnumerable<TVector> GetPointsInVolumeInclusive(T aabb, TScalar increment);
    static abstract IEnumerable<TVector> GetPointsInVolumeExclusiveExcept(T aabb, T other, TScalar increment);
    static abstract IEnumerable<TVector> GetPointsInVolumeIncusiveExcept(T aabb, T other, TScalar increment);
}

