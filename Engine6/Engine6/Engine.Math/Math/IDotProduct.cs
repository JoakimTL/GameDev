namespace Engine.Math.Math;

public interface IDotProduct<T, TReturn> where T : unmanaged where TReturn : unmanaged {
	static abstract TReturn Dot( in T l, in T r );
}