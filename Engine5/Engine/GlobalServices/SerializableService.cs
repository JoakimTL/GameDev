﻿using System.Reflection;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices;

public sealed class SerializableService : IGlobalService {

	private readonly Dictionary<Type, Guid> _guidFromTypes;
	private readonly Dictionary<Guid, Type> _typeFromGuids;

	public SerializableService( TypeService typeService ) {
		_guidFromTypes = new();
		_typeFromGuids = new();

		foreach ( var type in typeService.DerivedTypes.Where( p => p.IsAssignableTo( typeof( ISerializable ) ) ) ) {
			var prop = type.GetProperty( nameof( ISerializable.TypeIdentity ), BindingFlags.Static | BindingFlags.Public );
			Guid guid = prop?.GetValue( type, null ) as Guid? ?? Guid.Empty;
			if ( guid == Guid.Empty ) {
				Log.Warning( $"Unable to load guid for {type}." );
				continue;
			}
			_typeFromGuids.Add( guid, type );
			_guidFromTypes.Add( type, guid );
		}
	}

	public Type? GetFromIdentity( Guid id ) => _typeFromGuids.TryGetValue( id, out Type? type ) ? type : null;

	public Guid GetFromType( Type type ) => _guidFromTypes.TryGetValue( type, out Guid id ) ? id : Guid.Empty;

}