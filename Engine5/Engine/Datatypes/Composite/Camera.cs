namespace Engine.Datatypes.Composite;

public class Camera : MatrixProviderBase
{
    private MatrixProviderBase _view;
    private MatrixProviderBase _projection;

    public Camera(MatrixProviderBase view, MatrixProviderBase projection)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _projection = projection ?? throw new ArgumentNullException(nameof(projection));
        _view.MatrixChanged += UpdateMatrix;
        _projection.MatrixChanged += UpdateMatrix;
        SetChanged();
    }

    public MatrixProviderBase View
    {
        get => _view;
        set
        {
            if (value is null)
            {
                this.LogLine("Attempted to set view to null!", Log.Level.HIGH, stackLevel: 1);
                return;
            }
            _view.MatrixChanged -= UpdateMatrix;
            _view = value;
            _view.MatrixChanged += UpdateMatrix;
            SetChanged();
        }
    }

    public MatrixProviderBase Projection
    {
        get => _projection;
        set
        {
            if (value is null)
            {
                this.LogLine("Attempted to set projection to null!", Log.Level.HIGH, stackLevel: 1);
                return;
            }
            _projection.MatrixChanged -= UpdateMatrix;
            _projection = value;
            _projection.MatrixChanged += UpdateMatrix;
            SetChanged();
        }
    }

    private void UpdateMatrix(IMatrixProvider obj) => SetChanged();

    protected override void MatrixAccessed() => Matrix = _view.Matrix * _projection.Matrix;

}