namespace Engine.Structure;

public sealed class RestrictedServiceProvider<T> : ServiceProvider {
	protected override bool CanLoad( Type t ) => t.IsAssignableTo( typeof( T ) );
}
