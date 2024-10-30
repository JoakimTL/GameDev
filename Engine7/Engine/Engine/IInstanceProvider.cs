namespace Engine;

public interface IInstanceProvider {
	IInstanceCatalog Catalog { get; }
	event Action<object>? OnInstanceAdded;
	object Get( Type t );
}
