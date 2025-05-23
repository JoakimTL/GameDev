﻿using System.Runtime.CompilerServices;

namespace Engine.Structures;

public class SimpleSortedList<T>() where T : IComparable<T> {

	private readonly List<T> _underlying = [];

	public int Count => this._underlying.Count;

	public T this[ int index ] {
		get => this._underlying[ index ];
		set => this._underlying[ index ] = value;
	}

	public IReadOnlyList<T> AsReadOnly() => this._underlying.AsReadOnly();

	public void Add( T item ) {
		if (this.Count == 0) {
			this._underlying.Add( item );
			return;
		}
		int index = FindIndex( 0, this._underlying.Count, item );
		this._underlying.Insert( index, item );
	}

	public void Remove( T item ) {
		if (this.Count == 0)
			return;
		int index = FindIndex( 0, this._underlying.Count, item );
		if (index < 0 || index >= this._underlying.Count)
			return;
		this._underlying.RemoveAt( index );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private int FindIndex( int start, int end, T item ) {
		int middle;
		T valueAtIndex;
		int order;
		while (true) {
			middle = start + ((end - start) / 2);
			valueAtIndex = this._underlying[ middle ];
			order = item.CompareTo( valueAtIndex );
			if (order == 0)
				return middle;
			if (middle == start)
				return order < 0 ? start : end;
			if (order < 0) {
				end = middle;
			} else {
				start = middle;
			}
		}
	}

	public void Clear() => this._underlying.Clear();
}