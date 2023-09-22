using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Engine.GlobalServices.Settings;

public abstract class SettingsFileBase {

	internal readonly Dictionary<string, object> _settingValues;
	public event Action<SettingsFileBase>? SettingsChanged;

	protected SettingsFileBase() {
		_settingValues = new();
	}

	protected T Get<T>( [CallerMemberName] string caller = "" ) {
		Type t = GetType();
		var property = t.GetProperty( caller ) ?? throw new ArgumentException( $"Property {caller} not found in {t.Name}" );
		if ( !_settingValues.TryGetValue( caller, out var value ) )
			_settingValues[ caller ] = value = property.GetCustomAttribute<DefaultValueAttribute>()?.Value ?? throw new InvalidOperationException( $"Property {caller} has no default value" );
		if ( value is T tValue )
			return tValue;
		throw new Exception( $"Property {caller} is not of type {typeof( T ).Name}" );
	}

	protected void Set<T>( T? value, [CallerMemberName] string caller = "" ) {
		if ( _settingValues.TryGetValue( caller, out var oldValue ) && ( ( oldValue is T oldT && oldT.Equals( value ) ) || ( value is null && oldValue is null ) ) )
			return;
		if ( value is null ) {
			Type t = GetType();
			var property = t.GetProperty( caller ) ?? throw new ArgumentException( $"Property {caller} not found in {t.Name}" );
			value = ( (T?) property.GetCustomAttribute<DefaultValueAttribute>()?.Value ) ?? throw new ArgumentException( $"Property {caller} has no default value" );
		}
		_settingValues[ caller ] = value;
		SettingsChanged?.Invoke( this );
	}

	internal void SetValues( Dictionary<string, object> d ) {
		var propertiesByName = GetType().GetProperties().ToDictionary( p => p.Name );
		foreach ( var kvp in d ) {
			if ( kvp.Value is JsonElement element ) {
				if ( propertiesByName.TryGetValue( kvp.Key, out var property )  ) {
					var e = element.Deserialize( property.PropertyType );
					if ( e is not null )
						_settingValues[ kvp.Key ] = e;
				}
            } else {
				_settingValues[ kvp.Key ] = kvp.Value;
			}
		}
	}
}
