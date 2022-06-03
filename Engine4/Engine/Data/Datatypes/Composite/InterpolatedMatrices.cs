namespace Engine.Data.Datatypes.Composite;
public class InterpolatedMatrices : MatrixProviderBase {

	private readonly MatrixProviderBase _matrixA;
	private readonly MatrixProviderBase _matrixB;
	private float _interpolationFactor;

	public InterpolatedMatrices( MatrixProviderBase matrixA, MatrixProviderBase matrixB ) {
		this._matrixA = matrixA;
		this._matrixB = matrixB;
		this._interpolationFactor = 0;
		this._matrixA.MatrixChanged += UpdateMatrix;
		this._matrixB.MatrixChanged += UpdateMatrix;
		SetChanged();
	}

	private void UpdateMatrix( IMatrixProvider obj ) => SetChanged();

	public float InterpolationFactor {
		get => this._interpolationFactor;
		set {
			if ( value >= 0 && value <= 1 && value != this._interpolationFactor ) {
				this._interpolationFactor = value;
				SetChanged();
			}
		}
	}

	protected override void MatrixAccessed() => SetMatrix();

	private void SetMatrix() => this.Matrix = (this._matrixA.Matrix * this._interpolationFactor) + (this._matrixB.Matrix * ( 1 - this._interpolationFactor ));

	public void Dispose() {
		this._matrixA.MatrixChanged -= UpdateMatrix;
		this._matrixB.MatrixChanged -= UpdateMatrix;
	}
}
