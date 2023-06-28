namespace Engine.Structure;

/// <summary>
/// Creates a new object whenever <see cref="Get(Type, bool)"/> is called. Does not cause exceptions.
/// </summary>
public sealed class SilentTransientInjector : DependencyInjectorBase, IDependencyInjector {

	public static readonly SilentTransientInjector Singleton = new();

    public object? Get( Type type ) => GetInternal( type );

    protected override object? GetInternal( Type t ) {
		try {
			return Create( t, false );
		} catch ( Exception e ) {
			this.LogWarning( e.Message );
		}
		try {
			return t.IsValueType ? Activator.CreateInstance( t ) : default;
		} catch ( Exception e ) {
			this.LogWarning( e.Message );
		}
		return default;
	}
}
