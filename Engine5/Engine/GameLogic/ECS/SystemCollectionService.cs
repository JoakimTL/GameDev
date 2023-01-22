using Engine.Structure.Attributes;
using System.Reflection;

namespace Engine.GameLogic.ECS;

public sealed class ComponentTypeCollectionService : Identifiable, IGameLogicService, IDisposable {

	private readonly Dictionary<Type, ComponentTypeCollection> _requiredComponentTypes;
	private readonly Dictionary<ComponentTypeCollection, List<Type>> _systemsRequiringComponentTypes;
	private readonly Dictionary<Type, List<ComponentTypeCollection>> _collectionsContainingComponentType;

	public ComponentTypeCollectionService() {
		_requiredComponentTypes = new();
		_collectionsContainingComponentType = new();
		_systemsRequiringComponentTypes = new();
		AppDomain.CurrentDomain.AssemblyLoad += NewAssemblyLoaded;
		var assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach ( var assembly in assemblies )
			LoadAssembly( assembly );
	}

	private void LoadAssembly( Assembly assembly ) {
		var systems = assembly.GetTypes().Where( p => p.IsAssignableTo( typeof( SystemBase ) ) && !p.IsAbstract ).ToList();
		var validSystems = systems.Where( p => p.GetCustomAttributes<RequireAttribute>( false ).Any() ).ToList();
		var invalidSystems = systems.Except( validSystems );

		foreach ( var invalid in invalidSystems )
			this.LogWarning( $"{invalid.GetType().FullName} is not valid!" );

		foreach ( var valid in validSystems ) {
			ComponentTypeCollection componentTypeCollection = new( valid.GetCustomAttributes<RequireAttribute>().Select( p => p.RequiredType ) );
			_requiredComponentTypes.Add( valid, componentTypeCollection );
			{
				if ( !_systemsRequiringComponentTypes.TryGetValue( componentTypeCollection, out var list ) )
					_systemsRequiringComponentTypes.Add( componentTypeCollection, list = new() );
				list.Add( valid );
			}

			foreach ( var componentType in componentTypeCollection.ComponentTypes ) {
				if ( !_collectionsContainingComponentType.TryGetValue( componentType, out var list ) )
					_collectionsContainingComponentType.Add( componentType, list = new() );
				list.Add( componentTypeCollection );
			}
		}
	}

	public ComponentTypeCollection? GetRequiredComponentTypes( Type systemType )
		=> _requiredComponentTypes.TryGetValue( systemType, out var list ) ? list : null;

	public IEnumerable<Type> GetSystemsRequiringComponentTypeCollection( ComponentTypeCollection collection )
		=> _systemsRequiringComponentTypes.TryGetValue( collection, out var list ) ? list : Enumerable.Empty<Type>();

	public IEnumerable<ComponentTypeCollection> GetRequiringComponentTypeCollections( Type componentType )
		=> _collectionsContainingComponentType.TryGetValue( componentType, out var list ) ? list : Enumerable.Empty<ComponentTypeCollection>();

	public void Dispose()
		=> AppDomain.CurrentDomain.AssemblyLoad -= NewAssemblyLoaded;

	private void NewAssemblyLoaded( object? sender, AssemblyLoadEventArgs e )
		=> LoadAssembly( e.LoadedAssembly );
}
