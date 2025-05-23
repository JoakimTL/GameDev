﻿using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Engine;

public sealed class ResolvedType {

	public Type Type { get; }
	public string? Identity { get; }
	public bool HasParameterlessConstructor { get; }
	public int ConstructorCount { get; }
	public IReadOnlyList<Attribute> Attributes { get; }
	/// <summary>
	/// If this type has a Guid Attribute it's value is stored here.
	/// </summary>
	public Guid? Guid { get; }
	private readonly ConcurrentDictionary<Type, object> _attributesByType = [];
	private readonly ConcurrentDictionary<BindingFlags, List<PropertyInfo>> _propertiesByBindingFlags = [];
	private readonly TypeInstanceFactory _instanceFactory;
	private readonly ConcurrentDictionary<PropertyInfo, TypePropertyAccessor> _propertyAccessors = [];

	public ResolvedType( Type type ) {
		this.Type = type;
		this.HasParameterlessConstructor = type.IsValueType || type.GetConstructor( Type.EmptyTypes ) != null;
		this.ConstructorCount = type.GetConstructors().Length;
		this.Attributes = type.GetCustomAttributes( true ).OfType<Attribute>().ToArray();
		this._instanceFactory = new TypeInstanceFactory( type );
		this.Identity = Attributes.OfType<IdentityAttribute>().FirstOrDefault()?.Identity;
		string? guidString = Attributes.OfType<GuidAttribute>().FirstOrDefault()?.Value;
		Guid = guidString is not null ? new( guidString ) : null;
	}

	public IReadOnlyList<T> GetAttributes<T>() {
		lock (this._attributesByType) {
			if (this._attributesByType.TryGetValue( typeof( T ), out object? list ))
				return (IReadOnlyList<T>) list;
			list = new List<T>( this.Attributes.OfType<T>() );
			this._attributesByType.TryAdd( typeof( T ), list );
			return (IReadOnlyList<T>) list;
		}
	}

	public IReadOnlyList<PropertyInfo> GetProperties( BindingFlags flags ) {
		if (this._propertiesByBindingFlags.TryGetValue( flags, out List<PropertyInfo>? list ))
			return list;
		list = [ .. this.Type.GetProperties( flags ) ];
		this._propertiesByBindingFlags.TryAdd( flags, list );
		return list;
	}

	public object? CreateInstance( object[]? parameters ) => this._instanceFactory.CreateInstance( parameters );

	public TypePropertyAccessor GetPropertyAccessor( PropertyInfo property ) {
		if (this._propertyAccessors.TryGetValue( property, out TypePropertyAccessor? accessor ))
			return accessor;
		accessor = new TypePropertyAccessor( property );
		this._propertyAccessors.TryAdd( property, accessor );
		return accessor;
	}
}
