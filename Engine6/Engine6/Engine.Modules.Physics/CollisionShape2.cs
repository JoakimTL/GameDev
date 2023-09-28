using Engine.Data;

namespace Engine.Modules.Physics;

public class CollisionShape2 {

	private readonly CollisionShapeTemplate2 _template;
	private IMatrixProvider? _matrixProvider;

	public CollisionShape2( CollisionShapeTemplate2 template ) {
		this._template = template;
		this._matrixProvider = null;
	}

	public void SetMatrixProvider( IMatrixProvider? matrixProvider ) => this._matrixProvider = matrixProvider;

	public void CheckCollision() {
		//TODO IMPLEMENT...
	}

}
