namespace Engine.Rendering.Objects.Assets;

/// <summary>
/// This reference container must remain in memory, otherwise the reference it holds might be disposed while the reference is in use.
/// </summary>
public class ReferenceContainer<T> {
	internal ReferenceContainer( T value ) {
		this.Value = value;
	}

	public T Value { get; }
}
