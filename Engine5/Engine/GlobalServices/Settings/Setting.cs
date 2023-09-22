namespace Engine.GlobalServices.Settings;

public class Setting {
    internal event Action<object>? SettingsChangedEvent;

    private object? _value;
    private readonly object _settings;

    public Setting( object settings ) {
        this._settings = settings ?? throw new ArgumentNullException( nameof( settings ) );
    }

    internal object? Value {
        get => _value;
        set {
            _value = value;
            SettingsChangedEvent?.Invoke( this._settings );
        }
    }
}

public sealed class Setting<T> : Setting {
    public Setting( object settings ) : base( settings ) { }

    public new T? Value {
        get => base.Value is T t ? t : default;
        set => base.Value = value;
    }
}

[AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
public class DefaultValueAttribute : Attribute {
    public object Value { get; }

    public DefaultValueAttribute( object value ) {
        Value = value ?? throw new ArgumentNullException( nameof( value ) );
    }
}
