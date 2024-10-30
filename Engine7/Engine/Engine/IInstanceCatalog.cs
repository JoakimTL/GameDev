using System.Diagnostics.CodeAnalysis;

namespace Engine;

public interface IInstanceCatalog {
	IInstanceLibrary Library { get; }
	event Action<Type>? OnHostedTypeAdded;
	/// <summary>
	/// Finds the corresponding implementation of the contract type.<br/>
	/// If the contract type is a valid implementation it will be returned.<br/>
	/// If the contract type has been registered as a contract in the <see cref="IInstanceLibrary"/>, the contract's implementation type will be returned.<br/>
	/// If the contract type has a singular valid implementation within the loaded assemblies then that implementation will be returned.<br/>
	/// If no valid implementation is found null will be returned.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns>True if an implementation was found.</returns>
	bool TryResolve( Type contractType, [NotNullWhen( true )] out Type? implementationType );
	/// <summary>
	/// Adds the type to a list of services to be automatically initialized when the <see cref="IInstanceProvider"/> is instantiated.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns>True if the list didn't contain the type.</returns>
	bool Host<T>();
}
