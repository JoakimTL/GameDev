namespace Engine;

public interface IRemoveable {
	event Action<IRemoveable>? Removed;
}
