﻿namespace Engine.Data.Datatypes;
public class TracedBox<T> : Box<T> {
	public event Action? ValueAccessed;

	public TracedBox( T value ) : base( value ) { }

	protected override T GetValue() {
		ValueAccessed?.Invoke();
		return base.GetValue();
	}
}

public class Box<T> : Identifiable, IBox<T> {
	private T _value;

	public event Action<T>? Changed;

	public Box( T value ) {
		this._value = value;
	}

	protected virtual T GetValue() => this._value;
	protected virtual void SetValue( T value ) {
		if ( Equals( this._value, value ) )
			return;
		this._value = value;
		Changed?.Invoke( this._value );
	}

	public T Value {
		get => GetValue();
		set => SetValue( value );
	}
}

public interface IBox<T> : IReadOnlyBox<T> {
	new T Value { get; set; }
}

public interface IReadOnlyBox<T> {
	T Value { get; }
	public event Action<T>? Changed;
}
