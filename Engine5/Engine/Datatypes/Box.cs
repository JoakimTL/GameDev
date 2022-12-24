namespace Engine.Datatypes;
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
		_value = value;
	}

	protected virtual T GetValue() => _value;
	protected virtual void SetValue( T value ) {
		if ( Equals( _value, value ) )
			return;
		_value = value;
		Changed?.Invoke( _value );
	}

	public T Value {
		get => GetValue();
		set => SetValue( value );
	}

	public override string ToString() {
		return _value.ToString();
	}
}

public interface IBox<T> : IReadOnlyBox<T> {
	new T Value { get; set; }
}

public interface IReadOnlyBox<T> {
	T Value { get; }
	public event Action<T>? Changed;
}
