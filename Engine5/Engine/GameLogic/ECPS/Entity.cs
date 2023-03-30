using Engine.Datatypes;
using Engine.Structure;
using Engine.Structure.Interfaces;

namespace Engine.GameLogic.ECPS;
public sealed class Entity : DependencyInjectorBase, ICustomizedSerializable {

	public Guid EntityId { get; private set; }
	public Guid? ParentId { get; private set; }
	public Entity? Parent { get; private set; }
	private readonly Dictionary<Type, ComponentBase> _components;
	public event EntityComponentEvent? ComponentAdded;
	public event EntityComponentEvent? ComponentRemoved;
	public event EntityParentIdEvent? ParentIdChanged;
	public event EntityParentEvent? ParentChanged;

	protected override string UniqueNameTag => string.Join( ", ", _components.Values );

	internal Entity() {
		_components = new();
	}

	internal IEnumerable<ComponentBase> Components => _components.Values;

	public static Guid SerializationIdentity { get; } = new Guid( "8aa95334-e883-4de4-ac96-e0ca5deeb1dc" );
    public bool ShouldSerialize => true;

    internal void SetId( Guid guid ) => EntityId = guid;

	public void SetParentId( Guid? parentId ) {
		if ( ParentId == parentId )
			return;
		ParentId = parentId;
		ParentIdChanged?.Invoke( this, ParentId );
	}

	internal void SetParent( Entity? parent ) {
		if ( Parent == parent )
			return;
		Parent = parent;
		ParentChanged?.Invoke( this, Parent );
	}

	public T AddOrGet<T>() where T : ComponentBase, new() => (T) AddOrGet( typeof( T ) );
	public T? Get<T>() where T : ComponentBase, new() => Get(typeof(T)) as T;
	public T GetOrThrow<T>() where T : ComponentBase, new() => Get(typeof(T)) as T ?? throw new NullReferenceException($"Missing component {typeof(T).Name}");
	public T? RemoveAndGet<T>() where T : ComponentBase, new() => RemoveAndGet( typeof( T ) ) as T;
	public void Remove<T>() where T : ComponentBase, new() => RemoveAndGet( typeof( T ) );
	public void Remove( Type compoentType ) => RemoveAndGet( compoentType );

	private ComponentBase? Get( Type t ) => _components.TryGetValue( t, out var c ) ? c : null;

	private ComponentBase? RemoveAndGet( Type type ) {
		if ( _components.Remove( type, out var value ) ) {
			ComponentRemoved?.Invoke( value );
			value.Dispose();
			return value;
		}
		return null;
	}

	protected override object? GetInternal( Type t ) => AddOrGet( t );

	internal ComponentBase AddOrGet( Type type ) {
		if ( _components.TryGetValue( type, out ComponentBase? value ) )
			return value;
		return Add( type ) ?? throw new NullReferenceException( "Value should not be null here." );
	}

	private ComponentBase? Add( Type type ) {
		if ( Create( type, false ) is not ComponentBase value )
			return null;
		value.SetOwner( this );
		_components.Add( type, value );
		ComponentAdded?.Invoke( value );
		return value;
	}

	public bool HasAllComponents( ComponentTypeCollection componentTypeCollection ) {
		foreach ( var componentType in componentTypeCollection.ComponentTypes )
			if ( !_components.ContainsKey( componentType ) )
				return false;
		return true;
	}

	public bool DeserializeData( byte[] data ) {
		unsafe {
			fixed ( byte* srcPtr = data ) {
				EntityId = *(Guid*) srcPtr;
				ParentId = *(Guid*) ( srcPtr + sizeof( Guid ) );

				if ( data.Length <= sizeof( Guid ) * 2 )
					return true;

				var segments = Segmentation.Parse( data, (uint) sizeof( Guid ) * 2 );
				if ( segments is null )
					return this.LogWarningThenReturn( $"Error parsing component data for {EntityId}!", false );
				for ( int i = 0; i < segments.Length; i++ )
					if ( !DeserializeComponent( segments[ i ], out Type? addedType ) )
						if ( addedType is not null )
							RemoveAndGet( addedType );
			}
		}
		return true;
	}

	private unsafe bool DeserializeComponent( byte[] componentData, out Type? componentType ) {
		componentType = null;
        if (componentData.Length < sizeof(Guid))
            return this.LogWarningThenReturn($"Component data length not enough for a {nameof(Guid)}", false);
        object? obj;
		componentType = componentData.GetDeserializedType();
		if ( componentType is null )
			return this.LogWarningThenReturn( $"Unable to determine component type from {nameof( Guid )}", false );
		obj = AddOrGet( componentType );
		if ( obj is null )
			return this.LogWarningThenReturn( "Construction of component failed", false );
		if ( obj is not ICustomizedSerializable serializable )
			return this.LogWarningThenReturn( "Component is not serializable", false );
		if ( !serializable.DeserializeData( componentData[ sizeof( Guid ).. ] ) )
			return this.LogWarningThenReturn( "Deserializing the component failed", false );
		return true;
	}

	public byte[] SerializeData() {
		unsafe {
			List<byte[]> serializedComponents = new();
			foreach ( var component in Components )
				if ( component is ICustomizedSerializable s && s.ShouldSerialize )
					serializedComponents.Add( s.Serialize() );
			var data = Segmentation.SegmentWithPadding( (uint) sizeof( Guid ) * 2, 0, serializedComponents );
			EntityId.CopyInto( data );
			( ParentId ?? Guid.Empty ).CopyInto( data, (uint) sizeof( Guid ) );
			return data;
		}
	}


	//public static object? DeserializeData( ReadOnlyMemory<byte> data ) {

	//}

	//public ReadOnlyMemory<byte> SerializeData() {
	//	unsafe {
	//		List<ReadOnlyMemory<byte>> data = new() {
	//			EntityId.ToByteArray(),
	//			Parent?.EntityId.ToByteArray() ?? Guid.Empty.ToByteArray()
	//		};
	//		foreach ( var c in _components.Values )
	//			if ( c is ISerializable s )
	//				data.Add( s.SerializeData() );


	//	}
	//}
}
