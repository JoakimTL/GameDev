namespace Engine.Math.Calculation.Interfaces;

public interface IInnerProduct<T, TReturn> where T : unmanaged where TReturn : unmanaged {
	static abstract TReturn Dot( in T l, in T r );
}