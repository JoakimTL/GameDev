namespace Engine;

public interface IInstanceProvider : IDisposable {
	IInstanceCatalog Catalog { get; }
	event Action<object>? OnInstanceAdded;
	object Get( Type t );
}
