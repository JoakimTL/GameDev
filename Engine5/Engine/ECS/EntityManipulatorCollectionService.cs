using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using System.Reflection;

namespace Engine.ECS;

public sealed class EntityManipulatorCollectionService : Identifiable, IModuleService {

	private readonly Dictionary<Type, ComponentTypeCollection> _requiredComponentTypes;
	private readonly Dictionary<Type, List<ComponentTypeCollection>> _requirementCollectionsContainingComponentType;

	public EntityManipulatorCollectionService() {
		var systems = AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes() ).Where( p => p.IsAssignableTo( typeof( SystemBase ) ) && !p.IsAbstract ).ToList();
		var validSystems = systems.Where( p => p.GetCustomAttributes<RequireAttribute>( false ).Any() ).ToList();
		var invalidSystems = systems.Except( validSystems );
		foreach ( var invalid in invalidSystems )
			this.LogWarning( $"{invalid.GetType().FullName} is not valid!" );

		_requiredComponentTypes = new();
		_requirementCollectionsContainingComponentType = new();
		foreach ( var valid in validSystems ) {
			ComponentTypeCollection componentTypeCollection = new( valid.GetCustomAttributes<RequireAttribute>().Select( p => p.RequiredType ) );
			_requiredComponentTypes.Add( valid.GetType(), componentTypeCollection );
			foreach ( var componentType in componentTypeCollection.ComponentTypes ) {
				if ( !_requirementCollectionsContainingComponentType.TryGetValue( componentType, out var list ) )
					_requirementCollectionsContainingComponentType.Add( componentType, list = new() );
				list.Add( componentTypeCollection );
			}
		}

	}

	public ComponentTypeCollection? GetRequiredComponentTypes( Type systemType )
		=> _requiredComponentTypes.TryGetValue( systemType, out var list ) ? list : null;

	public IEnumerable<ComponentTypeCollection>? GetRequiringComponentTypeCollections( Type componentType )
		=> _requirementCollectionsContainingComponentType.TryGetValue( componentType, out var list ) ? list : null;

}
