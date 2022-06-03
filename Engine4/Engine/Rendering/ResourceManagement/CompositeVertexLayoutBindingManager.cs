using Engine.Rendering.Standard.VertexArrayObjects;

namespace Engine.Rendering.ResourceManagement;

public class CompositeVertexLayoutBindingManager : Identifiable {

	private readonly Dictionary<Type, Type> _layouts;

	public CompositeVertexLayoutBindingManager() {
		this._layouts = new Dictionary<Type, Type>();

		IEnumerable<Type>? layoutTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes().Where( q => !q.IsAbstract && q.IsAssignableTo( typeof( CompositeVertexArrayObjectDataLayout ) ) ) );
		foreach ( Type type in layoutTypes ) {
			object[]? attributes = type.GetCustomAttributes( false );
			if ( attributes.Length == 0 ) {
				Log.Warning( $"Found {type.Name}, but it has no attributes and can't be used." );
				continue;
			}
			object? attribute = attributes.FirstOrDefault( p => p is VertexLayoutBindingAttribute );
			if ( attribute is null ) {
				Log.Warning( $"Found {type.Name}, but it has no binding attribute and can't be used." );
				continue;
			}
			VertexLayoutBindingAttribute? bindingAttribute = attribute as VertexLayoutBindingAttribute;
			if ( bindingAttribute is null ) {
				Log.Warning( $"Found {type.Name}, but it has no binding attribute and can't be used." );
				continue;
			}
			if ( this._layouts.ContainsKey( bindingAttribute.BindingType ) ) {
				Log.Warning( $"{bindingAttribute.BindingType.Name} is already bound, skipping {type.Name}!" );
				continue;
			}
			this._layouts.Add( bindingAttribute.BindingType, type );
			this.LogLine( $"Bound {bindingAttribute.BindingType.Name} to {type.Name}!", Log.Level.NORMAL, ConsoleColor.Green );
		}
	}

	public CompositeVertexArrayObjectDataLayout? GetLayout( Type t ) {
		if ( this._layouts.TryGetValue( t, out Type? layoutType ) )
			return Resources.Render.VAODataLayouts.Get( layoutType );
		Log.Warning( $"Attempting to access {t.Name} failed!" );
		return null;
	}

	public CompositeVertexArrayObjectDataLayout? GetLayout<T>()
		=> GetLayout( typeof( T ) );
}
