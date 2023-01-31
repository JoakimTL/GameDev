using System.Numerics;

namespace Engine.Datatypes;

public abstract class MatrixProviderBase : Identifiable, IMatrixProvider
{
    private Matrix4x4 _matrix;
    private Matrix4x4 _inverseMatrix;
    private bool _changed;
    public event Action<IMatrixProvider>? MatrixChanged;

    protected MatrixProviderBase()
    {
        _matrix = Matrix4x4.Identity;
        _inverseMatrix = Matrix4x4.Identity;
    }

    protected void SetChanged()
    {
        _changed = true;
        MatrixChanged?.Invoke(this);
    }

    protected abstract void MatrixAccessed();

    public Matrix4x4 Matrix
    {
        get
        {
            if (_changed)
            {
                MatrixAccessed();
                _changed = false;
            }
            return _matrix;
        }
        protected set
        {
            if (value == _matrix)
                return;
            _matrix = value;
            if (!Matrix4x4.Invert(_matrix, out _inverseMatrix))
                _inverseMatrix = Matrix4x4.Identity;
        }
    }

    public Matrix4x4 InverseMatrix
    {
        get
        {
            if (_changed)
            {
                MatrixAccessed();
                _changed = false;
            }
            return _inverseMatrix;
        }
    }
}
