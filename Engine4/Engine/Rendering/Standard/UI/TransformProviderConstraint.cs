namespace Engine.Rendering.Standard.UI;

public class TransformProviderConstraint<T> : IUIConstraint<T> where T : class {
	public int ExecutionOrder { get; }
	private readonly Action<float, float, T> _transformApplicator;

	public TransformProviderConstraint(Action<float, float, T> transformApplicator, int executionOrder = 0) {
		this._transformApplicator = transformApplicator ?? throw new ArgumentNullException( nameof( transformApplicator ) );
		this.ExecutionOrder = executionOrder;
	}

	public void Apply( float time, float deltaTime, T data ) => this._transformApplicator.Invoke(time, deltaTime, data);
}