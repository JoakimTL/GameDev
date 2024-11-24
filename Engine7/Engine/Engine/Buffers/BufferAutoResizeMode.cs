namespace Engine.Buffers;

public enum BufferAutoResizeMode {
	/// <summary>
	/// No automatic resizing
	/// </summary>
	DISABLED,
	/// <summary>
	/// Adds the initial size to the current size.
	/// </summary>
	LINEAR,
	/// <summary>
	/// Doubles the current size.
	/// </summary>
	DOUBLE
}
