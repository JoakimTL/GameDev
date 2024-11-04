namespace Engine.Time;

public interface ITickSupplier {
	static abstract long Frequency { get; }
	static abstract long Ticks { get; }
}
