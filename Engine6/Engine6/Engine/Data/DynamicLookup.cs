﻿namespace Engine.Data;

public sealed class DynamicLookup<TKey, TValue> where TKey : notnull {

	private readonly Dictionary<TKey, HashSet<TValue>> _dictionary;

	public IReadOnlyCollection<TKey> Keys => this._dictionary.Keys;

	public DynamicLookup() {
		this._dictionary = [];
	}

	public void Add( TKey key, TValue value ) {
		if ( key is null )
			throw new ArgumentNullException( nameof( key ) );
		if ( value is null )
			throw new ArgumentNullException( nameof( value ) );
		if ( !this._dictionary.TryGetValue( key, out HashSet<TValue>? values ) )
			this._dictionary.Add( key, values = [] );
		values.Add( value );
	}

	public void AddRange( TKey key, IEnumerable<TValue> values ) {
		if ( key is null )
			throw new ArgumentNullException( nameof( key ) );
		if ( values is null )
			throw new ArgumentNullException( nameof( values ) );
		if ( !this._dictionary.TryGetValue( key, out HashSet<TValue>? valueSet ) )
			this._dictionary.Add( key, valueSet = [] );
		foreach ( TValue value in values )
			valueSet.Add( value );
	}
	public void AddRange( TKey key, Span<TValue> values ) {
		if ( key is null )
			throw new ArgumentNullException( nameof( key ) );
		if ( !this._dictionary.TryGetValue( key, out HashSet<TValue>? valueSet ) )
			this._dictionary.Add( key, valueSet = [] );
		foreach ( TValue value in values )
			valueSet.Add( value );
	}

	public void Remove( TKey key, TValue value ) {
		if ( key is null )
			throw new ArgumentNullException( nameof( key ) );
		if ( value is null )
			throw new ArgumentNullException( nameof( value ) );
		if ( !this._dictionary.TryGetValue( key, out HashSet<TValue>? values ) )
			return;
		if ( values.Remove( value ) && values.Count == 0 )
			this._dictionary.Remove( key );
	}

	public void Remove( TKey key ) {
		if ( key is null )
			throw new ArgumentNullException( nameof( key ) );
		this._dictionary.Remove( key );
	}

	public IReadOnlyCollection<TValue> this[ TKey key ] => GetValues( key );

	public IReadOnlyCollection<TValue> GetValues( TKey key ) {
		if ( key is null )
			throw new ArgumentNullException( nameof( key ) );
		if ( !this._dictionary.TryGetValue( key, out HashSet<TValue>? values ) )
			return Array.Empty<TValue>();
		return values;
	}

	/// <summary>
	/// Note: generates a new hashset.
	/// </summary>
	/// <returns>A new hashset with all values present in the lookup</returns>
	public HashSet<TValue> GetAllValues() {
		HashSet<TValue> allValues = [];
		foreach ( HashSet<TValue> values in this._dictionary.Values )
			allValues.UnionWith( values );
		return allValues;
	}
}