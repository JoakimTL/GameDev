﻿using Engine.Data;
using Engine.Math.NewFolder;

namespace Engine.Modules.Physics;

public class CollisionShape2 {

	private readonly CollisionShapeTemplate2 _template;
	private IMatrixProvider? _matrixProvider;
	private bool _matrixChanged;

	public CollisionShape2( CollisionShapeTemplate2 template ) {
		this._template = template;
		this._matrixProvider = null;
	}

	public void SetMatrixProvider( IMatrixProvider? matrixProvider ) {
		if ( this._matrixProvider is not null )
			this._matrixProvider.MatrixChanged -= OnMatrixChanged;
		this._matrixProvider = matrixProvider;
		if ( this._matrixProvider is not null )
			this._matrixProvider.MatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged( IMatrixProvider provider ) => _matrixChanged = true;

	public bool IsValid() => this._matrixProvider is not null;

	public bool TryGetAAb( out AABB2<double> aabb ) {
		throw new NotImplementedException();
	}

	public void CheckCollision() {
		//TODO IMPLEMENT...
	}

}
