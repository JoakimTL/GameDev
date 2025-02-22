namespace Engine.Processing;

public interface IProcessAfter : IProcessDirection {
	/// <summary>
	/// The type to process after.
	/// </summary>
	Type AfterType { get; }
}
