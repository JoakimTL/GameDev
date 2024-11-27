namespace Engine.Processing;

internal interface IProcessBefore : IProcessDirection {
	/// <summary>
	/// The type to process before.
	/// </summary>
	Type BeforeType { get; }
}
