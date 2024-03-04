namespace Engine.Math;

public interface IGeometricProductOperator<TSelf, TOther, TResult> where TSelf : unmanaged, IGeometricProductOperator<TSelf, TOther, TResult> {
	static abstract TResult operator *( in TSelf l, in TOther r );
}
