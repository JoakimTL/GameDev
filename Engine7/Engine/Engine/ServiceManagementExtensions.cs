using System.Diagnostics.CodeAnalysis;

namespace Engine;

public static class ServiceManagementExtensions {
	public static ServiceUpdaterExtension CreateUpdater( this IServiceProvider serviceProvider ) => new( serviceProvider );
	public static ServiceInitializerExtension CreateInitializer( this IServiceProvider serviceProvider ) => new( serviceProvider );
	public static T Get<T>( this IServiceProvider serviceProvider ) => (T) serviceProvider.Get( typeof( T ) );
	/// <summary>
	/// Finds the corresponding implementation of the input type.<br/>
	/// If the input type is a valid implementation it will be returned.<br/>
	/// If the input type has been registered as a contract in the <see cref="IServiceLibrary"/>, the contract's implementation type will be returned.<br/>
	/// If the input type has a singular valid implementation within the loaded assemblies then that implementation will be returned.<br/>
	/// If no valid implementation is found null will be returned.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="implementationType"></param>
	/// <returns>True if an implementation was found.</returns>
	public static bool TryResolve<T>( this IServiceCatalog serviceCatalog, [NotNullWhen( true )] out Type? implementationType ) => serviceCatalog.TryResolve( typeof( T ), out implementationType );
}
