namespace Engine;

public interface IRemovable {
	event Action<IRemovable>? Removed;
}
