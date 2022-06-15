using Engine.Data.Datatypes.Transforms;

namespace Engine.Rendering.Standard.UI;

public interface IUIConstraint<T> where T : class {
	int ExecutionOrder { get; }
	void Apply( float time, float deltaTime, T data );
}