
namespace Engine;

public sealed class RemovableList : Identifiable {
	private readonly HashSet<IRemovable> _removables = [];

	public void Add( IRemovable removable ) {
		removable.OnRemoved += OnRemovableRemoved;
		this._removables.Add( removable );
	}
	public void Remove( IRemovable removable ) {
		removable.OnRemoved -= OnRemovableRemoved;
		this._removables.Remove( removable );
	}

	private void OnRemovableRemoved( IRemovable removable ) {
		removable.OnRemoved -= OnRemovableRemoved;
		this._removables.Remove( removable );
	}

	public void Clear( bool removeCleared ) {
		if (removeCleared)
			foreach (IRemovable removable in this._removables) {
				removable.OnRemoved -= OnRemovableRemoved;
				removable.Remove();
			}
		this._removables.Clear();
	}
}