namespace Engine.Datatypes.Composite;
public class InterpolatedMatrices : MatrixProviderBase
{

    private readonly MatrixProviderBase _matrixA;
    private readonly MatrixProviderBase _matrixB;
    private float _interpolationFactor;

    public InterpolatedMatrices(MatrixProviderBase matrixA, MatrixProviderBase matrixB)
    {
        _matrixA = matrixA;
        _matrixB = matrixB;
        _interpolationFactor = 0;
        _matrixA.MatrixChanged += UpdateMatrix;
        _matrixB.MatrixChanged += UpdateMatrix;
        SetChanged();
    }

    private void UpdateMatrix(IMatrixProvider obj) => SetChanged();

    public float InterpolationFactor
    {
        get => _interpolationFactor;
        set
        {
            if (value >= 0 && value <= 1 && value != _interpolationFactor)
            {
                _interpolationFactor = value;
                SetChanged();
            }
        }
    }

    protected override void MatrixAccessed() => SetMatrix();

    private void SetMatrix() => Matrix = (_matrixA.Matrix * _interpolationFactor) + (_matrixB.Matrix * (1 - _interpolationFactor));

    public void Dispose()
    {
        _matrixA.MatrixChanged -= UpdateMatrix;
        _matrixB.MatrixChanged -= UpdateMatrix;
    }
}
