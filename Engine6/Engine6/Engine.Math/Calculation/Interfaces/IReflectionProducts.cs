namespace Engine.Math.Calculation.Interfaces;

public interface IReflectionProducts<T, TNormal> where T : unmanaged {
	static abstract T ReflectNormal( in T v, in TNormal normal );
	static abstract T ReflectMirror( in T v, in TNormal mirrorNormal );
}