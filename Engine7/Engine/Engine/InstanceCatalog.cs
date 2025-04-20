using System.Diagnostics.CodeAnalysis;

namespace Engine;

internal sealed class InstanceCatalog( InstanceLibrary instanceLibrary ) : IInstanceCatalog {

	private readonly HashSet<Type> _hostedTypes = [];
	private readonly InstanceLibrary _instanceLibrary = instanceLibrary;
	public IInstanceLibrary Library => this._instanceLibrary;
	internal IReadOnlyCollection<Type> HostedTypes => this._hostedTypes;
	public event Action<Type>? OnHostedTypeAdded;

	public bool Host<T>() => Host( typeof( T ) );
	public bool Host( Type t ) {
		if (this._hostedTypes.Add( t )) {
			OnHostedTypeAdded?.Invoke( t );
			return true;
		}
		return false;
	}

	public bool TryResolve( Type contractType, [NotNullWhen( true )] out Type? implementationType ) {
		if (contractType.IsClass && !contractType.IsAbstract) {
			implementationType = contractType;
			return true;
		}
		if (this._instanceLibrary.Connections.TryGetValue( contractType, out implementationType ))
			return true;

		Type[] eligibleTypes = [ .. TypeManager.Registry.AllTypes.Where( p => p.IsClass && !p.IsAbstract && p.IsAssignableTo( contractType ) ) ];
		if (eligibleTypes.Length == 1) {
			implementationType = eligibleTypes[ 0 ];
			return true;
		}
		return false;
	}
}
