namespace Engine.Serialization;

public sealed class SerializerProvider( IInstanceProvider instanceProvider ) {
	private readonly IInstanceProvider _instanceProvider = instanceProvider;

	public ISerializer? GetSerializerFor<TTarget>() => GetSerializerFor( typeof( TTarget ) );
	public ISerializer? GetSerializerFor( Type target ) {
		Type? type = TypeManager.Serializers.GetSerializerType( target );
		if (type is null)
			return null;
		return _instanceProvider.Get( type ) as ISerializer ?? throw new InvalidOperationException( $"Failed to instantiate serializer for {target}." );
	}

	public ISerializer? GetSerializerFor( Guid guid ) {
		Type? type = TypeManager.Serializers.GetSerializerType( guid );
		if (type is null)
			return null;
		return _instanceProvider.Get( type ) as ISerializer ?? throw new InvalidOperationException( $"Failed to instantiate serializer for {guid}." );
	}
}