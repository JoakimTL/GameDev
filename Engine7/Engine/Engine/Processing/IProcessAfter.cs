namespace Engine.Processing;

internal interface IProcessAfter : IProcessDirection {
	/// <summary>
	/// The type to process after.
	/// </summary>
	Type AfterType { get; }
}
