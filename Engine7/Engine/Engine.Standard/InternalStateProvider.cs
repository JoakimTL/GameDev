namespace Engine.Standard;

public sealed class InternalStateProvider : IServiceProvider {

	private readonly Dictionary<string, object> _state = [];

	public void Set( string name, object? value ) {
		if (value is null) {
			this._state.Remove( name );
			return;
		}
		if (this._state.TryGetValue( name, out object? oldValue ) && oldValue.Equals( value ))
			return;
		this._state[ name ] = value;
	}

	public T? Get<T>( string name ) {
		if (!this._state.TryGetValue( name, out object? value ))
			return default;
		if (value is T t)
			return t;
		return default;
	}
}