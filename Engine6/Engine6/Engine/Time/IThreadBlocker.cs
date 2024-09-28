namespace Engine.Time;
public interface IThreadBlocker : ICancellable {
	/// <param name="milliseconds">Time in milliseconds this method should block for</param>
	/// <returns>True if the block timed out, false if the block was cancelled.</returns>
	bool Block( uint milliseconds );
}
