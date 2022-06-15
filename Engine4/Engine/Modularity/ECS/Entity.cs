using System.Diagnostics.CodeAnalysis;
using Engine.Data;
using Engine.Structure;

namespace Engine.Modularity.ECS;
public class Entity : Identifiable, IUpdateable {

	public ulong Owner { get; private set; }
	public EntityManager? Manager { get; private set; }
	private readonly HashSet<Component> _components;
	private readonly HashSet<UpdateableComponent> _updateableComponents;
	private readonly Dictionary<Type, Component> _componentRefs;

	public event Action<Entity, Component>? ComponentAdded;
	public event Action<Entity, Component>? ComponentRemoved;
	public event Action<Entity, Component>? ComponentChanged;

	public bool Active { get; set; }

	public Entity( string name, Guid guid, ulong owner ) : base( name, guid ) {
		this._components = new HashSet<Component>();
		this._updateableComponents = new HashSet<UpdateableComponent>();
		this._componentRefs = new Dictionary<Type, Component>();
		this.Owner = owner;
	}

	internal void SetManager( EntityManager? mng ) => this.Manager = mng;

	public void Update( float time, float deltaTime ) {
		foreach ( UpdateableComponent? component in this._updateableComponents )
			component.Update( time, deltaTime );
	}

	public T? AddComponent<T>( T? component ) where T : Component {
		if ( component is null )
			return null;
		Type originalType = component.GetType();
		Type type = Resources.Get<ComponentTypeRegistry>().GetRegisteredAs( originalType );
		if ( this._componentRefs.ContainsKey( type ) )
			return null;
		if ( originalType != type ) {
			if ( this._componentRefs.ContainsKey( originalType ) )
				return null;
			this._componentRefs.Add( originalType, component );
		}
		this._componentRefs.Add( type, component );
		this._components.Add( component );
		component.SetEntity( this );
		component.Changed += OnComponentChanged;
		if ( component is UpdateableComponent uComponent )
			this._updateableComponents.Add( uComponent );
		ComponentAdded?.Invoke( this, component );
		return component;
	}

	public Component? RemoveComponent( Type type ) {
		if ( this._componentRefs.Remove( Resources.Get<ComponentTypeRegistry>().GetRegisteredAs( type ), out Component? component ) ) {
			this._components.Remove( component );
			Type refType = Resources.Get<ComponentTypeRegistry>().GetRegisteredAs( type );
			if ( refType != type )
				this._componentRefs.Remove( refType );
			this._componentRefs.Remove( component.GetType() );
			component.Removed();
			if ( component is UpdateableComponent uComponent )
				this._updateableComponents.Remove( uComponent );
			ComponentRemoved?.Invoke( this, component );
			component.Changed -= OnComponentChanged;
			return component;
		}
		return null;
	}

	public T? RemoveComponent<T>() where T : Component
		=> RemoveComponent( typeof( T ) ) as T;

	public bool HasComponent( Type t ) => this._componentRefs.ContainsKey( t );
	public bool HasComponent<T>() where T : Component => HasComponent( typeof( T ) );

	public bool TryGetComponent<T>( [NotNullWhen( returnValue: true )] out T? component ) where T : Component {
		component = null;
		if ( this._componentRefs.TryGetValue( typeof( T ), out Component? c ) )
			if ( c is T t ) {
				component = t;
				return true;
			}
		return false;
	}

	public T? GetComponent<T>() where T : Component
		=> TryGetComponent( out T? t ) ? t : null;

	public Component? GetComponent( Type t ) => this._componentRefs.TryGetValue( t, out Component? c ) ? c : null;

	private void OnComponentChanged( Component component ) => ComponentChanged?.Invoke( this, component );

	internal void ForeachComponent( Action<Entity, Component> func ) {
		foreach ( Component? component in this._components )
			func.Invoke( this, component );
	}

	private byte[] SerializeIdentification() => Segmentation.Segment( DataUtils.ToBytes( this.Name ), this.Guid.ToByteArray(), DataUtils.ToBytes( this.Owner ) );
	public static bool DeserializeIdentification( byte[] data, [NotNullWhen( true )] out string? name, [NotNullWhen( true )] out Guid? guid, out ulong owner ) {
		name = null;
		guid = null;
		owner = 0;
		byte[][]? identification = Segmentation.Parse( data );
		if ( identification is null || identification.Length != 3 )
			return false;
		name = DataUtils.ToStringUTF8( identification[ 0 ] );
		if ( name is null )
			return false;
		guid = new Guid( identification[ 1 ] );
		owner = DataUtils.ToUnmanaged<ulong>( identification[ 2 ] ) ?? 0;
		return true;
	}

	public static byte[]? Serialize( Entity e ) {
		List<byte[]> componentData = new();
		componentData.Add( e.SerializeIdentification() );
		foreach ( Component component in e._components ) {
			byte[]? serializedComponent = component.Serialize();
			if ( serializedComponent is null )
				continue;
			componentData.Add( serializedComponent );
		}
		byte[]? segmentatedData = Segmentation.Segment( componentData.ToArray() );
		if ( segmentatedData is null ) {
			e.LogError( "Corrupt data!" );
			return null;
		}
		return segmentatedData;
	}
}
