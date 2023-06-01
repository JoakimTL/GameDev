using Engine.Rendering.Contexts.Input;
using Engine.Structure.Interfaces;
using GlfwBinding.Enums;

namespace Engine.GlobalServices;
public class InputService : Identifiable, IGlobalService
{

    private bool[] _keys;
    private MouseState _mouseState;

    public InputService()
    {
        _keys = new bool[2048];
        _mouseState = new(new());
    }

    public bool this[Keys key]
    {
        get => _keys[(int)key];
        internal set => _keys[(int)key] = value;
    }

    public MouseState Mouse => _mouseState;

    internal void SetMouseData(MouseData data) => _mouseState = new(data);


}
