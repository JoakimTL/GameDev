namespace Engine;

public interface IInstanceProvider : IDisposable {
	IInstanceCatalog Catalog { get; }
	event Action<object>? OnInstanceAdded;
	object Get( Type t );
	/// <summary>
	/// Injects the instance to the list of available instances. The instance will be returned when the type is requested.
	/// </summary>
	/// <param name="triggerEvents">If true and the instance is added, the OnInstanceAdded event will be triggered.</param>
	/// <returns>True if the instance was added.</returns>
	bool Inject<T>( T instance, bool triggerEvents );
}
