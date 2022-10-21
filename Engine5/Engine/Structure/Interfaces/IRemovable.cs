namespace Engine.Structure.Interfaces;

public interface IRemovable
{
    event Action<IRemovable> Removed;
}
