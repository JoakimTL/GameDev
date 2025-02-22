namespace Engine.Processing;

public interface IProcessDirection {
	/// <summary>
	/// The common process type between this type and the chosen type.
	/// </summary>
	Type ProcessType { get; }
}
