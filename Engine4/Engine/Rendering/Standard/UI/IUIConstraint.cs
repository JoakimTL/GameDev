using Engine.Data.Datatypes.Transforms;

namespace Engine.Rendering.Standard.UI;

public interface IUIConstraint {
	int ExecutionOrder { get; }
	void Apply( float time, float deltaTime, Transform2 transform );
}