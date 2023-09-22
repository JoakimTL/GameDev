namespace Engine;

internal class ServiceProviderException : Exception {
	public readonly Type Type;

	public ServiceProviderException( Type t, string message, Exception? innerException ) : base( $"{message}{( message.Length > 0 ? Environment.NewLine : "" )}Unable to resolve type {t}!", innerException ) {
		this.Type = t;
	}

	public ServiceProviderException( Type t, string message = "" ) : this( t, message, null ) { }
}


