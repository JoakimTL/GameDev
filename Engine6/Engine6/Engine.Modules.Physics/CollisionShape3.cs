using Engine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Modules.Physics;
public class CollisionShape3 {

	private readonly CollisionShapeTemplate3 _template;
	private IMatrixProvider? _matrixProvider;

	public CollisionShape3( CollisionShapeTemplate3 template ) {
		this._template = template;
		this._matrixProvider = null;
	}

	public void SetMatrixProvider( IMatrixProvider? matrixProvider ) => this._matrixProvider = matrixProvider;

	public bool IsValid() => this._matrixProvider is not null;

	public void CheckCollision() {
		//TODO IMPLEMENT...
	}

}
