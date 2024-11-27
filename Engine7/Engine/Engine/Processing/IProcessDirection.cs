namespace Engine.Processing;

internal interface IProcessDirection {
	/// <summary>
	/// The common process type between this type and the chosen type.
	/// </summary>
	Type ProcessType { get; }
}
