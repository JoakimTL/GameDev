namespace Engine.Rendering.Contexts.Objects;

public interface ISceneInstanceData : IDisposable {
	/// <summary>
	/// Number of active instances
	/// </summary>
	uint ActiveInstances { get; }
	/// <summary>
	/// Offset for the first instance. This is an element-size offset, not byte offset.
	/// </summary>
	uint DataOffset { get; }
	event Action? Changed;
}
