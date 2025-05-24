using Engine.Serialization;
using System.Collections.Frozen;

namespace Engine;

public sealed class SerializerRegistry {
	private readonly FrozenDictionary<Guid, Type> _serializerTypesByGuid;
	private readonly FrozenDictionary<Type, Type> _serializerTypesByTargetType;

	public SerializerRegistry( TypeRegistry registry ) {
		Dictionary<Type, Type> serializersByType = [];
		Dictionary<Guid, Type> serializersByGuid = [];
		foreach (Type type in registry.GetAllNonAbstractSubclassesOf( typeof( SerializerBase<> ) ).Where( p => !p.IsAbstract )) {
			Type[] genericArguments = registry.GetGenericArgumentsOf( type, typeof( SerializerBase<> ) );
			if (genericArguments.Length != 1)
				throw new InvalidOperationException( $"Serializer {type.FullName} does not have exactly one generic argument." );
			Type targetType = genericArguments[ 0 ];
			Guid guid = type.Resolve().Guid ?? throw new InvalidOperationException( $"Serializer {type.FullName} does not have a GUID." );
			if (serializersByType.ContainsKey( targetType ))
				throw new InvalidOperationException( $"Serializer for {targetType} already registered." );
			if (serializersByGuid.ContainsKey( guid ))
				throw new InvalidOperationException( $"Serializer with GUID {guid} already registered." );
			serializersByGuid.Add( guid, type );
			serializersByType.Add( targetType, type );
		}
		this._serializerTypesByGuid = FrozenDictionary.ToFrozenDictionary( serializersByGuid );
		this._serializerTypesByTargetType = FrozenDictionary.ToFrozenDictionary( serializersByType );
	}

	public Type? GetSerializerType( Guid t ) => _serializerTypesByGuid.GetValueOrDefault( t );
	public Type? GetSerializerType( Type t ) => _serializerTypesByTargetType.GetValueOrDefault( t );
	public Type? GetSerializerType<TTarget>() => GetSerializerType( typeof( TTarget ) );
}