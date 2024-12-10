using Engine.Logging;

namespace Engine.Transforms.Camera;

public class Camera : MatrixProviderBase<float> {
	private MatrixProviderBase<float> _view;
	private MatrixProviderBase<float> _projection;

	public Camera( MatrixProviderBase<float> view, MatrixProviderBase<float> projection ) {
		this._view = view ?? throw new ArgumentNullException( nameof( view ) );
		this._projection = projection ?? throw new ArgumentNullException( nameof( projection ) );
		this._view.OnMatrixChanged += UpdateMatrix;
		this._projection.OnMatrixChanged += UpdateMatrix;
		SetChanged();
	}

	public MatrixProviderBase<float> View {
		get => this._view;
		set {
			if (value is null) {
				this.LogLine( "Attempted to set view to null!", Log.Level.HIGH, stackLevel: 1 );
				return;
			}
			this._view.OnMatrixChanged -= UpdateMatrix;
			this._view = value;
			this._view.OnMatrixChanged += UpdateMatrix;
			SetChanged();
		}
	}

	public MatrixProviderBase<float> Projection {
		get => this._projection;
		set {
			if (value is null) {
				this.LogLine( "Attempted to set projection to null!", Log.Level.HIGH, stackLevel: 1 );
				return;
			}
			this._projection.OnMatrixChanged -= UpdateMatrix;
			this._projection = value;
			this._projection.OnMatrixChanged += UpdateMatrix;
			SetChanged();
		}
	}

	private void UpdateMatrix( IMatrixProvider<float> obj ) => SetChanged();

	protected override void MatrixAccessed() => this.Matrix = this._view.Matrix * this._projection.Matrix;

}