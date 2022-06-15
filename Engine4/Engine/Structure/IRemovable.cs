namespace Engine.Structure;

public interface IRemovable {
	event Action<IRemovable>? Removed;
}
