namespace Engine.Time;

public interface ICancellable {
	bool Cancelled { get; }
	void Cancel();
}
