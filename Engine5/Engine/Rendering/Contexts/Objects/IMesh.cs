﻿namespace Engine.Rendering.Contexts.Objects;

public interface IMesh {
	/// <summary>
	/// Number of indices in the mesh.
	/// </summary>
	uint ElementCount { get; }
	/// <summary>
	/// Offset for the first element index. This is an element-size offset, not byte offset.
	/// </summary>
	uint ElementOffset { get; }
	/// <summary>
	/// Offset of the first vertex. This is an element-size offset, not byte offset.
	/// </summary>
	uint VertexOffset { get; }
	event Action? Changed;
}