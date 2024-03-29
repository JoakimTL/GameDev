﻿namespace Engine.Data.Datatypes.Composite;

public class Camera : MatrixProviderBase {
	private MatrixProviderBase _view;
	private MatrixProviderBase _projection;

	public Camera( MatrixProviderBase view, MatrixProviderBase projection ) {
		this._view = view ?? throw new ArgumentNullException( nameof( view ) );
		this._projection = projection ?? throw new ArgumentNullException( nameof( projection ) );
		this._view.MatrixChanged += UpdateMatrix;
		this._projection.MatrixChanged += UpdateMatrix;
		SetChanged();
	}

	public MatrixProviderBase View {
		get => this._view;
		set {
			if ( value is null ) {
				this.LogLine( "Attempted to set view to null!", Log.Level.HIGH, stackLevel: 1 );
				return;
			}
			this._view.MatrixChanged -= UpdateMatrix;
			this._view = value;
			this._view.MatrixChanged += UpdateMatrix;
			SetChanged();
		}
	}

	public MatrixProviderBase Projection {
		get => this._projection;
		set {
			if ( value is null ) {
				this.LogLine( "Attempted to set projection to null!", Log.Level.HIGH, stackLevel: 1 );
				return;
			}
			this._projection.MatrixChanged -= UpdateMatrix;
			this._projection = value;
			this._projection.MatrixChanged += UpdateMatrix;
			SetChanged();
		}
	}

	private void UpdateMatrix( IMatrixProvider obj ) => SetChanged();

	protected override void MatrixAccessed() => this.Matrix = this._view.Matrix * this._projection.Matrix;

}