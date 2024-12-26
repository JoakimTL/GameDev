namespace Engine;

public interface IRemovable {
	bool Removed { get; }
	event RemovalHandler? OnRemoved;

	void Remove();
}

public delegate void RemovalHandler( IRemovable removable );