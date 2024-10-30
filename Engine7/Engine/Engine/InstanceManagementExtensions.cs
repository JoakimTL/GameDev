using System.Diagnostics.CodeAnalysis;

namespace Engine;

public static class InstanceManagementExtensions {
	public static InstanceUpdaterExtension CreateUpdater( this IInstanceProvider instanceProvider ) => new( instanceProvider );
	public static InstanceInitializerExtension CreateInitializer( this IInstanceProvider instanceProvider ) => new( instanceProvider );
	public static T Get<T>( this IInstanceProvider instanceProvider ) => (T) instanceProvider.Get( typeof( T ) );
	/// <summary>
	/// Finds the corresponding implementation of the input type.<br/>
	/// If the input type is a valid implementation it will be returned.<br/>
	/// If the input type has been registered as a contract in the <see cref="IInstanceLibrary"/>, the contract's implementation type will be returned.<br/>
	/// If the input type has a singular valid implementation within the loaded assemblies then that implementation will be returned.<br/>
	/// If no valid implementation is found null will be returned.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="implementationType"></param>
	/// <returns>True if an implementation was found.</returns>
	public static bool TryResolve<T>( this IInstanceCatalog instanceCatalog, [NotNullWhen( true )] out Type? implementationType ) => instanceCatalog.TryResolve( typeof( T ), out implementationType );
}
